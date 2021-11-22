/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;
using Sitecore.Shell.Applications.WebEdit;
using Sitecore.Text;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FWD.Foundation.SitecoreExtensions.Models
{
    [Serializable]
    public class CustomFieldEditorOptions : PageEditFieldEditorOptions
    {
        public CustomFieldEditorOptions(NameValueCollection form, IEnumerable<FieldDescriptor> fields) : base(form, fields)
        {
            
        }
        protected override UrlString GetUrl()
        {
            return new UrlString("/sitecore/shell/applications/customfieldeditor.aspx?mo=mini");
        }
        public new UrlString ToUrlString()
        {
            UrlString url = this.GetUrl();
            this.ToUrlHandle().Add(url);
            return url;
        }
    }
}