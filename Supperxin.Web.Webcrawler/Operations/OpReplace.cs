namespace Supperxin.Web.Webcrawler.Operations
{
    public class OpReplace : IOperation
    {
        public object Operate(object value, params object[] paras)
        {
            if (!(value is string) || null == paras || paras.Length != 2 || !(paras[0] is string) || !(paras[1] is string))
            {
                return value;
            }

            return (value as string).Replace((string)paras[0], (string)paras[1]);
        }
    }
}
