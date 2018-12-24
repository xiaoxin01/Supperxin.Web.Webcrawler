namespace Supperxin.Web.Webcrawler.PageIterations
{
    public interface IPageIteration
    {
        string GetNextPage(string pageFormat, params object[] param);
    }
}