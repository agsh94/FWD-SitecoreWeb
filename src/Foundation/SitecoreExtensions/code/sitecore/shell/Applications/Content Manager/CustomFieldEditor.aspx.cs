/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Diagnostics;
using Sitecore.Reflection;
using System;

namespace FWD.Foundation.SitecoreExtensions.sitecore.shell.Applications.Content_Manager
{
    public partial class CustomFieldEditor : Sitecore.Shell.Web.UI.SecurePage
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, nameof(e));
            base.OnPreRender(e);
            if (this.CodeBeside == null)
                return;
            ReflectionUtil.CallMethod(this.CodeBeside, "OnPreRendered", true, true, new object[1]
            {
        (object) e
            });
        }

        /// <summary>Handles the Load event of the Page control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull((object)e, nameof(e));
        }
    }
}