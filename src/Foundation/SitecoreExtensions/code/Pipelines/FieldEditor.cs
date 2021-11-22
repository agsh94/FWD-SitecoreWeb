/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Pipelines.GetContentEditorWarnings;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.ContentEditor.Pipelines.GetContentEditorFields;
using Sitecore.Shell.Applications.ContentEditor.Pipelines.RenderContentEditor;
using Sitecore.Shell.Applications.ContentManager;
using Sitecore.Web.UI.HtmlControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomFieldEditor : Editor
    {
        /// <summary>
        /// </summary>
        private string _defaultIcon = string.Empty;
        /// <summary>
        /// </summary>
        private string _defaultTitle = string.Empty;
        /// <summary>
        /// </summary>
        private IEnumerable<FieldDescriptor> _fields;

        /// <summary>Gets or sets the default icon.</summary>
        /// <value>The default icon.</value>
        public string DefaultIcon
        {
            get
            {
                return this._defaultIcon;
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this._defaultIcon = value;
            }
        }

        /// <summary>Gets or sets the default title.</summary>
        /// <value>The default title.</value>
        public string DefaultTitle
        {
            get
            {
                return this._defaultTitle;
            }
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this._defaultTitle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [preserve sections].
        /// </summary>
        /// <value><c>true</c> if [preserve sections]; otherwise, <c>false</c>.</value>
        public bool PreserveSections { get; set; }

        /// <summary>Gets the fields.</summary>
        /// <value>The fields.</value>
        protected IEnumerable<FieldDescriptor> FieldUris
        {
            get
            {
                return this._fields;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Shell.Applications.ContentManager.FieldEditor" /> class.
        /// </summary>
        public CustomFieldEditor()
        {
            this.RenderTabsAndBars = false;
        }

        /// <summary>Renders the specified fields.</summary>
        /// <param name="fields">The fields.</param>
        /// <param name="fieldInfo">The fieldinfo.</param>
        /// <param name="parent">The parent.</param>
        public void Render(IEnumerable<FieldDescriptor> fields, Hashtable fieldInfo, System.Web.UI.Control parent)
        {
            this.Render(fields, fieldInfo, parent, false);
        }

        /// <summary>Renders the specified fields.</summary>
        /// <param name="fields">The fields.</param>
        /// <param name="fieldInfo">The fieldinfo.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="readOnly">if set to <c>true</c> [read only].</param>
        public void Render(
          IEnumerable<FieldDescriptor> fields,
          Hashtable fieldInfo,
          System.Web.UI.Control parent,
          bool readOnly)
        {
            Assert.ArgumentNotNull((object)fields, nameof(fields));
            Assert.ArgumentNotNull((object)parent, nameof(parent));
            Assert.ArgumentNotNull((object)fieldInfo, nameof(fieldInfo));
            Editor.Sections editorSections = this.GetEditorSections(fields, fieldInfo);
            this.Render(new RenderContentEditorArgs()
            {
                EditorFormatter = this.GetEditorFormatter(),
                Parent = parent,
                Sections = editorSections,
                ReadOnly = readOnly,
                Language = this.Language,
                IsAdministrator = this.IsAdministrator
            }, parent);
        }

        /// <summary>Renders the editor.</summary>
        /// <param name="root">The root.</param>
        /// <param name="item">The item.</param>
        /// <param name="fieldInfo">The field info.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="showEditor">if set to <c>true</c> this instance is show editor.</param>
        /// <exception cref="T:System.InvalidOperationException">
        /// Not supported in field editor
        /// </exception>
        public override void Render(
          Item item,
          Item root,
          Hashtable fieldInfo,
          System.Web.UI.Control parent,
          bool showEditor)
        {
            throw new InvalidOperationException("Not supported in field editor");
        }

        /// <summary>Returnes all section in the field editor.</summary>
        /// <returns>All section in the field editor.</returns>
        public Editor.Sections GetEditorSections(
          IEnumerable<FieldDescriptor> fields,
          Hashtable fieldInfo)
        {
            Assert.ArgumentNotNull((object)fields, nameof(fields));
            Assert.ArgumentNotNull((object)fieldInfo, nameof(fieldInfo));
            this._fields = fields;
            this.FieldInfo = fieldInfo;
            return this.GetSections();
        }

        /// <summary>Gets the editor formatter.</summary>
        /// <returns>The editor formatter.</returns>
        protected override EditorFormatter GetEditorFormatter()
        {
            EditorFormatter editorFormatter = base.GetEditorFormatter();
            editorFormatter.IsFieldEditor = true;
            return editorFormatter;
        }

        /// <summary>Gets the sections.</summary>
        /// <returns>The sections.</returns>
        protected override Editor.Sections GetSections()
        {
            Editor.Sections sections = new Editor.Sections();
            GetContentEditorFieldsArgs editorFieldsArgs = new GetContentEditorFieldsArgs(this.FieldUris)
            {
                DefaultIcon = this.DefaultIcon,
                DefaultTitle = this.DefaultTitle,
                FieldInfo = this.FieldInfo,
                PreserveSections = this.PreserveSections,
                Sections = sections,
                ShowDataFieldsOnly = this.ShowDataFieldsOnly
            };
            using (new LongRunningOperationWatcher(Settings.Profiling.RenderFieldThreshold, "getContentEditorFields pipeline[custom field set]", Array.Empty<string>()))
                CorePipeline.Run("getContentEditorFields", (PipelineArgs)editorFieldsArgs);

            Editor.Sections sections1 = new Editor.Sections();

            var count = 0;
            string keyName = "/Current_User/Content Editor/Sections/Collapsed";
            Registry.SetString(keyName, "");
            
            
            foreach (var section in sections)
            {
                StringBuilder bld = new StringBuilder();
                if (count == 0)
                {
                    section.CollapsedByDefault = false;
                }
                else
                {
                    section.CollapsedByDefault = true;
                }
                bld.Append(section.Name + " custom");
                section.Name = bld.ToString();
                sections1.Add(section);
                count++;
            }
            return sections1;
        }

        /// <summary>Gets the warnings.</summary>
        /// <param name="hasSections">
        /// if set to <c>true</c> [has sections].
        /// </param>
        /// <returns>
        /// </returns>
        protected override GetContentEditorWarningsArgs GetWarnings(
          bool hasSections)
        {
            return GetContentEditorWarningsArgs.Empty;
        }
    }
}