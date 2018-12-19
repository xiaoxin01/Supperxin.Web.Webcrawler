using System;

namespace Supperxin.Web.Webcrawler.PageIterations
{
    public class PiUnixTimestamp : IPageIteration
    {
        private DateTime _timeIdentity;
        private bool _initialed;

        public string GetNextPage(string pageFormat, int startPage)
        {
            InitialPage(startPage);
            // more info about this function: http://tool.chinaz.com/Tools/unixtime.aspx
            var epoch = (_timeIdentity.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            _timeIdentity = _timeIdentity.AddDays(1);

            return string.Format(pageFormat, epoch);
        }

        private void InitialPage(int startPage)
        {
            if (this._initialed)
            {
                return;
            }

            this._initialed = true;
            this._timeIdentity = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-startPage);
        }
    }
}