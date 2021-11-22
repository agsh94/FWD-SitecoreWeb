/*9fbef606107a605d69c0edbcd8029e5d*/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using RestSharp;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;

namespace FWD.Foundation.DataUploader.Repositories
{
    public class DataUploaderRepository
    {
        Logging.CustomSitecore.Logger log = Logging.CustomSitecore.Logger.Log;
        private readonly Sitecore.Data.Database db = Sitecore.Configuration.Factory.GetDatabase("master");
        private string googleKey = "";
        ExcelWorksheets Sheet;
        string DefaultLang = "en";
        public bool UploadExcel(Stream InputStream, int dataStartRow, string langName, int ItemNameColumn, string parentNode, string parentName, Models.Template configTemplate, ref Table resultTable, out string ErrorMsg, ref Item CMSParentItem, bool createSubFolder = false, int alphabets = 2, string defaultLang = "en")
        {
            DefaultLang = defaultLang;
            bool IsgoogleAPIdataHasError = true;
            bool setMasterDatahasError = true;
            bool InvalidRow = true;
            bool newItemCreateHasError = true;
            try
            {
                DataUploader.sitecore.DataUploader.totalRecord = 0;
                DataUploader.sitecore.DataUploader.recordCreatedSuccessfully = 0;
                ErrorMsg = "";
                bool hasError = false;
                TemplateItem Templateitem = GetTemplateItem(configTemplate);
                if (Templateitem == null)
                {
                    hasError = true;
                    ErrorMsg += string.Format(Dictionary.TemplateNotSelectedErrorMsg, configTemplate.TemplateItem.Name);
                    return !hasError;
                }
                List<Models.Field> ItemFields = configTemplate.Fields;
                Item ParentItem = GetParentItem(parentNode, langName, parentName);
                CMSParentItem = ParentItem;

                Item ApiInformation = GetAPIKeyItem(ParentItem);
                if (configTemplate.IsGoogleAPIEnabled && ApiInformation == null)
                {
                    hasError = true;
                    ErrorMsg += string.Format(Dictionary.GoogleAPIEnableButKeyItemNotFoundError, Constant.ApiINformation);
                    ParentItem.Delete();
                    return !hasError;
                }
                string googleAPIKey = ApiInformation[Template.Setting.Fields.Value];
                googleKey = googleAPIKey;
                if (string.IsNullOrEmpty(googleAPIKey) || string.IsNullOrWhiteSpace(googleAPIKey))
                {
                    hasError = true;
                    ErrorMsg += string.Format(Dictionary.GoogleAPIEnableButKeyItemButKeyEmptyError, ApiInformation.Language, ApiInformation.ID.ToString(), ApiInformation.Database.Name, ApiInformation.Name);
                    ParentItem.Delete();
                    return !hasError;
                }
                using (var package = new ExcelPackage(InputStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var currentSheet = package.Workbook.Worksheets;
                    Sheet = currentSheet;
                    var workSheet = currentSheet[defaultLang];
                    if (workSheet == null)
                    {
                        hasError = true;
                        ErrorMsg += Dictionary.DefaultLangSheetNotFound;
                        ParentItem.Delete();
                        return !hasError;
                    }
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    foreach (string lang in langName.Split(','))
                    {
                        if (currentSheet[lang].Dimension == null)
                        {
                            hasError = true;
                            ErrorMsg += Dictionary.EmptyWorksheet;
                            ParentItem.Delete();
                            return !hasError;
                        }
                        if (currentSheet[lang].Dimension.End.Row != noOfRow)
                        {
                            hasError = true;
                            ErrorMsg += Dictionary.RowsNotEqual;
                            ParentItem.Delete();
                            return !hasError;
                        }

                    }
                    DataUploader.sitecore.DataUploader.totalRecord = (noOfRow - (dataStartRow - 1));
                    var maxCol = ItemFields.Select(x => x.Col).Max();
                    if (maxCol > noOfCol)
                    {
                        hasError = true;
                        ErrorMsg += Dictionary.MappingOfFieldToColumn;
                        ParentItem.Delete();
                        return !hasError;
                    }

                    for (int rowIterator = dataStartRow; rowIterator <= noOfRow; rowIterator++)
                    {
                        Item newItem = null;
                        if (HasValidateCells(ItemFields, currentSheet, rowIterator, langName, ref resultTable))
                        {
                            InvalidRow = false;
                            try
                            {
                                using (new SecurityDisabler())
                                {
                                    var itemName = workSheet.Cells[rowIterator, ItemNameColumn].Value?.ToString();

                                    if (itemName != null)
                                    {

                                        newItem = CreateLangVersionItems(langName, ParentItem, itemName, Templateitem.ID, ref newItemCreateHasError, createSubFolder, alphabets);
                                        if (newItem == null)
                                            return false;
                                        try
                                        {
                                            IRestResponse response;

                                            newItem.Editing.BeginEdit();
                                            foreach (Models.Field field in ItemFields)
                                            {
                                                AddValueToItemField(field, newItem, langName, rowIterator, currentSheet);

                                            }
                                            newItem.Editing.EndEdit();
                                            if (configTemplate.IsGoogleAPIEnabled)
                                            {
                                                response = SetLatLogFromGoogleAPI(newItem, configTemplate, googleAPIKey, ref resultTable, langName, ref IsgoogleAPIdataHasError);
                                                SetMasterDataRecievedFromGoogle(ItemFields, newItem, langName, response, ref setMasterDatahasError, ref resultTable);
                                            }




                                        }
                                        catch (Exception ex)
                                        {
                                            AddRowToTable(newItem, ref resultTable, System.Drawing.Color.Red, Dictionary.ErrorEditingItemForExcel, rowIterator.ToString(), ex.Message);
                                            hasError = true;
                                            ErrorMsg += Dictionary.SomethingWentWrong;
                                        }

                                    }
                                    else
                                    {
                                        AddExceptionRow(Dictionary.NameColumnEmptyErrorMsg, ref resultTable, rowIterator.ToString());
                                        hasError = true;

                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                hasError = true;
                                AddExceptionRow(Dictionary.InvalidOrMissingData, ref resultTable, rowIterator.ToString(), ex.Message);
                                log.Error("", ex);
                                InvalidRow = true;
                            }
                        }
                        else
                        {
                            InvalidRow = true;
                        }
                        if (!IsgoogleAPIdataHasError && !setMasterDatahasError && !newItemCreateHasError && !InvalidRow)
                        {
                            AddRowToTable(newItem, ref resultTable, System.Drawing.Color.Green, Dictionary.ItemCreatedSuccessfully, null, null);
                            DataUploader.sitecore.DataUploader.recordCreatedSuccessfully++;
                        }
                        if (!newItemCreateHasError && (IsgoogleAPIdataHasError || setMasterDatahasError))
                        {
                            AddRowToTable(newItem, ref resultTable, System.Drawing.Color.Orange, Dictionary.ItemCreatedSuccessfullWithPartialData, null, null);
                        }
                    }
                }
                return !hasError;

            }
            catch (Exception ex)
            {
                log.Error("", ex);
                ErrorMsg = Dictionary.ErrorWhileUploadingData;
                return false;
            }
        }
        public Item GetParentItem(string parentNode, string langName, string parentName)
        {

            Item ParentNodeItem = db.GetItem(new ID(parentNode));
            Item ParentItem = ParentNodeItem;

            if (string.IsNullOrEmpty(parentName) || string.IsNullOrWhiteSpace(parentName))
            {
                //assigning the selected Parent Nord as parent Item
                ParentItem = ParentNodeItem;
            }
            else
            {
                ParentItem = CreateItem(langName, ParentNodeItem, parentName, ParentItem.TemplateID);
                if (ParentItem == null)
                    ParentItem = ParentNodeItem;

            }

            return ParentItem;
        }

        private Item CreateLangVersionItems(string langName, Item ParentItem, string itemName, ID templateID, ref bool hasError, bool createAlphabetFolder = false, int alphabetCount = 2)
        {
            try
            {
                if (createAlphabetFolder)
                {
                    Item alphabetFolder;
                    string folder = "";
                    for (int i = 0; i < alphabetCount; i++)
                    {
                        folder += itemName[i].ToString();
                    }
                    alphabetFolder = ParentItem.Children.FirstOrDefault(x => x.TemplateID == ParentItem.TemplateID && x.Name == folder);
                    if (alphabetFolder == null)
                    {
                        alphabetFolder = CreateItem(langName, ParentItem, folder, ParentItem.TemplateID);
                    }
                    hasError = false;
                    return CreateItem(langName, alphabetFolder, itemName, templateID);
                }
                else
                {
                    hasError = false;
                    return CreateItem(langName, ParentItem, itemName, templateID);
                }

            }
            catch (Exception ex)
            {
                hasError = true;
                log.Error("", ex);
                return null;
            }

        }

        private Item CreateItem(string langName, Item ParentItem, string itemName, ID templateID)
        {
            try
            {
                Item newItem = null;
                int flag = 0;
                foreach (var lang in langName.Split(','))
                {

                    if (flag == 0)
                    {
                        using (new SecurityDisabler())
                        using (new LanguageSwitcher(lang))
                        {
                            ParentItem = ParentItem.Database.GetItem(ParentItem.ID);
                            //creating new folder with the selected parent Ndoe template under it and setting parentItema as the newly created item.
                            newItem = ParentItem.Add(ItemUtil.ProposeValidItemName(itemName), new TemplateID(templateID));
                        }
                        flag++;
                    }
                    else
                    {
                        using (new LanguageSwitcher(lang))
                        {
                            if (newItem != null)
                            {
                                newItem = newItem.Database.GetItem(newItem.ID);
                                newItem.Versions.AddVersion();
                            }
                        }
                    }

                }

                return newItem;
            }
            catch (Exception ex)
            {
                log.Error("Excpetion occured while creating item by using Uploader utility", ex);
                return null;
            }
        }

        private bool HasValidateCells(List<Models.Field> ItemFields, ExcelWorksheets workSheets, int rowIterator, string langs, ref Table resultTable)
        {
            try
            {
                StringBuilder ErrorMsg = new StringBuilder();
                if (ItemFields == null)
                    return false;
                bool isValid = true;
                int col = 0;
                foreach (string lang in langs.Split(','))
                {
                    ExcelWorksheet workSheet = workSheets[lang];
                    if (workSheet == null)
                    {
                        ErrorMsg.Append(string.Format(Dictionary.SheetNotFoundForLanguage, lang));
                        TableRow tr = new TableRow();
                        List<TableCell> tc = new List<TableCell>() {
                                                new TableCell(){Text = ErrorMsg.ToString()}
                                };
                        tr.ForeColor = System.Drawing.Color.Red;
                        tr.Cells.AddRange(tc.ToArray());
                        resultTable.Rows.Add(tr);
                        continue;
                    }
                    foreach (Models.Field field in ItemFields)
                    {
                        col = field.Col;
                        if (workSheet.Cells[rowIterator, field.Col].Merge)
                        {
                            isValid = false;
                        }
                    }
                }
                if (!isValid)
                {
                    TableRow tr = new TableRow();
                    List<TableCell> tc = new List<TableCell>() {
                                                new TableCell(){Text = string.Format(Dictionary.MergedRowError,rowIterator.ToString(),col.ToString())}
                                };
                    tr.ForeColor = System.Drawing.Color.Red;
                    tr.Cells.AddRange(tc.ToArray());
                    resultTable.Rows.Add(tr);
                }
                return isValid;
            }
            catch (Exception ex)
            {
                TableRow tr = new TableRow();
                List<TableCell> tc = new List<TableCell>() {
                                                new TableCell(){Text = ex.Message}
                                };
                tr.ForeColor = System.Drawing.Color.Red;
                tr.Cells.AddRange(tc.ToArray());
                resultTable.Rows.Add(tr);
                return false;
            }

        }

        private IRestResponse SetLatLogFromGoogleAPI(Item item, Models.Template configTemplate, string APIKey, ref Table resultTable, string langName, ref bool hasError)
        {
            item.Reload();
            Item configTemplateItem = configTemplate.TemplateItem;
            string ErrorMsg = "";
            try
            {
                IRestResponse response;

                StringBuilder address = new StringBuilder();
                List<Item> AddressFields = ((MultilistField)configTemplateItem.Fields[Template.ApiDetails.Fields.Address])?.GetItems()?.ToList();
                if (AddressFields == null)
                    return null;
                foreach (Item field in AddressFields)
                {
                    if (field.ID == Template.Hospital.Fields.District || field.ID == Template.Hospital.Fields.Country)
                    {
                        address.Append(GetItemName(item[field.ID]));
                    }
                    else
                    {
                        address.Append(item[field.ID]);
                    }

                }
                var client = new RestClient(Sitecore.Configuration.Settings.GetAppSetting(Constant.GoogleAPIURI));
                var request = new RestRequest(Method.GET);
                request.AddQueryParameter("address", address.ToString());
                request.AddQueryParameter("key", APIKey);
                response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject content = JsonConvert.DeserializeObject<JObject>(response.Content);
                    string status = content["status"].ToString();
                    if (status.Equals("OK"))
                    {
                        JObject location = JsonConvert.DeserializeObject<JObject>(content["results"][0]["geometry"]["location"].ToString());
                        JArray addressComponent = JsonConvert.DeserializeObject<JArray>(content["results"][0]["address_components"].ToString());

                        foreach (string lang in langName.Split(','))
                        {
                            using (new SecurityDisabler())
                            using (new LanguageSwitcher(lang))
                            {
                                item.Editing.BeginEdit();
                                if (configTemplate.IsSetLatLong)
                                {
                                    item.Fields[new ID(configTemplateItem[Template.ApiDetails.Fields.Latitude])].Value = location["lat"]?.ToString();
                                    item.Fields[new ID(configTemplateItem[Template.ApiDetails.Fields.Longitute])].Value = location["lng"]?.ToString();

                                }
                                if (configTemplate.IsSetPostalCode)
                                {
                                    //string postalCode = addressComponent.Select
                                    item.Fields[new ID(configTemplateItem[Template.ApiDetails.Fields.PostalCode])].Value = addressComponent?.Last["long_name"]?.ToString();
                                }
                                item.Editing.EndEdit();
                            }

                        }
                        hasError = false;
                    }
                    else
                    {
                        hasError = true;
                        ErrorMsg += content["error_message"].ToString();
                        throw new Exception();
                    }
                }
                else
                {
                    hasError = true;
                    AddExceptionRow(Dictionary.FaildToReceiveFromGAPI, ref resultTable, item.ID.ToString());
                }

                return response;
            }
            catch (Exception ex)
            {
                TableRow tableRow = new TableRow();
                List<TableCell> tableCells = new List<TableCell>() {
                                                new TableCell(){Text = string.Format(Dictionary.ErrorSetingLatLong,item.ID.ToString(),ex.Message,ErrorMsg)}
                                    };
                tableRow.ForeColor = System.Drawing.Color.Red;
                tableRow.Cells.AddRange(tableCells.ToArray());
                resultTable.Rows.Add(tableRow);
                log.Error("", ex);
                hasError = true;
                return null;
            }



        }
        private string GetItemName(string Id)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrWhiteSpace(Id))
            {
                return string.Empty;
            }
            Item item = db.GetItem(new ID(Id));
            if (item == null)
                return string.Empty;
            return item.Name;
        }
        private TemplateItem GetTemplateItem(Models.Template configTemplate)
        {
            TemplateItem Templateitem = null;
            if (!string.IsNullOrEmpty(configTemplate.TemplateItem[Template.DataUploadTemplate.Fields.Template]))
            {
                Templateitem = db.GetTemplate(new ID(configTemplate.TemplateItem[Template.DataUploadTemplate.Fields.Template]));
            }
            return Templateitem;
        }
        private Item GetAPIKeyItem(Item item)
        {
            Item ApiInformation = null;
            if (item == null)
                return null;
            SiteInfo site = GetSiteFomItem(item);
            if (site == null)
                return null;
            ApiInformation = db.SelectItems(string.Format("{0}//*[@@templateid='{1}']", site.RootPath.ToLower().Replace(site.Name.ToLower(), string.Format("#{0}#", site.Name.ToLower())), Constant.ApiINformation)).FirstOrDefault(x => x[Template.Setting.Fields.Name].Equals(Constant.GoogleMapAPI));
            return ApiInformation;
        }

