namespace Supperxin.Web.Webcrawler.ValueContainers
{
    public interface IValueGetterFactory
    {
        IValueGetter CreateValueGetter(string type, string content);
    }
}
