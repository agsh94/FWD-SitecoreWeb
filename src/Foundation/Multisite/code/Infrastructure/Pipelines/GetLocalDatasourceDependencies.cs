/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Collections.Generic;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Pipelines.GetDependencies;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using FWD.Foundation.Multisite.Extensions;
using System.Diagnostics.CodeAnalysis;
#endregion

namespace FWD.Foundation.Multisite.Infrastructure.Pipelines
{
    [ExcludeFromCodeCoverage]
    public class GetLocalDatasourceDependencies : BaseProcessor
    {
        public override void Process(GetDependenciesArgs args)
        {
            Assert.IsNotNull(args?.Dependencies, Constants.DependenciesNull);
            Item item = args?.IndexedItem as SitecoreIndexableItem;
            if (item == null)
                return;

            if (item.IsLocalDatasourceItem())
                AddLocalDatasourceParentDependency(item, args?.Dependencies);
        }

        private void AddLocalDatasourceParentDependency(Item item, ICollection<IIndexableUniqueId> dependencies)
        {
            var localDatasourceFolder = item.GetParentLocalDatasourceFolder();
            if (localDatasourceFolder?.Parent == null)
                return;
            dependencies.Add((SitecoreItemUniqueId)localDatasourceFolder.Parent.Uri);
        }
    }
}