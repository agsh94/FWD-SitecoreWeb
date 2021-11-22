using Sitecore.Diagnostics;
using Sitecore.Pipelines.PreprocessRequest;
using System.Net;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.PreprocessRequest
{
    public class RegexForbiddenHandler : PreprocessRequestProcessor
    {
        public override void Process(PreprocessRequestArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            char[] regex = Sitecore.Configuration.Settings.GetAppSetting(GlobalConstants.MatchRegexURLs).ToCharArray();
            int isMatched = args.HttpContext.Request.Path.IndexOfAny(regex);
            if (isMatched >= 0)
            {
                HttpResponseBase responseBase = args.HttpContext.Response;
                responseBase.TrySkipIisCustomErrors = true;
                responseBase.StatusCode = (int)HttpStatusCode.Forbidden;
                responseBase.Write($"{GlobalConstants.DangerousRequestDetected}{args.HttpContext.Request.Path}");
                responseBase.End();
            }
        }
    }
}
