namespace Supperxin.Web.Webcrawler.ValueContainers
{
    public class ValueGetterFactory : IValueGetterFactory
    {
        public IValueGetter CreateValueGetter(string type, string content)
        {
            IValueGetter valueGetter;
            switch (type.ToLower())
            {
                case "json":
                    valueGetter = new JsonValueGetter(content);
                    break;
                default: // html
                    valueGetter = new HtmlValueContainer(content);
                    break;
            }

            return valueGetter;
        }
    }
}
