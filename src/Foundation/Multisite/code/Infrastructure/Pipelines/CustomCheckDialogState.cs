/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetRenderingDatasource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWD.Foundation.Multisite.Infrastructure.Pipelines
{
    public class CustomCheckDialogState
    {
        public void Process(GetRenderingDatasourceArgs args)
        {
            Assert.IsNotNull((object)args, nameof(args));
            string str = args.RenderingItem["data source"];
            bool flag = !string.IsNullOrEmpty(str);
            if (string.IsNullOrEmpty(args.CurrentDatasource) && !this.IsCurrentRenderingContextItem(args))
                args.CurrentDatasource = str;
            string path = args.RenderingItem["Datasource Template"];
            if ((!flag || args.ShowDialogIfDatasourceSetOnRenderingItem) && (!string.IsNullOrEmpty(path) || args.FallbackDatasourceRoots.Count != 0))
                return;
            args.AbortPipeline();
        }

        /// <summary>
        /// Determines whether [is context item rendering] [the specified args].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// 	<c>true</c> if [is context item rendering] [the specified args]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCurrentRenderingContextItem(GetRenderingDatasourceArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            return !string.IsNullOrEmpty(args.ContextItemPath) && string.Equals(args.RenderingItem.Paths.FullPath, args.ContextItemPath, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}