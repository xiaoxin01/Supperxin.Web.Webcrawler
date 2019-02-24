using System.Collections.Generic;

namespace Supperxin.Web.Webcrawler.Operations
{
    public class OperationFactory : IOperationFactory
    {
        private Dictionary<string, IOperation> _operationCache = new Dictionary<string, IOperation>();
        public IOperation MakeOperation(string operationName)
        {
            var assembly = typeof(OperationFactory).Assembly;
            var classFullName = $"{typeof(OperationFactory).Namespace}.{operationName}";

            if (_operationCache.ContainsKey(classFullName))
            {
                return _operationCache[classFullName];
            }

            var type = assembly.GetType(classFullName);
            var operation = System.Activator.CreateInstance(type) as IOperation;
            _operationCache.Add(classFullName, operation);

            return operation;
        }
    }
}
