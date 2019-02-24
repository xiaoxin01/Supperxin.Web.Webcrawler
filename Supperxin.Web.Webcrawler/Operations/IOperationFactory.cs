namespace Supperxin.Web.Webcrawler.Operations
{
    public interface IOperationFactory
    {
        IOperation MakeOperation(string operationName);
    }
}
