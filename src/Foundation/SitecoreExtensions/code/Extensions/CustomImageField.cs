/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Helpers;
using System;

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    public class CustomImageField : Sitecore.Shell.Applications.ContentEditor.Image
    {
        /// <summary>
        /// The ItemID proprety is set by the Content Editor via reflection
        /// </summary> 
        public string ItemID { get; set; }

        /// <summary>
        /// Override the OnPreRender method. 
        /// The base OnPreRender method assigns a value to the Source viewstate property and we need to overwrite it.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Source = GetSource();
        }

        protected virtual string GetSource()
        {
            //retrieve and return the computed source value if it has already been set
            var contextSource = GetViewStateString("ContextSource");
            if (!string.IsNullOrWhiteSpace(contextSource))
                return contextSource;

            //retrieve the context item (the item containing the image field)
            var contextItem = ItemID != null ? Sitecore.Context.ContentDatabase.GetItem(ItemID) : null;
            if (contextItem == null)
                return string.Empty;

            //determine the source to be used by the media browser          
            string siteMediaFolder = StringHelper.GetSiteMediaFolder(this.ItemID);
            string mediaDatasource = StringHelper.GetMediaDataSource(AdvanceImageConstants.ImageFolder, this.Source);
            if (!string.IsNullOrEmpty(mediaDatasource))
            {
                var item = Sitecore.Context.ContentDatabase.GetItem(mediaDatasource);
                if (item != null)
                {
                    contextSource = item?.Paths.FullPath;
                }
                else if (Sitecore.Data.ID.IsID(mediaDatasource))
                {
                    mediaDatasource = string.Empty;
                }
            }
            if (string.IsNullOrEmpty(contextSource))
            {
                contextSource = string.Format("{0}/{1}/{2}", AdvanceImageConstants.MediaLibraryNodePath, CustomMediaLinkProviderConstants.MediaSiteFolder, siteMediaFolder);

                var contextWithMediaDatasource = string.Format("{0}/{1}", contextSource, mediaDatasource);
                var mediaItem = Sitecore.Context.ContentDatabase.GetItem(contextWithMediaDatasource);
                if (mediaItem != null)
                {
                    contextSource = contextWithMediaDatasource;
                }
            }


            //store the computed source value in view bag for later retrieval
            SetViewStateString("ContextSource", contextSource);

            //return the computed source value
            return contextSource;
        }
        //#endregion
    }

}