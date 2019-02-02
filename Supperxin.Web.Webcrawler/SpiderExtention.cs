using DotnetSpider.Core.Selector;

namespace Supperxin.Web.Webcrawler
{
    public static class SpiderExtention
    {
        public static ISelectable Selector(this ISelectable selectable, string selector, string type = "XPath")
        {
            ISelectable selectableReturn;
            switch (type.ToLower())
            {
                case "json":
                    selectableReturn = selectable.JsonPath(selector);
                    break;
                default:
                    selectableReturn = selectable.XPath(selector);
                    break;
            }

            return selectableReturn;
        }
    }
}
