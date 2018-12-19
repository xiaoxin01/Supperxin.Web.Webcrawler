namespace Supperxin.Web.Webcrawler.PageIterations
{
    public class PageIterationFactory
    {
        public static IPageIteration MakePageIteration(string iterationName)
        {
            var assembly = typeof(PageIterationFactory).Assembly;
            var type = assembly.GetType($"{typeof(PageIterationFactory).Namespace}.{iterationName}");

            return System.Activator.CreateInstance(type) as IPageIteration;
        }
    }
}