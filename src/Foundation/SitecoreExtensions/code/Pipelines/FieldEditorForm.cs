/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Validators;
using Sitecore.Diagnostics;
using Sitecore.Shell;
using Sitecore.Shell.Applications.ContentManager;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomFieldEditorForm : FieldEditorForm
    {
        private bool SectionToggling { get; set; } = false;
        protected override void OnPreRendered(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, nameof(e));
            this.UpdateEditor();
            Context.ClientPage.Modified = false;
        }
        /// <summary>Updates the editor.</summary>
        private void UpdateEditor()
        {
            if (Context.ClientPage.IsEvent && !this.SectionToggling)
                return;
            if (this.SectionToggling)
                this.FieldInfo.Clear();
            Border parent = new Border();
            this.ContentEditor.Controls.Clear();
            parent.ID = "Editors";
            Context.ClientPage.AddControl((System.Web.UI.Control)this.ContentEditor, (System.Web.UI.Control)parent);
            this.RenderEditor(parent);
            this.UpdateValidatorBar(parent);
        }
        private void RenderEditor(Border parent)
        {
            Assert.ArgumentNotNull((object)parent, nameof(parent));
            Assert.IsNotNull((object)this.Options, "Editor options");
            CustomFieldEditor fieldEditor1 = new CustomFieldEditor();
            fieldEditor1.DefaultIcon = this.Options.Icon;
            fieldEditor1.DefaultTitle = this.Options.Title;
            fieldEditor1.PreserveSections = this.Options.PreserveSections;
            fieldEditor1.ShowInputBoxes = this.Options.ShowInputBoxes;
            fieldEditor1.ShowSections = this.Options.ShowSections;
            CustomFieldEditor fieldEditor2 = fieldEditor1;
            if (!Context.ClientPage.IsEvent)
            {
                if (!string.IsNullOrEmpty(this.Options.Title))
                    this.DialogTitle.Text = this.Options.Title;
                if (!string.IsNullOrEmpty(this.Options.Text))
                    this.DialogText.Text = this.Options.Text;
                if (!string.IsNullOrEmpty(this.Options.Icon))
                    this.DialogIcon.Src = this.Options.Icon;
            }
            fieldEditor2.Render((IEnumerable<FieldDescriptor>)this.Options.Fields, this.FieldInfo, (System.Web.UI.Control)parent);
            if (!Context.ClientPage.IsEvent)
                return;
            SheerResponse.SetInnerHtml("ContentEditor", (System.Web.UI.Control)parent);
        }
        private void UpdateValidatorBar(Border parent)
        {
            Assert.ArgumentNotNull((object)parent, nameof(parent));
            if (!UserOptions.ContentEditor.ShowValidatorBar)
                return;
            Sitecore.Data.Validators.ValidatorCollection validators = this.BuildValidators(ValidatorsMode.ValidatorBar);
            ValidatorManager.Validate(validators, new ValidatorOptions(false));
            string text = ValidatorBarFormatter.RenderValidationResult(validators);
            bool flag = text.IndexOf("Applications/16x16/bullet_square_grey.png", StringComparison.InvariantCulture) >= 0;
            System.Web.UI.Control control = parent.FindControl("ValidatorPanel");
            if (control == null)
                return;
            control.Controls.Add((System.Web.UI.Control)new LiteralControl(text));
            Context.ClientPage.FindControl("ContentEditorForm").Controls.Add((System.Web.UI.Control)new LiteralControl(string.Format("<input type=\"hidden\" id=\"scHasValidators\" name=\"scHasValidators\" value=\"{0}\"/>", validators.Count > 0 ? (object)"1" : (object)string.Empty)));
            if (flag)
                control.Controls.Add((System.Web.UI.Control)new LiteralControl(string.Format("<script type=\"text/javascript\" language=\"javascript\">window.setTimeout('scContent.updateValidators()', {0})</script>", (object)Settings.Validators.UpdateFrequency)));
            control.Controls.Add((System.Web.UI.Control)new LiteralControl("<script type=\"text/javascript\" language=\"javascript\">scContent.updateFieldMarkers()</script>"));
        }
    }
}