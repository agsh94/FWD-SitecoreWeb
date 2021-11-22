/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.DataUploader.Models;
using FWD.Foundation.DataUploader.Repositories;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Diagnostics;
using Sitecore.Configuration;
using System.Text;

namespace FWD.Foundation.DataUploader.sitecore
{
    public partial class DataUploader : System.Web.UI.Page
    {
        public static int recordCreatedSuccessfully;
        public static int totalRecord;
        Logging.CustomSitecore.Logger log = Logging.CustomSitecore.Logger.Log;
        Item CMSParentItem;
        private Sitecore.Data.Database db = Sitecore.Configuration.Factory.GetDatabase("master");

        TemplateConfigurationRepository rep = new TemplateConfigurationRepository();
        private TemplateConfiguration mappingConfig;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                bool EnableDataUploadUtility = Settings.GetBoolSetting(Constant.EnableDataUploadUtility, false);
                if (!EnableDataUploadUtility)
                {
                    Log.Info(Dictionary.FeatureDisable, this);
                    txtIsdisable.Value = (!EnableDataUploadUtility).ToString();
                    return;
                }
                lblMarketSite.Text = Dictionary.LableMarketSite;
                lblType.Text = Dictionary.LableType;
                lblLanguage.Text = Dictionary.LableLanguage;
                lblParentFolder.Text = Dictionary.LableParentFolder;
                lblTemplate.Text = Dictionary.LableTemplate;
                lblSelectFile.Text = Dictionary.LableSelectFile;
                lnkGoTO.InnerText = Dictionary.BtnGOTOText;
                btnExport.Text = Dictionary.BtnExport;
                btnValidate.Text = Dictionary.BtnUploadText;
                lblAlphabets.Text = Dictionary.Alphabets;
                lblDefaultLang.Text = Dictionary.LableDefaultLanguage;
                lblCreateSubFolder.Text = Dictionary.LableCreateSubFolder;
                if (!Page.IsPostBack)
                {
                    List<Sitecore.Web.SiteInfo> sites = Sitecore.Sites.SiteContextFactory.Sites.Where(x => !string.IsNullOrEmpty(x.HostName)).ToList();

                    SiteNode.DataTextField = "Name";
                    SiteNode.DataValueField = "Name";
                    SiteNode.DataSource = sites;
                    SiteNode.DataBind();

                    PopulateLanguage();
                    List<Item> FolderTypes = db.GetItem(Constant.FolderTypes).Children.ToList();
                    FolderType.DataTextField = "Name";
                    FolderType.DataValueField = "ID";
                    FolderType.DataSource = FolderTypes;
                    FolderType.DataBind();
                    PopulateParents();
                    PopulateTemplate();
                    mappingConfig = rep.GetConfiguration();

                    if (mappingConfig == null)
                    {
                        lblMessage.Text = Dictionary.TemplateMappingIssue;
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }
                }
                if (resultTable.Rows.Count < 2)
                {
                    resultTable.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("", ex.Message);
                log.Error("", ex);

            }
        }

        protected void btnValidate_Click(object sender, EventArgs e)
        {
            Spinner.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "block");

            try
            {
                mappingConfig = rep.GetConfiguration();
                Item DataUploadSetting = db.GetItem(Constant.DataUploadSetting);
                if (DataUploadSetting == null)
                {
                    lblMessage.Text = string.Format(Dictionary.NoSettingItemFound, Constant.DataUploadSetting.ToString());
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    Spinner.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "none");

                    return;
                }
                string parentName = txtParentName.Text;
                bool IsUploaded = false;
                string langName = String.Join(",", LanguageCheckBoxList.Items.OfType<ListItem>().Where(r => r.Selected).Select(r => r.Text));
                if (string.IsNullOrEmpty(langName))
                {
                    lblLang.Text = Dictionary.SelectLangErrorMsg;
                    lblLang.ForeColor = System.Drawing.Color.Red;
                    Spinner.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "none");

