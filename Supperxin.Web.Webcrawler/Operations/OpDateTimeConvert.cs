using System;

namespace Supperxin.Web.Webcrawler.Operations
{
    public class OpDateTimeConvert : IOperation
    {
        public object Operate(object value, params object[] paras)
        {
            if (!(value is string) || null == paras || paras.Length != 1 || !(paras[0] is string))
            {
                return value;
            }

            string result = null;
            switch ((string)paras[0])
            {
                case "UnixEpochTimestamp":
                    result = ParseUnixEpochTimestamp(value);
                    break;
            }

            return result ?? value;
        }

        private string ParseUnixEpochTimestamp(object value)
        {
            string result = null;
            try
            {
                var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddMilliseconds(Convert.ToDouble(value)).ToLocalTime();
                result = dtDateTime.ToString();
                return result;
            }
            catch
            {
                return result;
            }
        }
    }
}
