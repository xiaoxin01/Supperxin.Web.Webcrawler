using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotnetSpider.Core;
using DotnetSpider.Core.Processor;
using HtmlAgilityPack;
using Supperxin.Web.Webcrawler.ValueContainers;
using Microsoft.Extensions.DependencyInjection;
using Supperxin.Web.Webcrawler.PageIterations;
using Supperxin.Web.Webcrawler.Operations;

namespace Supperxin.Web.Webcrawler
{
    public class GeneralProcessor : BasePageProcessor
    {
        private readonly CrawlJob job;
        private readonly IOperationFactory _operationFactory;

        public GeneralProcessor(CrawlJob job)
        {
            this.job = job;
            this._operationFactory = Program.ServiceProvider.GetService<Operations.IOperationFactory>();
        }
        protected override void Handle(Page page)
        {
            if (PageIsResultItem(page.Url))
            {
                var itemMeta = this.job.ItemMetaCache.ContainsKey(page.Url) ? this.job.ItemMetaCache[page.Url] : new Dictionary<string, object>();
                //itemMeta.Add("Url", page.Url);

                object value = null;
                foreach (var field in job.Fields)
                {
                    if (string.IsNullOrEmpty(field.FieldValue))
                    {
                        value = page.Selectable.Selector(field.XPath).GetValue();
                    }
                    else
                    {
                        value = field.FieldValue;
                    }
                    OperationFields(field.FieldName, ref value);
                    itemMeta.Add(field.FieldName, value);
                }

                // if (this.job.CheckCacheMetas.ContainsKey(page.Url))
                // {
                //     foreach (var meta in this.job.CheckCacheMetas[page.Url])
                //     {
                //         itemMeta.Add(meta.Key, meta.Value);
                //     }
                // }
                page.AddResultItem(page.Url, itemMeta);
            }
            else
            //(PageIsListPage(page))
            {
                var pageMeta = new Dictionary<string, object>();
                var pageCacheMeta = new Dictionary<string, object>();
                object value = null;
                foreach (var meta in this.job.Metas.Where(m => m.XPathFrom == "Page"))
                {
                    var selectedResults = page.Selectable.Selector(meta.XPath);
                    switch (meta.FieldType)
                    {
                        case "Array":
                            value = !string.IsNullOrEmpty(meta.Regex) ?
                                (from r in selectedResults.GetValues() select (Regex.Match(r, meta.Regex).Value)).ToArray() : selectedResults.GetValues();
                            break;
                        default:
                            value = selectedResults.GetValue(); value = !string.IsNullOrEmpty(meta.Regex) ? Regex.Match(value as string, meta.Regex).Value : value;
                            break;
                    }

                    OperationFields(meta.FieldName, ref value);
                    pageMeta.Add(meta.FieldName, value);
                    if (meta.CheckCache)
                    {
                        pageCacheMeta.Add(meta.FieldName, value);
                    }
                }

                // services
                var valueGetterFactory = Program.ServiceProvider.GetService<IValueGetterFactory>();


                var itemsHtml = page.Selectable.Selector(this.job.ResultItemXPath, this.job.ValueContainerType).GetValues();
                var hasCachedPage = false;
                var itemMeta = pageMeta.ToDictionary(v => v.Key, v => v.Value);

                foreach (var html in itemsHtml)
                {
                    // if page is not match result item regex, ignore
                    if (!PageIsResultItem(html))
                    {
                        continue;
                    }
                    var valueContainer = valueGetterFactory.CreateValueGetter(this.job.ValueContainerType, html);

                    itemMeta = pageMeta.ToDictionary(v => v.Key, v => v.Value);
                    var checkCacheMeta = pageCacheMeta.ToDictionary(v => v.Key, v => v.Value);
                    value = null;
                    foreach (var meta in this.job.Metas.Where(m => m.XPathFrom != "Page"))
                    {
                        value = valueContainer.GetValue<object>(meta);
                        OperationFields(meta.FieldName, ref value);

                        itemMeta.Add(meta.FieldName, value);
                        if (meta.CheckCache)
                        {
                            checkCacheMeta.Add(meta.FieldName, value);
                        }
                    }

                    // Check cache
                    if (checkCacheMeta.Count > 0)
                    {
                        // convention: first meta field is the key.
                        var itemKey = checkCacheMeta.Values.First() as string;
                        var cacheExists = this.job.CheckCacheMetas.ContainsKey(itemKey);
                        if (cacheExists)
                        {
                            var cacheItem = this.job.CheckCacheMetas[itemKey];
                            bool cacheMatch = true;
                            foreach (var key in checkCacheMeta.Keys)
                            {
                                if (!cacheItem.ContainsKey(key) || (string)cacheItem[key] != (string)checkCacheMeta[key])
                                {
                                    cacheMatch = false;
                                    break;
                                }
                            }
                            if (cacheMatch)
                            {
                                Console.WriteLine($"  Cache Match: {itemKey}");
                                hasCachedPage = true;
                                continue;
                            }
                            else
                            {
                                this.job.CheckCacheMetas[itemKey] = checkCacheMeta;
                                this.job.CheckCacheMetasChanged = true;
                            }
                        }
                        else
                        {
                            this.job.CheckCacheMetas.Add(itemKey, checkCacheMeta);
                            this.job.CheckCacheMetasChanged = true;
                        }
                    }



                    if (itemMeta.ContainsKey("Url"))
                    {
                        if (this.job.AddResultItemDirectly)
                        {
                            page.AddResultItem(itemMeta["Url"] as string, itemMeta);
                            Console.WriteLine($"  Add item: {itemMeta["Url"]}");
                        }
                        else
                        {
                            // add itemMeta to cache, so when crawl the page detail, can use these meta.
                            if (!this.job.ItemMetaCache.ContainsKey(itemMeta["Url"] as string))
                                this.job.ItemMetaCache.Add(itemMeta["Url"] as string, itemMeta);
                            else
                                this.job.ItemMetaCache[itemMeta["Url"] as string] = itemMeta;

                            page.AddTargetRequest(itemMeta["Url"] as string);
                        }
                    }
                }

                // means all item in this page is crawled, so need to check next page
                // !hasCachedPage means this page contains cached item, so the rest pages is crawled already.
                // items need to > 0 because a page not exist may return 0 records

                if (null != this.job.PageIteration && !hasCachedPage /*&& itemsHtml.Count() > 0 */)
                {
                    var nextPageUrl = this.job.PageIteration.GetNextPage(itemMeta);
                    if (!string.IsNullOrEmpty(nextPageUrl))
                    {
                        Console.WriteLine($" --> crawl next page : {nextPageUrl}");
                        page.AddTargetRequest(nextPageUrl);
                    }
                }

                // if (!string.IsNullOrEmpty(this.job.ListPageFormat) && !hasCachedPage && itemsHtml.Count() > 0)
                // {
                //     var nextPageUrl = string.Format(this.job.ListPageFormat, this.job.ListPageStart++);
                //     page.AddTargetRequest(nextPageUrl);
                // }


                // var values = page.Selectable.Links().Regex(this.job.TargetUrlRegex).GetValues();
                // page.AddTargetRequests(values);
            }
            // // not scan pages from detail page.
            // // only add page from list page.
            // else
            // {
            //     var values = page.Selectable.Links().Regex(this.job.TargetUrlRegex).GetValues();
            //     page.AddTargetRequests(values);
            // }
        }

        private void OperationFields(string fieldName, ref object value)
        {
            if (null == this.job.Operations || !this.job.Operations.Exists(o => o.FieldName == fieldName))
                return;

            var operation = this.job.Operations.First(o => o.FieldName == fieldName);

            var opObject = _operationFactory.MakeOperation(operation.OperationName);
            value = opObject.Operate(value, operation.Parameters);
        }

        private bool PageIsResultItem(string url)
        {
            return Regex.IsMatch(url, this.job.IsItemPageCheckRegex);
        }
    }
}
