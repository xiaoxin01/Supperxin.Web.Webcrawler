namespace Supperxin.Web.Webcrawler.Operations
{
    public class OpHtmlDecode : IOperation
    {
        public object Operate(object value)
        {
            if (!(value is string))
            {
                return value;
            }

            return System.Net.WebUtility.HtmlDecode(value as string);
        }
    }
}