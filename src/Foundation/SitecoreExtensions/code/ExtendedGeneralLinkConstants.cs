/*9fbef606107a605d69c0edbcd8029e5d*/
namespace FWD.Foundation.SitecoreExtensions
{
    public struct GeneralLinkFieldAttributes
    {
        public const string Anchor = "anchor";
        public const string Class = "class";
        public const string Height = "height";
        public const string Href = "href";
        public const string Id = "id";
        public const string Language = "la";
        public const string Link = "link";
        public const string LinkType = "linktype";
        public const string QueryStringKey = "ro";
        public const string QueryString = "querystring";
        public const string ScContent = "sc_content";
        public const string Target = "target";
        public const string Text = "text";
        public const string Title = "title";
        public const string Url = "url";
        public const string Value = "value";
        public const string Width = "width";
    }

    public struct SectionContentFields
    {
        public const string Title = "title";
        public const string SubTitle = "subTitle";
        public const string Description = "description";
        public const string Link = "link";
    }

    public struct GeneralLinkTypes
    {
        public const string Anchor = "anchor";
        public const string External = "external";
        public const string Internal = "internal";
        public const string Javascript = "javascript";
        public const string MailTo = "mailto";
        public const string Media = "media";
        public const string ModelPopup = "modelpopup";
        public const string Form = "form";
    }

    public struct HandleMessageConstants
    {
        public const string Clear = "contentlink:clear";
        public const string ContentLinkAnchor = "contentlink:anchorlink";
        public const string ContentLinkExternalLink = "contentlink:externallink";
        public const string Follow = "contentlink:follow";
        public const string ContentLinkInternalLink = "contentlink:internallink";
        public const string ContentLinkJavascript = "contentlink:javascript";
        public const string ContentLinkMailTo = "contentlink:mailto";
        public const string ContentLinkMedia = "contentlink:media";
        public const string ContentLinkModelPopup = "contentlink:modelpopup";
        public const string ContentLinkForm = "contentlink:form";
    }

    public struct PathsandUrls
    {
        public const string AnchorLink = "/sitecore/shell/Applications/Dialogs/Anchor link.aspx";
        public const string ExternalLink = "/sitecore/shell/Applications/Dialogs/External link.aspx";
        public const string InternalLink = "/sitecore/shell/Applications/Dialogs/Internal link.aspx";
        public const string JavascriptLink = "/sitecore/shell/Applications/Dialogs/Javascript link.aspx";
        public const string MailToLink = "/sitecore/shell/Applications/Dialogs/Mail link.aspx";
        public const string MediaLink = "/sitecore/shell/Applications/Dialogs/Media link.aspx";
        public const string ModelPopupLink = "/sitecore/shell/Applications/Dialogs/Model Popup Link.aspx";
        public const string FormLink = "/sitecore/shell/Applications/Dialogs/Form Link.aspx";

        public const string ContentNodePath = "/sitecore/content";
        public const string MediaLibraryNodePath = "/sitecore/media library";
    }

    public struct ExtendedGeneralLinkConstants
    {
        public const string Value = "value";
        public const string ModelPopupContent = "modelpopupcontent";        
        public const string FormContent = "formcontent";
        public const string TemplateListToResolveAllFields = "";

        public const string DisclosurePopupContent = "disclosurepopupcontent";
        public const string DisclosurePopupProperty = "disclosure"; 
        public const string DisclsoreTemplateId = "{94797418-FFA8-4BB5-8BCD-697BEB8EF1CD}";
    }
}