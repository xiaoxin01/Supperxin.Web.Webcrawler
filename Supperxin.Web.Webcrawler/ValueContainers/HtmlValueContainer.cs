namespace Supperxin.Web.Webcrawler.ValueContainers
{
    public class HtmlValueContainer : IValueGetter
    {
        private readonly string _htmlString;

        public HtmlValueContainer(string htmlString)
        {
            _htmlString = htmlString;
        }
        public T GetValue<T>(Meta meta) where T : class
        {
            object value = null;
            // switch (meta.FieldType)
            // {
            //     case "Const":
            //         value = meta.FieldValue;
            //         break;
            //     case "Array":
            //         // need to enhance
            //         value = valueContainer.GetValue<string[]>(meta);
            //         value = new string[] { item.SelectSingleNode(meta.XPath).Attributes[meta.Attribute].Value ?? string.Empty };
            //         break;
            //     default:
            //         var node = item.SelectSingleNode(meta.XPath);
            //         value = null != meta.Attribute && null != node.Attributes[meta.Attribute] ? node.Attributes[meta.Attribute].Value : node.InnerText;
            //         value = !string.IsNullOrEmpty(meta.Regex) ? Regex.Match(value as string, meta.Regex).Value : value;
            //         break;
            // }

            return value as T;
        }
    }
}
