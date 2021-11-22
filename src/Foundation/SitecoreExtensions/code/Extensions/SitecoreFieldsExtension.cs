/*9fbef606107a605d69c0edbcd8029e5d*/


namespace Sitecore.Sharedsource.Data.Fields
{
    using SC = Sitecore;

    public static class Field
    {
        public static bool IsStandardTempalteField(
          this SC.Data.Fields.Field field)
        {
            if (field == null)
                return false;

            SC.Data.Templates.Template template = SC.Data.Managers.TemplateManager.GetTemplate(
              SC.Configuration.Settings.DefaultBaseTemplate,
              field.Database);

            SC.Diagnostics.Assert.IsNotNull(template, "template");
            return template.ContainsField(field.ID);
        }
    }
}