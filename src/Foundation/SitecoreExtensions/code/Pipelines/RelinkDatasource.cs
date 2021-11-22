/*9fbef606107a605d69c0edbcd8029e5d*/
using System;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.Pipelines.ItemProvider.AddFromTemplate;
using FWD.Foundation.SitecoreExtensions.Helpers;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class RelinkDatasource : AddFromTemplateProcessor
    {
        public override void Process(AddFromTemplateArgs args)
        {

            Assert.ArgumentNotNull(args, nameof(args));

            if (args.Destination.Database.Name != "master") return;

            var templateItem = args.Destination.Database.GetItem(args.TemplateId);

            Assert.IsNotNull(templateItem, "Template did not exist!");

            if (templateItem.TemplateID != TemplateIDs.BranchTemplate && !templateItem.Paths.FullPath.Contains(GlobalConstants.PageTemplatesItemPath)) return;

            Assert.HasAccess((args.Destination.Access.CanCreate() ? 1 : 0) != 0, "AddFromTemplate - Add access required (destination: {0}, template: {1})", args.Destination.ID, args.TemplateId);

            Item newItem = args.Destination.Database.Engines.DataEngine.AddFromTemplate(args.ItemName, args.TemplateId, args.Destination, args.NewId);

            RewriteBranchRenderingDataSources(newItem, templateItem);

            var newItemDescendants = newItem.Axes.GetDescendants();
            for (int i = 0; i < newItemDescendants.Length; i++)
            {
                RewriteBranchRenderingDataSources(newItemDescendants[i], templateItem);
            }

            args.Result = newItem;

        }
        protected virtual void RewriteBranchRenderingDataSources(Item item, BranchItem branchTemplateItem)
        {

            string branchBasePath = branchTemplateItem.InnerItem.Paths.FullPath;
            string queryPath = GlobalConstants.QueryPath;
            LayoutHelper.ApplyActionToAllRenderings(item, rendering =>
            {

                if (string.IsNullOrWhiteSpace(rendering.Datasource))
                    return RenderingActionResult.None;

                var renderingTargetItem = item.Database.GetItem(rendering.Datasource);

                if (!((renderingTargetItem != null && renderingTargetItem.Paths.FullPath.StartsWith(branchBasePath, StringComparison.OrdinalIgnoreCase)) || rendering.Datasource.StartsWith(queryPath, StringComparison.OrdinalIgnoreCase)))
                    return RenderingActionResult.None;

                var newTargetPath = string.Empty;
                Item newTargetItem = null;
                if (renderingTargetItem != null && renderingTargetItem.Paths.FullPath.StartsWith(branchBasePath, StringComparison.OrdinalIgnoreCase))
                {
                    newTargetPath = GetDatasourceItemPathWithBranchTemplate(renderingTargetItem, branchBasePath, item);
                    newTargetItem = item.Database.GetItem(newTargetPath);
                }
                else if (rendering.Datasource.StartsWith(queryPath, StringComparison.OrdinalIgnoreCase))
                {
                    newTargetItem = GetDatasourceItemPathWithQuery(item, rendering.Datasource);
                }

                if (newTargetItem == null)
                {
                    rendering.Datasource = "INVALID_BRANCH_SUBITEM_ID";
                    return RenderingActionResult.None;
                }

                rendering.Datasource = newTargetItem.ID.ToString();

                return RenderingActionResult.None;
            });
        }

        private string GetDatasourceItemPathWithBranchTemplate(Item renderingTargetItem, string branchBasePath, Item item)
        {
            var relativeRenderingPath = renderingTargetItem.Paths.FullPath.Substring(branchBasePath.Length).TrimStart('/');
            relativeRenderingPath = relativeRenderingPath.Substring(relativeRenderingPath.IndexOf('/'));
            var newTargetPath = item.Paths.FullPath + relativeRenderingPath;
            return newTargetPath;
        }
        private Item GetDatasourceItemPathWithQuery(Item item, string datasource)
        {
            datasource = datasource?.Substring("query:".Length);
            Item queryItem = item.Axes.SelectSingleItem(datasource);
            return queryItem;
        }
    }
}