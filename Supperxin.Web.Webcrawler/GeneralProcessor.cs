using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotnetSpider.Core;
using DotnetSpider.Core.Processor;
using HtmlAgilityPack;
using Supperxin.Web.Webcrawler.ValueContainers;
using Microsoft.Extensions.DependencyInjection;

namespace Supperxin.Web.Webcrawler
{
    public class GeneralProcessor : BasePageProcessor
    {
        private readonly CrawlJob job;

        public GeneralProcessor(CrawlJob job)
        {
            this.job = job;
        }
        protected override void Handle(Page page)
        {
            if (PageIsResultItem(page.Url))
            {
                var itemMeta = this.job.ItemMetaCache.ContainsKey(page.Url) ? this.job.ItemMetaCache[page.Url] : new Dictionary<string, object>();
                //itemMeta.Add("Url", page.Url);

                foreach (var field in job.Fields)
                {
                    if (string.IsNullOrEmpty(field.FieldValue))
                    {
                        itemMeta.Add(field.FieldName, page.Selectable.Selector(field.XPath).GetValue());
                    }
                    else
                    {
                        itemMeta.Add(field.FieldName, field.FieldValue);
                    }
                }

                // make operation to field
                // duplicate with page meta, need to improve
                OperationFields(itemMeta);

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

                    pageMeta.Add(meta.FieldName, value);
                    if (meta.CheckCache)
                    {
                        pageCacheMeta.Add(meta.FieldName, value);
                    }
                }



                var itemsHtml = page.Selectable.Selector(this.job.ResultItemXPath, this.job.ValueContainerType).GetValues();

                var valueGetterFactory = Program.ServiceProvider.GetService<IValueGetterFactory>();
                var hasCachedPage = false;

                foreach (var html in itemsHtml)
                {
                    // if page is not match result item regex, ignore
                    if (!PageIsResultItem(html))
                    {
                        continue;
                    }
                    var valueContainer = valueGetterFactory.CreateValueGetter(this.job.ValueContainerType, html);

                    var itemMeta = pageMeta.ToDictionary(v => v.Key, v => v.Value);
                    var checkCacheMeta = pageCacheMeta.ToDictionary(v => v.Key, v => v.Value);
                    value = null;
                    foreach (var meta in this.job.Metas.Where(m => m.XPathFrom != "Page"))
                    {
                        value = valueContainer.GetValue<object>(meta);

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
                            // make operation to field
                            OperationFields(itemMeta);
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
                    var nextPageUrl = this.job.PageIteration.GetNextPage();
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

        private void OperationFields(Dictionary<string, object> itemMeta)
        {
            if (null == this.job.Operations)
                return;

            foreach (var operation in this.job.Operations)
            {
                var opObject = Operations.OperationFactory.MakeOperatoin(operation.OperationName);
                var opValue = opObject.Operate(itemMeta[operation.FieldName], operation.Parameters);
                itemMeta[operation.FieldName] = opValue;
            }
        }

        private bool PageIsResultItem(string url)
        {
            return Regex.IsMatch(url, this.job.IsItemPageCheckRegex);
        }
    }
}
