/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Models;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.WebEdit;
using Sitecore.Shell.Applications.WebEdit.Commands;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class ExperienceEditor : FieldEditorCommand
    {
        protected override PageEditFieldEditorOptions GetOptions(ClientPipelineArgs args, NameValueCollection form)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            Assert.ArgumentNotNull((object)form, nameof(form));
            List<FieldDescriptor> fieldDescriptorList = new List<FieldDescriptor>();
            Item obj1 = Database.GetItem(ItemUri.Parse(args.Parameters["uri"]));
            Assert.IsNotNull((object)obj1, "item");
            string parameter1 = args.Parameters["fields"];
            Assert.IsNotNullOrEmpty(parameter1, "Field Editor command expects 'fields' parameter");
            string parameter2 = args.Parameters["command"];
            Assert.IsNotNullOrEmpty(parameter2, "Field Editor command expects 'command' parameter");
            Item obj2 = Client.CoreDatabase.GetItem(parameter2);
            Assert.IsNotNull((object)obj2, "command item");

            obj1.Fields.ReadAll();

            string finalparameter = "";
            StringBuilder bld = new StringBuilder();
            var customfields = obj1.Fields.Where(x => !x.Name.StartsWith("__", System.StringComparison.Ordinal)).OrderBy(x => x.SectionSortorder).ThenBy(x => x.Sortorder);

            foreach (var field_group in customfields)
            {
                bld.Append(field_group.Name + "|");
            }
            finalparameter = bld.ToString();

            foreach (string fieldName in new ListString(finalparameter.Remove(finalparameter.Length - 1)))
            {
                if (obj1.Fields[fieldName] != null)
                    fieldDescriptorList.Add(new FieldDescriptor(obj1, fieldName));
            }
            CustomFieldEditorOptions fieldEditorOptions = new CustomFieldEditorOptions(form, (IEnumerable<FieldDescriptor>)fieldDescriptorList);
            fieldEditorOptions.Title = obj2["Title"];
            fieldEditorOptions.Icon = obj2["Icon"];
            fieldEditorOptions.PreserveSections = true;
            return fieldEditorOptions;
        }
        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, nameof(context));
            return context.Items.Length != 0 && context.Items[0] != null && !context.Items[0].Access.CanWrite() ? CommandState.Disabled : CommandState.Enabled;
        }
    }
}