using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotnetSpider.Core;
using DotnetSpider.Core.Processor;
using HtmlAgilityPack;

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
                var itemMeta = new Dictionary<string, object>();
                //itemMeta.Add("Url", page.Url);

                foreach (var field in job.Fields)
                {
                    if (string.IsNullOrEmpty(field.FieldValue))
                    {
                        itemMeta.Add(field.FieldName, page.Selectable.XPath(field.XPath).GetValue());
                    }
                    else
                    {
                        itemMeta.Add(field.FieldName, field.FieldValue);
                    }
                }

                if (this.job.CheckCacheMetas.ContainsKey(page.Url))
                {
                    foreach (var meta in this.job.CheckCacheMetas[page.Url])
                    {
                        itemMeta.Add(meta.Key, meta.Value);
                    }
                }
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
                    var selectedResults = page.Selectable.XPath(meta.XPath);
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



                var itemsHtml = page.Selectable.XPath(this.job.ResultItemXPath).GetValues();
                var itemDocument = new HtmlDocument();
                var hasCachedPage = false;

                foreach (var html in itemsHtml)
                {
                    itemDocument.LoadHtml(html);
                    var item = itemDocument.DocumentNode;

                    var itemMeta = pageMeta.ToDictionary(v => v.Key, v => v.Value);
                    var checkCacheMeta = pageCacheMeta.ToDictionary(v => v.Key, v => v.Value);
                    value = null;
                    foreach (var meta in this.job.Metas.Where(m => m.XPathFrom != "Page"))
                    {
                        switch (meta.FieldType)
                        {
                            case "Const":
                                value = meta.FieldValue;
                                break;
                            case "Array":
                                // need to enhance
                                value = new string[] { item.SelectSingleNode(meta.XPath).Attributes[meta.Attribute].Value ?? string.Empty };
                                break;
                            default:
                                var node = item.SelectSingleNode(meta.XPath);
                                value = null != meta.Attribute && null != node.Attributes[meta.Attribute] ? node.Attributes[meta.Attribute].Value : node.InnerText;
                                value = !string.IsNullOrEmpty(meta.Regex) ? Regex.Match(value as string, meta.Regex).Value : value;
                                break;
                        }

                        itemMeta.Add(meta.FieldName, value);
                        if (meta.CheckCache)
                        {
                            checkCacheMeta.Add(meta.FieldName, value);
                        }
                    }

                    // if page is not match result item regex, ignore
                    if (!PageIsResultItem(html))
                    {
                        continue;
                    }

                    // make operation to field
                    foreach (var operation in this.job.Operations)
                    {
                        var opObject = Operations.OperationFactory.MakeOperatoin(operation.OperationName);
                        var opValue = opObject.Operate(itemMeta[operation.FieldName]);
                        itemMeta[operation.FieldName] = opValue;
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
                            page.AddTargetRequest(itemMeta["Url"] as string);
                        }
                    }
                }

                // means all item in this page is crawled, so need to check next page
                // !hasCachedPage means this page contains cached item, so the rest pages is crawled already.
                // items need to > 0 because a page not exist may return 0 records

                if (null != this.job.PageIteration && !hasCachedPage && itemsHtml.Count() > 0)
                {
                    var nextPageUrl = this.job.PageIteration.GetNextPage();
                    if (!string.IsNullOrEmpty(nextPageUrl))
                    {
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

        private bool PageIsResultItem(string url)
        {
            return Regex.IsMatch(url, this.job.IsItemPageCheckRegex);
        }

        // public bool PageIsListPage(Page page)
        // {
        //     return Regex.IsMatch(page.Url, this.job.IsListPageCheckRegex);
        // }
    }
}