/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetRenderingDatasource;
using Sitecore.Shell.Applications.Dialogs;
using FWD.Foundation.Multisite.Infrastructure.Dialogs;

namespace FWD.Foundation.Multisite.Infrastructure.Pipelines
{
    public class GetCustomRenderingDialogUrl
    {
        public void Process(GetRenderingDatasourceArgs args)
        {
            Assert.IsNotNull((object)args, nameof(args));
            CustomSelectDatasourceOptions datasourceOptions1 = new CustomSelectDatasourceOptions();
            datasourceOptions1.DatasourceItemDefaultName = args.RenderingItem.DisplayName;
            datasourceOptions1.ContentLanguage = args.ContentLanguage;
            datasourceOptions1.CurrentDatasource = args.CurrentDatasource;
            datasourceOptions1.IncludeTemplatesForSelection = args.TemplatesForSelection;
            datasourceOptions1.Parameters = args.ContextItemPath;
            datasourceOptions1.CurrentRenderingItem = args.RenderingItem;
            SelectDatasourceOptions datasourceOptions2 = datasourceOptions1;
            if (args.Prototype != null)
                datasourceOptions2.DatasourcePrototype = args.Prototype;
            args.DialogUrl = datasourceOptions2.ToUrlString(args.ContentDatabase).ToString();
        }
    }
}