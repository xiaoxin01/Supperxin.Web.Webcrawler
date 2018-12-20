using System;
using System.Collections.Generic;

namespace Supperxin.Web.Webcrawler
{
    public class CrawlJob
    {
        public CrawlJob()
        {
            this.SaveResultsTo = "results/";
            this.CheckCacheMetas = new Dictionary<string, Dictionary<string, object>>();
        }
        public string JobName { get; set; }
        public string[] StartUrls { get; set; }
        public string IsListPageCheckRegex { get; set; }
        public string IsItemPageCheckRegex { get; set; }
        public int ListPageStart { get; set; }
        public string ResultItemXPath { get; set; }
        public string ResultItemRegex { get; set; }
        public string SaveResultsTo { get; set; }
        public bool Enabled { get; set; }
        public List<Meta> Metas { get; set; }
        public List<Operation> Operations { get; set; }
        public PageIteration PageIteration { get; set; }
        public List<FieldMapping> Fields { get; set; }
        public Dictionary<string, Dictionary<string, object>> CheckCacheMetas { get; set; }
        public bool CheckCacheMetasChanged { get; set; }
        public bool AddResultItemDirectly { get; set; }
        private Dictionary<string, string> _saveTo;
        public Dictionary<string, string> SaveTo
        {
            get { return this._saveTo; }
            set
            {
                this._saveTo = value;
                if (!this._saveTo.ContainsKey("Type"))
                {
                    return;
                }
                try
                {
                    var type = this.GetType().Assembly.GetType($"{this.GetType().Assembly.GetName().Name}.HttpPipline");
                    this._pipeline = System.Activator.CreateInstance(type, this, this._saveTo) as DotnetSpider.Core.Pipeline.BasePipeline;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    this._pipeline = new DotnetSpider.Core.Pipeline.JsonFilePipeline(GetSavePath());
                }
            }
        }

        private string GetSavePath()
        {
            return System.IO.Path.Combine(this.SaveResultsTo, this.JobName);
        }

        private DotnetSpider.Core.Pipeline.BasePipeline _pipeline;
        public DotnetSpider.Core.Pipeline.BasePipeline Pipeline
        {
            get
            {
                if (_pipeline == null)
                {
                    this._pipeline = new DotnetSpider.Core.Pipeline.JsonFilePipeline(GetSavePath());
                }

                return this._pipeline;
            }
        }
        public HashSet<string> StandardFields { get; set; }
        public string UserAgent { get; set; }
    }

    public class PageIteration
    {
        private PageIterations.IPageIteration _pageIteration;
        public string IterationName { get; set; }
        public string PageFormat { get; set; }
        public int MaxPage { get; set; }
        public string GetNextPage()
        {
            if (null == this._pageIteration)
            {
                this._pageIteration = PageIterations.PageIterationFactory.MakePageIteration(this.IterationName);
            }

            return this._pageIteration.GetNextPage(this.PageFormat, this.MaxPage);
        }
    }
}