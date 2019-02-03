using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Supperxin.Web.Webcrawler.PageIterations
{
    public class PiMetaValue : IPageIteration
    {
        private int _pageCount;

        // note that this only work when the input item list is decsending by id(order), the last item is the page index.
        public string GetNextPage(string pageFormat, params object[] param)
        {
            if (null == param || param.Length < 3 || !(param[0] is int) || !(param[2] is Dictionary<string, object>))
                return null;

            var maxPage = (int)param[0];
            var metadata = param[2] as Dictionary<string, object>;

            if (this._pageCount >= maxPage)
                return null;

            var pageUrl = pageFormat;
            var validUrl = false;

            foreach (Match match in Regex.Matches(pageFormat, "{(\\S+?)}"))
            {
                var fieldName = match.Groups[1].Value;
                if (metadata.ContainsKey(fieldName))
                {
                    pageUrl = pageUrl.Replace(match.Value, metadata[fieldName] as string);
                    validUrl = true;
                }
            }

            this._pageCount++;

            return validUrl ? pageUrl : null;
        }

    }
}
