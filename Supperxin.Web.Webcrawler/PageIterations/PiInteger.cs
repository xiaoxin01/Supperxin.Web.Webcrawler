using System;

namespace Supperxin.Web.Webcrawler.PageIterations
{
    public class PiInteger : IPageIteration
    {
        private bool _initialed;
        private int _maxPage;
        private int _startPage;
        private int _pageIndex;

        public string GetNextPage(string pageFormat, params object[] param)
        {
            if (null == param || param.Length < 2 || !(param[0] is int) || !(param[1] is int))
                return null;

            var maxPage = (int)param[0];
            var startPage = (int)param[1];

            InitialPage(maxPage, startPage);

            if (this._pageIndex > maxPage)
                return null;

            return string.Format(pageFormat, this._pageIndex++);
        }

        private void InitialPage(int maxPage, int startPage)
        {
            if (this._initialed)
            {
                return;
            }

            this._initialed = true;
            this._maxPage = maxPage;
            this._startPage = startPage;
            this._pageIndex = startPage;
        }
    }
}