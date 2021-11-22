using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using System.Collections.Specialized;
using Sitecore.Data.Items;
using Sitecore.Text;
using Sitecore.Globalization;

namespace FWD.Foundation.CDNPurgeUtility.Commands
{
    public class PurgeUtility : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, nameof(context));
            if (context.Items.Length != 1)
                return;
            Item obj = context.Items[0];
            Context.ClientPage.Start((object)this, "Run", new NameValueCollection()
            {
                [Constants.ID] = obj.ID.ToString(),
                [Constants.SubItems] = context.Parameters[Constants.SubItems],
            });
        }
        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (!args.HasResult)
            {
                string text = string.Empty;
                bool withsubitems = args.Parameters[Constants.SubItems] == "1";
                if (withsubitems)
                {
                    text = Translate.Text(Constants.ConfirmPurgeWithSubItems);
                }
                else
                {
                    text = Translate.Text(Constants.ConfirmPurgeWithoutSubItems);
                }
                SheerResponse.Confirm(text);
                args.WaitForPostBack();
            }
            else
            {
                if (args.Result != "yes")
                {
                    return;
                }
                UrlString urlString = new UrlString(UIUtil.GetUri(Constants.XMLControl));
                urlString.Append(Constants.SubItems, args.Parameters[Constants.SubItems]);
                urlString.Append(Constants.ID, args.Parameters[Constants.ID]);
                SheerResponse.ShowModalDialog(urlString.ToString(), "650px", "400px");
            }
        }
    }
}