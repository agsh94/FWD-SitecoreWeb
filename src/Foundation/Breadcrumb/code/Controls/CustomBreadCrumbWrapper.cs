using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak;
using Sitecore.ExperienceEditor.Speak.Ribbon.Controls.BreadCrumb;
using Sitecore.Globalization;
using Sitecore.Mvc.Presentation;
using Sitecore.Security.Accounts;
using Sitecore.Web;
using System.Web;

namespace FWD.Foundation.CustomBreadcrumb.Controls
{
    public class CustomBreadCrumbWrapper : Breadcrumb
    {
        public CustomBreadCrumbWrapper()
        {
            base.InitializeControl();
        }

        public CustomBreadCrumbWrapper(RenderingParametersResolver parametersResolver)
          : base(parametersResolver)
        {
            Assert.ArgumentNotNull((object)parametersResolver, nameof(parametersResolver));
            base.InitializeControl();
        }
        protected override void PreRender()
        {
            using (new UserSwitcher(ContextUtil.ResolveUser()))
            {
                string language = WebUtil.ExtractUrlParm("sc_lang", HttpContext.Current.Request.Url.PathAndQuery);
                Item obj;
                using (new LanguageSwitcher(language))
                {
                    obj = this.RibbonDatabase.GetItem(WebUtil.GetQueryString("itemid"));
                    this.Attributes[(object)"data-sc-itemid"] = obj.ID.ToString();
                    this.Attributes[(object)"data-sc-dic-go"] = Translate.Text("Go");
                    this.Attributes[(object)"data-sc-dic-edit"] = Translate.Text("Edit");
                    this.Attributes[(object)"data-sc-dic-edit-tooltip"] = Translate.Text("Edit this page in the Content Editor.");
                    this.Attributes[(object)"data-sc-dic-treeview-tooltip"] = Translate.Text("Display the website in a tree structure.");
                    this.Attributes[(object)"data-sc-structure"] = this.GetStructure(obj);
                }
            }
        }
    }
}