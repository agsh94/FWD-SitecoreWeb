/*9fbef606107a605d69c0edbcd8029e5d*/
namespace FWD.Foundation.DataUploader
{
    public static class Dictionary
    {
        public static readonly string TemplateNotSelectedErrorMsg = "Template is not selected on the mapped Template {0}";
        public static readonly string MappingOfFieldToColumn = "Make Sure Mapping of Field to colums are correct";
        public static readonly string ItemCreatedSuccessfully = "Item Created Successfully!";
        public static readonly string ItemCreatedSuccessfullWithPartialData = "Item Created Successfully! with partial data";
        public static readonly string ErrorEditingItem = "Error Editing Item <br /> Error Message {0}";
        public static readonly string NameColumnEmptyErrorMsg = "Name Column is empty for row {0} <br />";
        public static readonly string ErrorCreateingItemMsg = "Error Creating Item Error Message=> {0} <br/> Stack Trace => {1}";
        public static readonly string FileNotFoundErrorMsg = "File Not Found {0}";
        public static readonly string GoogleAPIEnableButKeyItemNotFoundError = "Google API Integration is Enabled but not API Key Item found under Settings from Template {0}";
        public static readonly string GoogleAPIEnableButKeyItemButKeyEmptyError = "Google API Key is Empty or have whitespace in item <a href='/sitecore/shell/Applications/Content Editor.aspx?la={0}&fo={1}&sc_content={2}'>{3}</a>";
        public static readonly string ErrorEditingItemForExcel = "Error Editing Item <br /> of row {0} Error Message{1}";
        public static readonly string InvalidOrMissingData = "Invalid or Missing Data on row {0} \n Error Message:{1}";
        public static readonly string MergedRowError = "Row Number {0} have cells merged at Column {1}, please seperate these cells, item cannot be created from these row";
        public static readonly string FaildToReceiveFromGAPI = "Failed to receive Lat Long from google for Item {0}";
        public static readonly string ErrorSetingLatLong = "Error to set Lat Long for Item {0} ErrorMessage: {1} API Error: {2}";
        public static readonly string NoSettingItemFound = "No Setting Item Found with Id {0}";
        public static readonly string SelectLangErrorMsg = "Please Select any language";
        public static readonly string FileTypeErrorMsg = "Only Excel file is allowed";
        public static readonly string DataUploadedSuccessfully = "Data Uploaded successfully";
        public static readonly string LinkWithLangEnAndMasterDbDefault = "/sitecore/shell/Applications/Content Editor.aspx?la=en&fo={0}&sc_content=master";
        public static readonly string ErrorWhileUploadingData = "Error Uploading Data";
        public static readonly string MediaLinkFormat = "<image mediaid=\"{0}\" />";
        public static readonly string FeatureDisable = "Data Upload Utility Feature is Disabled";
        public static readonly string EmptyWorksheet = "Excel Sheet is Empty";
        public static readonly string TemplateMappingIssue = "There is issue with Template Mapping Setting.";
        public static readonly string SheetNotFoundForLanguage = "Sheet not found for language: {0}";
        public static readonly string TemplateValidation = "Selected Template(<strong>{0}</strong>)have Incorrect Data Location Mapping for the Fields:<br>";
        public static readonly string TemplateFieldValidation = "Field:{0}, DataLocationFolder: {1}";
        public static readonly string RecorsMessage = "<br>Records created: {0} out of {1}";
        public static readonly string PropertyMissing = "Google API result does not have property {0} <br> for Item {1}";
        public static readonly string ErrorSetting = "Error Setting Data Received From Google";
        public static readonly string ErrorGettingLatLongForProvince = "Error Getting Lat log for Province {0} Error Message {1} :";
        public static readonly string RowsNotEqual = "Files does not have equal number of row please make it equal for each selected Language";
        public static readonly string DefaultLangSheetNotFound = "Sheet Not Found in the file from the Default Selected Language.";
        public static readonly string SomethingWentWrong = "Something went wrong";

        public static readonly string LableMarketSite = "Market Site:";
        public static readonly string LableLanguage = "Language:";
        public static readonly string LableType = "Type:";
        public static readonly string LableParentFolder = "Parent Folder:";
        public static readonly string LableTemplate = "Template:";
        public static readonly string LableSelectFile = "Select a File:";
        public static readonly string LableDefaultLanguage = "Select Default Language:";
        public static readonly string LableCreateSubFolder = "Create Sub Folder:";
        public static readonly string BtnUploadText = "Upload";
        public static readonly string BtnGOTOText = "Go To CMS";
        public static readonly string BtnExport = "Export Log";
        public static readonly string Alphabets = "Alphabets:";
        
    }
}