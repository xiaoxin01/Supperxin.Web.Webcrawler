using System;

namespace Supperxin.Web.Webcrawler.PageIterations
{
    public class PiUnixTimestamp : IPageIteration
    {
        private DateTime _timeIdentity;
        private bool _initialed;
        private int _pageCount;

        public string GetNextPage(string pageFormat, params object[] param)
        {
            if (null == param || param.Length < 1 || !(param[0] is int))
                return null;

            var maxPage = (int)param[0];

            InitialPage(maxPage);

            if (this._pageCount >= maxPage)
                return null;
            // more info about this function: http://tool.chinaz.com/Tools/unixtime.aspx
            var epoch = (_timeIdentity.AddDays(-_pageCount++).ToUniversalTime().Ticks - 621355968000000000) / 10000000;

            return string.Format(pageFormat, epoch);
        }

        private void InitialPage(int maxPage)
        {
            if (this._initialed)
            {
                return;
            }

            this._initialed = true;
            this._timeIdentity = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1);
        }
    }
}