using System;
using System.Collections.Generic;
using DotnetSpider.Core;
using DotnetSpider.Core.Pipeline;

namespace Supperxin.Web.Webcrawler
{
    public class HttpPipline : BasePipeline
    {
        private readonly CrawlJob job;
        private readonly string processUrl;
        public HttpPipline(CrawlJob job, Dictionary<string, string> paras)
        {
            this.job = job;
            this.processUrl = paras.ContainsKey("ProcessUrl") ? paras["ProcessUrl"] : string.Empty;
        }
        public override void Process(IEnumerable<ResultItems> resultItems, ISpider spider)
        {
            foreach (var item in resultItems)
            {
                var isSuccess = PostService.PostData(item.Results.Values, this.processUrl);
                if (isSuccess)
                {

                }
                else
                {
                    Console.WriteLine("Post data error!");
                }
            }
        }
    }
}
