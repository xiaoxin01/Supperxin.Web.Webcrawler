namespace Supperxin.Web.Webcrawler.ValueContainers
{
    public interface IValueGetter
    {
        T GetValue<T>(Meta meta) where T : class;
    }
}
