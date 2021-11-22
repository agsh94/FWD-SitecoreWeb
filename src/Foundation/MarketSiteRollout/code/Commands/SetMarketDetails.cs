using FWD.Foundation.MarketSiteRollout.Constants;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Specialized;

namespace FWD.Foundation.MarketSiteRollout.Commands
{
    public class SetMarketDetails : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));
            if (context.Items.Length != 1)
                return;
            Context.ClientPage.Start((object)this, "Run", new NameValueCollection()
            {
                ["db"] = CommonConstants.MasterDatabase,
                ["lang"] = "en",
                ["ver"] = "1",
                ["cfs"] = "1",
                ["scriptId"] = CommonConstants.ScriptID.ToString(),
                ["scriptDb"] = CommonConstants.MasterDatabase
            });
        }
        protected void Run(ClientPipelineArgs args)
        {
            if (!SheerResponse.CheckModified())
                return;
            if (args.IsPostBack)
            {
                return;
            }
            UrlString urlString = new UrlString(UIUtil.GetUri("control:PowerShellRunner"));
            urlString.Append("db", args.Parameters["db"]);
            urlString.Append("lang", args.Parameters["lang"]);
            urlString.Append("ver", args.Parameters["ver"]);
            urlString.Append("cfs", args.Parameters["cfs"]);
            urlString.Append("scriptId", args.Parameters["scriptId"]);
            urlString.Append("scriptDb", args.Parameters["scriptDb"]);
            SheerResponse.ShowModalDialog(urlString.ToString(), "400px", "400px", string.Empty, true);
            args.WaitForPostBack();
        }
    }
}