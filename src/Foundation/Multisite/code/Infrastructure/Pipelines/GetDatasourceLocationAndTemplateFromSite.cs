/*9fbef606107a605d69c0edbcd8029e5d*/
#region
using FWD.Foundation.Multisite.Providers;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetRenderingDatasource;
#endregion

namespace FWD.Foundation.Multisite.Infrastructure.Pipelines
{
    public class GetDatasourceLocationAndTemplateFromSite
    {
        //Sitecore has no constant in FieldIDs for this standard field
        private readonly IDatasourceProvider _provider;

        public GetDatasourceLocationAndTemplateFromSite() : this(new DatasourceProvider())
        {
        }

        public GetDatasourceLocationAndTemplateFromSite(IDatasourceProvider provider)
        {
            _provider = provider;
        }

        public void Process(GetRenderingDatasourceArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            var datasource = args?.RenderingItem[RenderingOptionsFields.DatasourceLocation];

            var datasourceList = datasource.Split('|');

            if (datasourceList.Length > 0)
            {
                foreach (var str in datasourceList)
                {
                    if (DatasourceConfigurationService.IsSiteDatasourceLocation(str))
                    {
                        ResolveDatasource(args, str);
                        ResolveDatasourceTemplate(args, str);
                    }
                }
            }
        }

        protected virtual void ResolveDatasource(GetRenderingDatasourceArgs args, string source)
        {
            var contextItem = args?.ContentDatabase?.GetItem(args?.ContextItemPath);
            var name = DatasourceConfigurationService.GetSiteDatasourceConfigurationName(source);
            if (string.IsNullOrEmpty(name))
                return;

            var datasourceLocations = _provider.GetDatasourceLocations(contextItem, name);

            if (datasourceLocations != null)
            {
                args?.DatasourceRoots?.AddRange(datasourceLocations);
            }
        }

        protected virtual void ResolveDatasourceTemplate(GetRenderingDatasourceArgs args, string source)
        {
            var contextItem = args?.ContentDatabase.GetItem(args?.ContextItemPath);
            var name = DatasourceConfigurationService.GetSiteDatasourceConfigurationName(source);
            if (string.IsNullOrEmpty(name))
                return;
            if (args != null)
                args.Prototype = _provider?.GetDatasourceTemplate(contextItem, name);
        }
    }
}