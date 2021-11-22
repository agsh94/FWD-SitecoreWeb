/*9fbef606107a605d69c0edbcd8029e5d*/
using System.Diagnostics.CodeAnalysis;
using Sitecore;
using Sitecore.Data;
using Sitecore.Mvc.Presentation;
using Sitecore.Speak.Components.Models;
using Sitecore.Web;
using System.Web;

namespace FWD.Foundation.Multisite
{
    [ExcludeFromCodeCoverage]
    public class CustomFormDesignBoardRenderingModel : ComponentRenderingModel
    {
        public string FormId { get; set; }

        public string FormMode { get; set; }

        public string DefaultPageFieldType { get; set; }

        public string PageFieldTypes { get; set; }

        public string FormsRootFolder { get; set; }

        public string PlaceholderText { get; set; }

        public string PagePlaceholderText { get; set; }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            this.FormId = this.GetIdValue("formId", "FormId");
            this.FormMode = this.GetStringValue("sc_formmode", "FormMode");
            this.DefaultPageFieldType = this.GetString("DefaultPageFieldType", string.Empty);
            this.PageFieldTypes = this.GetString("PageFieldTypes", string.Empty);
            this.PlaceholderText = this.GetString("PlaceholderText", string.Empty);
            this.PagePlaceholderText = this.GetString("PagePlaceholderText", string.Empty);
            this.FormsRootFolder = this.GetString("FormsRootFolder", string.Empty);
            var cookie = HttpContext.Current.Request.Cookies["FormRootId"];
            if (cookie != null)
            {
                var cookieValue = cookie?.Value;
                this.FormsRootFolder = cookieValue;
            }

            string empty = string.Empty;
            if (ID.IsID(this.DefaultPageFieldType))
                empty = ClientHost.Databases.ContentDatabase.GetItem(ID.Parse(this.DefaultPageFieldType))?.Fields["Field Template"]?.Value;
            this.Properties["FormId"] = (object)this.FormId;
            this.Properties["FormMode"] = (object)this.FormMode;
            this.Properties["DefaultPageFieldTemplate"] = (object)empty;
        }

        protected string GetIdValue(string queryStringKey, string modelParameterName)
        {
            string rawUrl = ClientHost.Context.Request.RawUrl;
            string urlParm = WebUtil.ExtractUrlParm(queryStringKey, rawUrl);
            if (ID.IsID(urlParm))
                return urlParm;
            string id = this.GetString(modelParameterName, string.Empty);
            if (ID.IsID(id))
                return id;
            return string.Empty;
        }

        protected string GetStringValue(string queryStringKey, string modelParameterName)
        {
            string rawUrl = ClientHost.Context.Request.RawUrl;
            string urlParm = WebUtil.ExtractUrlParm(queryStringKey, rawUrl);
            if (!string.IsNullOrWhiteSpace(urlParm))
                return urlParm;
            return this.GetString(modelParameterName, string.Empty);
        }
    }
}