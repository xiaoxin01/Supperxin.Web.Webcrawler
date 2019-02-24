namespace Supperxin.Web.Webcrawler.Operations
{
    public class OpRegexReplace : IOperation
    {
        public object Operate(object value, params object[] paras)
        {
            if (!(value is string) || null == paras || paras.Length != 2 || !(paras[0] is string) || !(paras[1] is string))
            {
                return value;
            }

            return System.Text.RegularExpressions.Regex.Replace((value as string), (string)paras[0], (string)paras[1]);
        }
    }
}
