/*9fbef606107a605d69c0edbcd8029e5d*/
#region
using System;
using System.Diagnostics.CodeAnalysis;
using FWD.Foundation.Multisite.Providers;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Sitecore.Web.UI.XmlControls;
#endregion

namespace FWD.Foundation.Multisite.Dialogs
{
    [ExcludeFromCodeCoverage]
    public class DatasourceSettingsPage : DialogForm
    {
        private const string DialogRootSettingName = DataSourceSettings.DialogRootSettingName;

        public XmlControl Dialog { get; set; }
        protected Border Items { get; set; }

        protected TreeviewEx Treeview { get; set; }

        protected DataContext DataContext { get; set; }


        protected string Root => Sitecore.Configuration.Settings.GetSetting(DialogRootSettingName, DataSourceSettings.RenderingPath);

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, nameof(e));
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;
            if (DataContext != null)
            {
                DataContext.GetFromQueryString();
                DataContext.Root = Root;
                DataContext.Filter = FetchFilter();
            }
        }

        protected void OkClick()
        {
            var selectionItem = Treeview.GetSelectionItem();
            if (selectionItem == null)
            {
                SheerResponse.Alert("Select an item.");
            }
            else
            {
                SetDialogResult(selectionItem);
                SheerResponse.CloseWindow();
            }
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull(args, nameof(args));
            OkClick();
        }

        protected virtual void SetDialogResult(Item selectedItem)
        {
            Assert.ArgumentNotNull(selectedItem, nameof(selectedItem));
            SheerResponse.SetDialogValue(selectedItem?.ID.ToString());
        }

        protected string FetchFilter()
        {
            return "(contains(@@templatekey, 'folder') or contains(@Datasource Location, '" + DatasourceConfigurationService.SiteDatasourcePrefix + "'))";
        }
    }
}