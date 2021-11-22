using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.Pipelines.HttpRequest;
using System.Web.Hosting;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomFileResolver : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (Context.Page.FilePath.Length > 0)
                return;
            string str = args.Url.FilePath;
            if (string.IsNullOrEmpty(str))
                str = "/";
            if (HostingEnvironment.VirtualPathProvider.DirectoryExists(str))
            {
                string withaspx = FileUtil.MakePath(str, "default.aspx");
                if (string.CompareOrdinal(withaspx, "/default.aspx") == 0)
                    return;
                string withindex = FileUtil.MakePath(str, "index.html");
                if (!HostingEnvironment.VirtualPathProvider.FileExists(withaspx) && !HostingEnvironment.VirtualPathProvider.FileExists(withindex))
                    return;
                if (HostingEnvironment.VirtualPathProvider.FileExists(withaspx))
                    str = withaspx;
                else
                    str = withindex;
            }
            else if (string.CompareOrdinal(str, "/default.aspx") == 0 || !HostingEnvironment.VirtualPathProvider.FileExists(str))
            {
                return;
            }
            Tracer.Info((object)("Using virtual file \"" + str + "\" instead of Sitecore layout."));
            Context.Page.FilePath = str;
        }
    }
}