        private void AddValueToItemField(Models.Field field, Item newItem, string langName, int row, ExcelWorksheets worksheets)
        {
            string itemID = "";
            if (field.Type.ToLower() == "masterdata" && !field.GetFromGoogle)
            {
                ExcelWorksheet workSheet = worksheets[DefaultLang];
                string value = workSheet.Cells[row, field.Col].Value?.ToString();
                itemID = GetMasterDataValue(field, value, langName, row);
            }

            foreach (string lang in langName.Split(','))
            {
                ExcelWorksheet workSheet = worksheets[lang];
                string value = workSheet.Cells[row, field.Col].Value?.ToString();

                using (new LanguageSwitcher(lang))
                {
                    newItem = newItem.Database.GetItem(newItem.ID);
                    newItem.Editing.BeginEdit();
                    switch (field.Type.ToLower())
                    {
                        case "image":
                            newItem.Fields[new ID(field.Id)].Value = string.Format(Dictionary.MediaLinkFormat, value);
                            break;
                        case "date":
                            DateTime date = DateTime.Parse(value);
                            newItem.Fields[new ID(field.Id)].Value = Sitecore.DateUtil.ToIsoDate(date);
                            break;
                        case "masterdata":
                            if (!field.GetFromGoogle)
                                newItem.Fields[new ID(field.Id)].Value = itemID;
                            break;
                        default:
                            newItem.Fields[new ID(field.Id)].Value = value;
                            break;
                    }
                    newItem.Editing.EndEdit();
                }
            }
        }
        private void AddRowToTable(Item newItem, ref Table resultTable, System.Drawing.Color color, string msg, string param1, string param2)
        {
            string status = msg;
            if (!string.IsNullOrEmpty(param1))
            {
                status = string.Format(msg, param1);
            }
            if (!string.IsNullOrEmpty(param1) && !string.IsNullOrEmpty(param2))
            {
                status = string.Format(msg, param1, param2);
            }
            TableRow tableRow = new TableRow();
            List<TableCell> tableCells = new List<TableCell>() {
                                                new TableCell(){Text = newItem.Name},
                                                new TableCell(){Text = newItem.ID.ToString()},
                                                new TableCell(){Text = status}
                                    };
            tableRow.ForeColor = color;
            tableRow.Cells.AddRange(tableCells.ToArray());
            resultTable.Rows.Add(tableRow);
        }
        private void AddExceptionRow(string msg, ref Table resultTable, string param1)
        {
            string status = msg;
            if (!string.IsNullOrEmpty(param1))
            {
                status = string.Format(msg, param1);
            }
            TableRow tableRow = new TableRow();
            List<TableCell> tableCells = new List<TableCell>() {
                                                new TableCell(){Text = status}
                                    };
            tableRow.ForeColor = System.Drawing.Color.Red;
            tableRow.Cells.AddRange(tableCells.ToArray());
            resultTable.Rows.Add(tableRow);

        }
        private void AddExceptionRow(string msg, ref Table resultTable, string param1, string param2)
        {
            string status = msg;
            if (!string.IsNullOrEmpty(param1) && !string.IsNullOrEmpty(param2))
            {
                status = string.Format(msg, param1, param2);
            }
            TableRow tableRow = new TableRow();
            List<TableCell> tableCells = new List<TableCell>() {
                                                new TableCell(){Text = status}
                                    };
            tableRow.ForeColor = System.Drawing.Color.Red;
            tableRow.Cells.AddRange(tableCells.ToArray());
            resultTable.Rows.Add(tableRow);
        }
        private string GetMasterDataValue(Models.Field field, string value, string langName, int row = 0)
        {
            
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            string[] values = value.Split(',');
            List<string> res = new List<string>();
            Item[] createdItems = new Item[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                string val = values[i].Trim();
                if (field.MasterDataFolderLocation != null)
                {
                    SiteInfo site = GetSiteFomItem(field.MasterDataFolderLocation);
                    field.MasterDataFolderChildren = db.SelectItems(string.Format("fast:{0}//*[@@templateid='{1}']", field.MasterDataFolderLocation.Paths.FullPath.ToLower().Replace(site.Name.ToLower(), string.Format("#{0}#", site.Name.ToLower())), field.MasterDataTemplateID)).ToList();
                }
                Item item = field.MasterDataFolderChildren.FirstOrDefault(x => x.Name.ToLower() == ItemUtil.ProposeValidItemName(val).ToLower());
                if (item != null)
                {
                    res.Add(item.ID.ToString());
                }
                else
                {
                    bool createsubfolder = false;
                    int alphabets = 0;

                    if (field.IsProvince)
                    {
                        int.TryParse(field.MasterDataType[Template.MasterDataSetting.Fields.Alphabets], out alphabets);
                        createsubfolder = ((CheckboxField)field.MasterDataType.Fields[Template.MasterDataSetting.Fields.CreateSubFolder]).Checked;
                        item = CreateItemAndSetLatLong(langName, field.MasterDataFolderLocation, val, new ID(field.MasterDataTemplateID), field.MasterDataType, createsubfolder, alphabets);
                        res.Add(item.ID.ToString());
                        createdItems[i] = item;
                    }
                    else if (field.GetFromGoogle)
                    {
                        createsubfolder = ((CheckboxField)field.MasterDataType.Fields[Template.MasterDataSetting.Fields.CreateSubFolder]).Checked;
                        int.TryParse(field.MasterDataType[Template.MasterDataSetting.Fields.Alphabets], out alphabets);
                        bool hasError = false;
                        item = CreateLangVersionItems(langName, field.MasterDataFolderLocation, val, new ID(field.MasterDataTemplateID), ref hasError, createsubfolder, alphabets);
                        res.Add(item.ID.ToString());
                        createdItems[i] = item;
                    }
                    else
                    {
                        if (field.MasterDataType != null)
                        {
                            createsubfolder = ((CheckboxField)field.MasterDataType.Fields[Template.MasterDataSetting.Fields.CreateSubFolder]).Checked;
                            int.TryParse(field.MasterDataType[Template.MasterDataSetting.Fields.Alphabets], out alphabets);
                        }
                        bool hasError = false;
                        item = CreateLangVersionItems(langName, field.MasterDataFolderLocation, val, new ID(field.MasterDataTemplateID), ref hasError, createsubfolder, alphabets);
                        res.Add(item.ID.ToString());
                        createdItems[i] = item;
                    }



                }
            }
            if (row > 0)
            {
                for (int i = 0; i < createdItems.Length; i++)
                {
                    Item item = createdItems[i];
                    if (item != null)
                    {
                        if (item.TemplateID == Template.TagItem.ID)
                        {
                            foreach (string lang in langName.Split(','))
                            {
                                string cellValue = Sheet[lang].Cells[row, field.Col].Value?.ToString();
                                if (!string.IsNullOrEmpty(cellValue) &&  !string.IsNullOrWhiteSpace(cellValue))
                                {
                                    string[] vals = Sheet[lang].Cells[row, field.Col].Value?.ToString().Split(',');
                                    string val = vals[i];
                                    using (new LanguageSwitcher(lang))
                                    {
                                        item = item.Database.GetItem(item.ID);
                                        item.Editing.BeginEdit();
                                        item.Fields[Template.TagItem.Fields.Value].Value = val.Trim();
                                        item.Editing.EndEdit();
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return string.Join("|", res);
        }
        private SiteInfo GetSiteFomItem(Item item)
        {
            return Sitecore.Configuration.Factory.GetSiteInfoList().Where(info => !string.IsNullOrEmpty(info.HostName)).FirstOrDefault((x => item.Paths.FullPath.StartsWith(x.RootPath, StringComparison.InvariantCultureIgnoreCase)));
        }
        private Item CreateItemAndSetLatLong(string langName, Item ParentItem, string itemName, ID templateID, Item masterDataType, bool createsubfolder = false, int alphabets = 2)
        {
            bool hasError = false;
            Item item = CreateLangVersionItems(langName, ParentItem, itemName, templateID, ref hasError, createsubfolder, alphabets);
            if (item == null)
                return null;

            var client = new RestClient(Sitecore.Configuration.Settings.GetAppSetting(Constant.GoogleAPIURI));
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("address", itemName);
            request.AddQueryParameter("key", googleKey);
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                JObject content = JsonConvert.DeserializeObject<JObject>(response.Content);
                string status = content["status"].ToString();
                if (status.Equals("OK"))
                {
                    JObject location = JsonConvert.DeserializeObject<JObject>(content["results"][0]["geometry"]["location"].ToString());
                    foreach (string lang in langName.Split(','))
                    {
                        using (new SecurityDisabler())
                        using (new LanguageSwitcher(lang))
                        {
                            item = item.Database.GetItem(item.ID);
                            item.Editing.BeginEdit();
                            item.Fields[new ID(masterDataType[Template.MasterDataSetting.Fields.Latitude])].Value = location["lat"]?.ToString();
                            item.Fields[new ID(masterDataType[Template.MasterDataSetting.Fields.Longitude])].Value = location["lng"]?.ToString();
                            item.Fields[Template.TagItem.Fields.Value].Value = new CultureInfo("en").TextInfo.ToTitleCase(itemName.ToLower());
                            item.Editing.EndEdit();
                        }

                    }
                }
                else
                {
                    log.Error(string.Format(Dictionary.ErrorGettingLatLongForProvince, item.ID, content["error_message"].ToString()));
                }
            }
            else
            {
                log.Error("Google API is Down.");
            }
            return item;
        }
        private void SetMasterDataRecievedFromGoogle(List<Models.Field> fields, Item newItem, string langName, IRestResponse response, ref bool hasError, ref Table resultTable)
        {

            try
            {
                StringBuilder res = new StringBuilder();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject content = JsonConvert.DeserializeObject<JObject>(response.Content);
                    List<Models.Address> addressComponent = JsonConvert.DeserializeObject<List<Models.Address>>(content["results"][0]["address_components"].ToString());
                    List<Models.Field> masterDatafields = fields.Where(x => x.Type.Equals("masterdata") && x.GetFromGoogle).ToList();

                    foreach (Models.Field field in masterDatafields)
                    {
                        string property = field.MasterDataType[Template.MasterDataSetting.Fields.Property];
                        Models.Address address = addressComponent.FirstOrDefault(x => x.types.Contains(property.Trim()));

                        if (address == null)
                        {
                            res.Append(string.Format(Dictionary.PropertyMissing, property, newItem.ID));
                        }
                        else
                        {
                            string val = GetMasterDataValue(field, address.long_name, langName);
                            using (new SecurityDisabler())
                            {
                                foreach (string lang in langName.Split(','))
                                {
                                    using (new LanguageSwitcher(lang))
                                    {
                                        newItem = db.GetItem(newItem.ID);
                                        newItem.Editing.BeginEdit();
                                        newItem.Fields[new ID(field.Id)].Value = val;
                                        newItem.Editing.EndEdit();
                                    }
                                }
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(res.ToString()))
                {
                    hasError = false;
                }
                else
                {
                    hasError = true;
                    TableRow tableRow = new TableRow();
                    List<TableCell> tableCells = new List<TableCell>() {
                                                new TableCell(){Text = res.ToString()}
                                    };
                    tableRow.ForeColor = System.Drawing.Color.Red;
                    tableRow.Cells.AddRange(tableCells.ToArray());
                    resultTable.Rows.Add(tableRow);
                }

            }
            catch (Exception ex)
            {
                hasError = true;
                log.Error(Dictionary.ErrorSetting, ex);
                TableRow tableRow = new TableRow();
                List<TableCell> tableCells = new List<TableCell>() {
                                                new TableCell(){Text = string.Format(Dictionary.ErrorSetting)}
                                    };
                tableRow.ForeColor = System.Drawing.Color.Red;
                tableRow.Cells.AddRange(tableCells.ToArray());
                resultTable.Rows.Add(tableRow);
            }

        }
    }
}