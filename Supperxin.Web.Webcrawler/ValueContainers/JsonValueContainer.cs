using Newtonsoft.Json.Linq;

namespace Supperxin.Web.Webcrawler.ValueContainers
{
    public class JsonValueGetter : IValueGetter
    {
        private readonly string _jsonString;
        private readonly JObject _jObject;

        public JsonValueGetter(string jsonString)
        {
            _jsonString = jsonString;
            _jObject = JObject.Parse(jsonString);
        }
        public T GetValue<T>(Meta meta) where T : class
        {
            object value;
            switch (meta.FieldType)
            {
                case "Const":
                    value = meta.FieldValue;
                    break;
                default:
                    var node = _jObject.SelectToken(meta.XPath);
                    value = null != node ? node.Value<string>() : null;
                    break;
            }

            return value as T;
        }
    }
}
