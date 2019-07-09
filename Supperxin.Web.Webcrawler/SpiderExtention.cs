using DotnetSpider.Core.Selector;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace Supperxin.Web.Webcrawler
{
    public static class SpiderExtention
    {
        public static IEnumerable<string> Selector(this ISelectable selectable, string selector, string type = "XPath", string content = "")
        {
            ISelectable selectableReturn = null;
            switch (type)
            {
                case "Json":
                    selectableReturn = selectable.JsonPath(selector);
                    break;
                case "Xml":
                    HtmlNode.ElementsFlags.Remove("link");
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(content);
                    return (from node in htmlDoc.DocumentNode.SelectNodes(selector)
                            select node.OuterHtml).ToArray();
                default:
                    selectableReturn = selectable.XPath(selector);
                    break;
            }

            return selectableReturn.GetValues();
        }
    }
}
