/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Collections;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;
using Sitecore.Xml.Xsl;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    [ExcludeFromCodeCoverage]
    public class GetAdvanceImageFieldValue
    {
        protected string TitleFieldName = "title";

        public virtual void Process(RenderFieldArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (!this.IsImage(args))
                return;
            ImageRenderer renderer = this.CreateRenderer();
            this.ConfigureRenderer(args, renderer);
            this.SetRenderFieldResult(renderer.Render(), args);
        }

        protected virtual bool IsImage(RenderFieldArgs args)
        {
            return args.FieldTypeKey == "advance image";
        }

        protected virtual void SetRenderFieldResult(RenderFieldResult result, RenderFieldArgs args)
        {
            args.Result.FirstPart = result.FirstPart;
            args.Result.LastPart = result.LastPart;
            args.WebEditParameters.AddRange((SafeDictionary<string, string>)args.Parameters);
            args.DisableWebEditContentEditing = true;
            args.DisableWebEditFieldWrapping = true;
            args.WebEditClick = "return Sitecore.WebEdit.editControl($JavascriptParameters, 'webedit:chooseimage')";
        }

        protected virtual void ConfigureRenderer(RenderFieldArgs args, ImageRenderer imageRenderer)
        {
            Item itemToRender = args.Item;
            imageRenderer.Item = itemToRender;
            imageRenderer.FieldName = args.FieldName;
            imageRenderer.FieldValue = args.FieldValue;
            imageRenderer.Parameters = args.Parameters;
            if (itemToRender == null)
                return;
            imageRenderer.Parameters.Add("la", itemToRender.Language.Name);
            this.EnsureMediaItemTitle(args, itemToRender, imageRenderer);
        }

        protected virtual void EnsureMediaItemTitle(
          RenderFieldArgs args,
          Item itemToRender,
          ImageRenderer imageRenderer)
        {
            if (!string.IsNullOrEmpty(args.Parameters[this.TitleFieldName]))
                return;
            Item innerImageItem = this.GetInnerImageItem(args, itemToRender);
            if (innerImageItem == null)
                return;
            Field field = innerImageItem.Fields[this.TitleFieldName];
            if (field == null)
                return;
            string str = field.Value;
            if (string.IsNullOrEmpty(str) || imageRenderer.Parameters == null)
                return;
            imageRenderer.Parameters.Add(this.TitleFieldName, str);
        }

        protected virtual Item GetInnerImageItem(RenderFieldArgs args, Item itemToRender)
        {
            Field field = itemToRender.Fields[args.FieldName];
            if (field == null)
                return (Item)null;
            return new ImageField(field, args.FieldValue).MediaItem;
        }

        protected virtual ImageRenderer CreateRenderer()
        {
            return new ImageRenderer();
        }
    }
}