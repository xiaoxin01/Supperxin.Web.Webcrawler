namespace Supperxin.Web.Webcrawler.PageIterations
{
    public interface IPageIterationFactory
    {
        IPageIteration MakePageIteration(string iterationName);
    }
}
