using System.Text.RegularExpressions;

namespace Supperxin.Web.Webcrawler.ValueContainers
{
    public class HtmlValueContainer : IValueGetter
    {
        private readonly string _htmlString;
        private readonly HtmlAgilityPack.HtmlDocument _htmlDocument;

        public HtmlValueContainer(string htmlString)
        {
            _htmlString = htmlString;
            _htmlDocument = new HtmlAgilityPack.HtmlDocument();
            _htmlDocument.LoadHtml(htmlString);
        }
        public T GetValue<T>(Meta meta) where T : class
        {
            object value = null;
            switch (meta.FieldType)
            {
                case "Const":
                    value = meta.FieldValue;
                    break;
                case "Array":
                    // need to enhance
                    value = new string[] { _htmlDocument.DocumentNode.SelectSingleNode(meta.XPath).Attributes[meta.Attribute].Value ?? string.Empty };
                    break;
                default:
                    var node = _htmlDocument.DocumentNode.SelectSingleNode(meta.XPath);
                    value = null != meta.Attribute && null != node.Attributes[meta.Attribute] ? node.Attributes[meta.Attribute].Value : node.InnerText;
                    value = !string.IsNullOrEmpty(meta.Regex) ? Regex.Match(value as string, meta.Regex).Value : value;
                    break;
            }

            return value as T;
        }
    }
}
