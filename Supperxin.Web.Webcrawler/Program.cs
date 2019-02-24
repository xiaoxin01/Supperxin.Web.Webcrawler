using System;
using System.Collections.Generic;
using System.IO;
using DotnetSpider.Core;
using DotnetSpider.Core.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Supperxin.Web.Webcrawler
{
    class Program
    {
        public static ServiceProvider ServiceProvider;
        static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

            ServiceProvider = new ServiceCollection()
                .AddSingleton<ValueContainers.IValueGetterFactory, ValueContainers.ValueGetterFactory>()
                .AddSingleton<PageIterations.IPageIterationFactory, PageIterations.PageIterationFactory>()
                .AddSingleton<Operations.IOperationFactory, Operations.OperationFactory>()
                .BuildServiceProvider();

            IConfigurationRoot configuration = builder.Build();

            var crawlJobs = new List<CrawlJob>();
            configuration.GetSection("CrawlJobs").Bind(crawlJobs);

            if (!Directory.Exists("state"))
            {
                Directory.CreateDirectory("state");
            }

            //var crawlJob = InitialExampleCrawlJob();
            foreach (var crawlJob in crawlJobs.FindAll(j => j.Enabled))
            {
                var cacheFileName = $"state/{crawlJob.JobName}.json";
                if (File.Exists(cacheFileName))
                {
                    crawlJob.CheckCacheMetas = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(File.ReadAllText(cacheFileName));
                }
                var site = new Site { CycleRetryTimes = 0, SleepTime = 300, UserAgent = crawlJob.UserAgent };
                var spider = Spider.Create(site, new GeneralProcessor(crawlJob))
                    .AddStartUrl(crawlJob.StartUrls)
                    .AddPipeline(crawlJob.Pipeline);
                spider.Downloader = new DotnetSpider.Core.Downloader.HttpClientDownloader(60);
                spider.ThreadNum = 1;
                spider.SkipTargetUrlsWhenResultIsEmpty = false;
                spider.Run();
                if (crawlJob.CheckCacheMetasChanged)
                {
                    File.WriteAllText(cacheFileName, JsonConvert.SerializeObject(crawlJob.CheckCacheMetas));
                }
            }
            //Console.Read();
        }

        private static CrawlJob InitialExampleCrawlJob()
        {
            var crawlJob = new CrawlJob()
            {
                JobName = "Test Crawl Job",
                StartUrls = new string[] { "http://www.china-hydrogen.org/hydrogen/" },
                ResultItemRegex = "\\.html",
                Fields = new System.Collections.Generic.List<FieldMapping>()
            };

            crawlJob.Fields.Add(new FieldMapping()
            {
                FieldName = "Title",
                XPath = "/html/body/div[2]/div/div[1]/div[2]/div[1]/h1"
            });
            crawlJob.Fields.Add(new FieldMapping()
            {
                FieldName = "Time",
                XPath = "/html/body/div[2]/div/div[1]/div[2]/div[1]/h5/span[1]"
            });
            crawlJob.Fields.Add(new FieldMapping()
            {
                FieldName = "Content",
                XPath = "/html/body/div[2]/div/div[1]/div[2]/div[2]"
            });

            return crawlJob;
        }
    }
}
