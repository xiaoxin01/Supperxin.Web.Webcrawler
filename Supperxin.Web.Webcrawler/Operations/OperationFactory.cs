namespace Supperxin.Web.Webcrawler.Operations
{
    public class OperationFactory
    {
        public static IOperation MakeOperatoin(string operationName)
        {
            var assembly = typeof(OperationFactory).Assembly;
            var type = assembly.GetType($"{typeof(OperationFactory).Namespace}.{operationName}");

            return System.Activator.CreateInstance(type) as IOperation;
        }
    }
}