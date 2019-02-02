namespace Supperxin.Web.Webcrawler.Operations
{
    public class OpAppendStringBegin : IOperation
    {
        public object Operate(object value, params object[] paras)
        {
            if (!(value is string) || null == paras || paras.Length != 1 || !(paras[0] is string))
            {
                return value;
            }

            return (string)paras[0] + (value as string);
        }
    }
}