                    return;
                }
                else
                {
                    lblLang.Text = "";
                }



                Models.Template configTemplate = mappingConfig.Templates.FirstOrDefault(x => x.TemplateItem.ID.ToString() == TemplateItem.SelectedValue);
                bool err = ValidateConfigsForMasterDataLocation(configTemplate);
                if (err)
                {
                    Spinner.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "none");
                    return;
                }
                if (FileUpload.HasFile)
                {
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    Regex reg = new Regex(@"^.*\.(xlsx|xls)$");
                    if (!reg.IsMatch(fileName))
                    {
                        lblMessage.Text = Dictionary.FileTypeErrorMsg;
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        string ErrorMsg = "";

                        int ItemNameColumn;
                        string itemName = CheckEmptyOrWhitespace(DataUploadSetting[Template.DataUploadSetting.Fields.ItemNameColumn], "1");
                        int.TryParse(itemName, out ItemNameColumn);
                        bool IsFirstRowHeader;
                        string isFirstRowHeader = CheckEmptyOrWhitespace(DataUploadSetting[Template.DataUploadSetting.Fields.IsFirstRowHeader], "1");
                        bool.TryParse(isFirstRowHeader, out IsFirstRowHeader);
                        char colSeperator;
                        string colseperator = CheckEmptyOrWhitespace(DataUploadSetting[Template.DataUploadSetting.Fields.ColSeperator], ";");
                        char.TryParse(colseperator, out colSeperator);
                        DataUploaderRepository dataUploaderRepository = new DataUploaderRepository();

                        int dataStartRow;
                        string datastartrow = CheckEmptyOrWhitespace(DataUploadSetting[Template.DataUploadSetting.Fields.DataStartRow], "1");
                        int.TryParse(datastartrow, out dataStartRow);
                        resultTable.Visible = true;

                        bool createsubfolder = createSubfolder.Checked;
                        int alphabets = int.Parse(Alphabets.SelectedValue);
                        IsUploaded = dataUploaderRepository.UploadExcel(FileUpload.PostedFile.InputStream, dataStartRow, langName, ItemNameColumn, ParentNode.SelectedValue, parentName, configTemplate, ref resultTable, out ErrorMsg, ref CMSParentItem, createsubfolder, alphabets,DefaultLanguage.SelectedValue);
                        btnExport.Visible = true;
                        lnkGoTO.Attributes.Remove("href");
                        lnkGoTO.Attributes.Add("href", string.Format(Dictionary.LinkWithLangEnAndMasterDbDefault, CMSParentItem.ID.ToString()));


                        if (IsUploaded && string.IsNullOrEmpty(ErrorMsg))
                        {
                            lblMessage.Text = Dictionary.DataUploadedSuccessfully + string.Format(Dictionary.RecorsMessage, recordCreatedSuccessfully.ToString(), totalRecord.ToString());
                            lblMessage.ForeColor = System.Drawing.Color.Green;
                        }
                        else
                        {
                            lblMessage.Text = ErrorMsg;
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
                else
                {
                    lblMessage.Text = "File not selected. Please upload a file.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
                Spinner.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "none");

            }
            catch (Exception ex)
            {
                Log.Error("", ex.Message);
                log.Error("", ex);
                Spinner.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "none");
            }

        }

        protected void PopulateParents()
        {
            try
            {
                string selectedTemplateFolder = db.GetItem(new ID(FolderType.SelectedValue)).Fields[Template.DataUploaderMapping.Fields.TemplateFolder].Value;
                SiteInfo selectedSite = GetSelectedSite();
                ParentNode.DataTextField = "Name";
                ParentNode.DataValueField = "ID";
                var siteName = selectedSite.Name.ToLower();
                ParentNode.DataSource = db.SelectItems(string.Format("fast:{0}//*[@@templateid='{1}']", selectedSite.RootPath.ToLower().Replace(siteName, string.Format("#{0}#", siteName)), selectedTemplateFolder));
                ParentNode.DataBind();
                lnkGoTO.Attributes.Add("href", string.Format(Dictionary.LinkWithLangEnAndMasterDbDefault, ParentNode.SelectedValue));

            }
            catch (Exception ex)
            {
                Log.Error("", ex.Message);
                log.Error("", ex);
            }

        }
        protected void PopulateTemplate()
        {
            try
            {
                List<Item> Templates = ((MultilistField)db.GetItem(new Sitecore.Data.ID(FolderType.SelectedValue)).Fields[Template.DataUploaderMapping.Fields.SupportedChildren]).GetItems().ToList();
                TemplateItem.DataTextField = "Name";
                TemplateItem.DataValueField = "ID";
                TemplateItem.DataSource = Templates;
                TemplateItem.DataBind();
            }
            catch (Exception ex)
            {

                Log.Error("", ex.Message);
                log.Error("", ex);
            }

        }
        protected void PopulateLanguage()
        {
            try
            {
                SiteInfo selectedSite = GetSelectedSite();
                Item siteConfiguration = ((LinkField)db.GetItem(selectedSite.RootPath).Fields[Constant.SiteConfigurationLinkField])?.TargetItem;
                if (siteConfiguration != null)
                {
                    NameValueListField nameValueListField = siteConfiguration.Fields[Constant.LocalizationLanguage];
                    List<Language> languages = db.GetLanguages().Where(x => nameValueListField.NameValues.AllKeys.Contains(x.Origin.ItemId.ToString())).ToList();

                    LanguageCheckBoxList.DataTextField = "Name";
                    LanguageCheckBoxList.DataValueField = "Name";
                    LanguageCheckBoxList.DataSource = languages;
                    LanguageCheckBoxList.DataBind();

                    DefaultLanguage.DataTextField = "Name";
                    DefaultLanguage.DataValueField = "Name";
                    DefaultLanguage.DataSource = languages;
                    DefaultLanguage.DataBind();
                }
            }
            catch (Exception ex)
            {

                Log.Error("", ex.Message);
                log.Error("", ex);
            }

        }
        protected void SiteNode_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateParents();
            PopulateLanguage();
        }

        protected void TemplateFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateParents();
            PopulateTemplate();
        }

        private SiteInfo GetSelectedSite()
        {
            string selectedSiteName = SiteNode.SelectedValue;
            SiteInfo selectedSite = Sitecore.Sites.SiteContextFactory.Sites.FirstOrDefault(x => x.Name == selectedSiteName);
            return selectedSite;
        }
        private SiteInfo GetSiteFomItem(Item item)
        {
            return Sitecore.Configuration.Factory.GetSiteInfoList().Where(info => !string.IsNullOrEmpty(info.HostName)).FirstOrDefault((x => item.Paths.FullPath.StartsWith(x.RootPath, StringComparison.InvariantCultureIgnoreCase)));
        }
        private string CheckEmptyOrWhitespace(string value, string defaultValue)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                return defaultValue;
            return value;
        }

        protected void ParentNode_SelectedIndexChanged(object sender, EventArgs e)
        {
            lnkGoTO.Attributes.Add("href", string.Format(Dictionary.LinkWithLangEnAndMasterDbDefault, ParentNode.SelectedValue));
        }

        private bool ValidateConfigsForMasterDataLocation(Models.Template template)
        {
            bool hasError = false;
            SiteInfo site = GetSelectedSite();
            StringBuilder res = new StringBuilder();
            res.Append(string.Format(Dictionary.TemplateValidation, template.Name));
            List<Models.Field> fields = template.Fields.Where(x => x.Type == "masterdata").ToList();
            foreach (Models.Field field in fields)
            {
                SiteInfo datalocationsite = GetSiteFomItem(field.MasterDataFolderLocation);
                if (!datalocationsite.Name.Equals(site.Name))
                {
                    hasError = true;
                    res.Append(string.Format(Dictionary.TemplateFieldValidation, field.Name, field.MasterDataFolderLocation.Paths.Path));
                }
            }
            if (hasError)
            {
                lblMessage.Text = res.ToString();
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
            return hasError;
        }

        protected void LanguageCheckBoxList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblLang.Text = "";
        }
    }
}