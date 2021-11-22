/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Text.RegularExpressions;
using System.Globalization;

#endregion

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    public static class StringExtensions
  {
      public static string Humanize(this string input)
    {
      return Regex.Replace(input, "(\\B[A-Z])", " $1");
    }

      public static string ToCssLinkValue(this string link)
    {
            //return string.IsNullOrWhiteSpace(url) ? "none" : $"url('{url}')";
            return string.IsNullOrWhiteSpace(link) ? "none" : string.Format(CultureInfo.InvariantCulture,"{0}{1}{2}","url('", link, "')");
        }
  }
}