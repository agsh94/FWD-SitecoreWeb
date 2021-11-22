/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using Temp = FWD.Foundation.DataUploader.Template;
namespace FWD.Foundation.DataUploader.Models
{
    public class TemplateConfiguration
    {
        Database db = Sitecore.Configuration.Factory.GetDatabase("master");
        private readonly List<Template> templates = new List<Template>();
        public IEnumerable<Template> Templates { get { return templates; } }

        public TemplateConfiguration()
        {
            AddTemplates();
        }
        protected void AddTemplates()
        {
            Item TemplatesFolder = db.GetItem(Constant.TemplateMappingFolder);
            if (TemplatesFolder != null)
            {
                foreach (Item tempItem in TemplatesFolder.GetChildren())
                {
                    Sitecore.Data.Fields.ReferenceField reference = tempItem.Fields[Temp.DataUploadTemplate.Fields.Template];
                    Item temp = reference.TargetItem;
                    Template template = new Template();
                    template.Id = temp.ID.ToString();
                    template.Name = temp.Name;
                    template.Path = temp.Paths.Path;
                    template.IsGoogleAPIEnabled = ((CheckboxField)tempItem.Fields[Temp.ApiDetails.Fields.IsEnable]).Checked;
                    template.IsSetLatLong = ((CheckboxField)tempItem.Fields[Temp.ApiDetails.Fields.SetLatLong]).Checked;
                    template.IsSetPostalCode = ((CheckboxField)tempItem.Fields[Temp.ApiDetails.Fields.SetPostalCode]).Checked;
                    template.TemplateItem = tempItem;
                    foreach (Item child in tempItem.GetChildren())
                    {
                        string col = child[Temp.FieldMapping.Fields.Col];
                        if (!string.IsNullOrEmpty(col) || !string.IsNullOrWhiteSpace(col))
                        {
                            Sitecore.Data.Fields.ReferenceField fieldreference = child.Fields[Temp.FieldMapping.Fields.Field];
                            Item field = fieldreference.TargetItem;
                            Field f = new Field();

                            f.Id = field.ID.ToString();
                            f.Col = int.Parse(child[Temp.FieldMapping.Fields.Col]);
                            f.Name = field.Name;
                            f.Type = child[Temp.FieldMapping.Fields.Type];
                            f.MasterDataTemplateID = child[Temp.FieldMapping.Fields.MasterDataTemplate];
                            ReferenceField referenceField = child.Fields[Temp.FieldMapping.Fields.MasterDataLocation];
                            f.MasterDataFolderLocation = referenceField.TargetItem;
                            if (!referenceField.TargetID.IsNull && !string.IsNullOrEmpty(f.MasterDataTemplateID) && !string.IsNullOrWhiteSpace(f.MasterDataTemplateID))
                            {
                                SiteInfo site = GetSiteFomItem(f.MasterDataFolderLocation);
                                f.MasterDataFolderChildren = db.SelectItems(string.Format("fast:{0}//*[@@templateid='{1}']", f.MasterDataFolderLocation.Paths.FullPath.ToLower().Replace(site.Name.ToLower(), string.Format("#{0}#", site.Name.ToLower())), f.MasterDataTemplateID)).ToList();
                            }
                            f.MasterDataType = ((ReferenceField)child.Fields[Temp.FieldMapping.Fields.MasterDataType])?.TargetItem;
                            f.IsProvince = ((CheckboxField)child.Fields[Temp.FieldMapping.Fields.IsProvince]).Checked;
                            f.GetFromGoogle = ((CheckboxField)child.Fields[Temp.FieldMapping.Fields.GetFromGoogle]).Checked;
                            template.Fields.Add(f);
                        }

                    }
                    templates.Add(template);
                }

            }
        }
        private SiteInfo GetSiteFomItem(Item item)
        {
            return Sitecore.Configuration.Factory.GetSiteInfoList().Where(info => !string.IsNullOrEmpty(info.HostName)).FirstOrDefault((x => item.Paths.FullPath.StartsWith(x.RootPath, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}