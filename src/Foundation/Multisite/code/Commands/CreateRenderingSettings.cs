/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using FWD.Foundation.Multisite.Providers;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;

#endregion

namespace FWD.Foundation.Multisite.Commands
{
    [ExcludeFromCodeCoverage]
    public class CreateRenderingSettings : Command
  {
      private const string DatasourceLocationFieldName = RenderingSettings.DatasourceLocationFieldName;

      public override void Execute(CommandContext context)
    {
      var parameters = new NameValueCollection();
      var parentId = context?.Parameters[RenderingSettings.ParentId];
      if (string.IsNullOrEmpty(parentId))
      {
        var item = context?.Items[0];
        parentId = item?.ID.ToString();
      }
      parameters.Add(RenderingSettings.Item, parentId);
            Context.ClientPage.Start(this, "Run", parameters);
    }

      public void Run(ClientPipelineArgs args)
    {
      if (args!=null && !args.IsPostBack)
      {
        ShowDatasourceSettingsDialog();
        args.WaitForPostBack();
      }
      else
      {
        if (args != null && !args.HasResult)
            return;
          var itemId = ID.Parse(args?.Parameters[RenderingSettings.Item]);
        CreateDatasourceConfigurationItem(itemId, args?.Result);
      }
    }

      private static void CreateDatasourceConfigurationItem(ID contextItemId, string renderingItemId)
    {
      var contextItem = Context.ContentDatabase.GetItem(contextItemId);
      if (contextItem == null)
          return;

        var renderingItem = Context.ContentDatabase.GetItem(renderingItemId);
      if (renderingItem == null)
          return;

        var datasourceConfigurationName = GetDatasourceConfigurationName(renderingItem);

      contextItem.Add(datasourceConfigurationName, new TemplateID(DatasourceConfiguration.Id));
    }

      private static string GetDatasourceConfigurationName(Item renderingItem)
    {
      var datasourceLocationValue = renderingItem[DatasourceLocationFieldName];
      var datasourceConfigurationName = DatasourceConfigurationService.GetSiteDatasourceConfigurationName(datasourceLocationValue);
      if (string.IsNullOrEmpty(datasourceConfigurationName))
          datasourceConfigurationName = renderingItem.Name;
        return datasourceConfigurationName;
    }

      private static void ShowDatasourceSettingsDialog()
    {
      var urlString = new UrlString(Context.Site.XmlControlPage)
                      {
                        ["xmlcontrol"] = RenderingSettings.DatasourceSettings
                      };
      var dialogOptions = new ModalDialogOptions(urlString.ToString())
                          {
                            Response = true
                          };
      SheerResponse.ShowModalDialog(dialogOptions);
    }
  }
}