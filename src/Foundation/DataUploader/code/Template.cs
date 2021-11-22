/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Data;

namespace FWD.Foundation.DataUploader
{
    public struct Template
    {
        public struct Hospital
        {
            public readonly static ID ID = new ID("{29D97C39-6DC9-4E94-8A9A-12154E2AD19B}");
            public struct Fields
            {
                public readonly static ID Name = new ID("{BC88B2B5-F787-423F-966D-26720D738083}");
                public readonly static ID FacilityType = new ID("{C20CCE7B-5349-432E-A656-77394CE985E2}");
                public readonly static ID City = new ID("{A88837DD-01D0-456A-B678-EF12FFFA2496}");
                public readonly static ID Longitude = new ID("{E7BD7BBC-38C3-42C8-BBEB-A84E612D75A5}");
                public readonly static ID Latitude = new ID("{7E4EF5F8-C53E-40AF-B1B0-AA8E85495DC0}");
                public readonly static ID StreetNumber = new ID("{D0A7AD35-AD85-43AE-AAF6-1DBF443C91FB}");
                public readonly static ID Road = new ID("{A8DF4643-7000-41CA-8038-46AFE2CB5494}");
                public readonly static ID District = new ID("{20969991-14D8-413F-A0B3-606DDE852FB4}");
                public readonly static ID Country = new ID("{FB6D3F20-15F7-448B-BA1B-BA2304D55DC6}");
                public readonly static ID PostalCode = new ID("{DC85978E-5C4C-404E-A4B0-1D0B35C01FE2}");
                public readonly static ID Area = new ID("{F493EE0F-A54A-4384-9F8E-F8CA6482B7AA}");
                public readonly static ID Fax = new ID("{1012CC35-3918-4CA9-8029-9002BC4071C7}");
                public readonly static ID Mobile = new ID("{0580280C-57D4-4966-A08A-0F7E874814A4}");
                public readonly static ID Telephone = new ID("{CC819FC2-63B9-4F6B-8538-24759FBD25D9}");
                public readonly static ID OpeningTiming = new ID("{E8EAE067-BF99-4E68-863B-07BEF46127EF}");
                public readonly static ID FacilityIndividualService = new ID("{D637D618-F6A0-48AE-BBB6-EF0544201C1B}");
                public readonly static ID FacilityGroupService = new ID("{46DB4D4C-69ED-42F5-A11D-D051D684ED33}");
                public readonly static ID StateBioFoundation = new ID("{E267456E-79E2-4479-938A-281EEFDCA46A}");
            }
        }
        public struct Setting
        {
            public readonly static ID ID = new ID("{81DA41F4-FD7D-4F45-82AD-44E83F47F69A}");
            public struct Fields
            {
                public readonly static ID Name = new ID("{0BB8C93B-A99D-46B3-8E18-FA27970A244C}");
                public readonly static ID Value = new ID("{B95C8194-E837-46C4-AB8A-A5C6874359CF}");

            }
        }

        public struct DataUploaderMapping
        {
            public readonly static ID ID = new ID("{3842AFAB-7DB8-4A3A-864B-1C30AB3E5BDD}");
            public struct Fields
            {
                public readonly static ID TemplateFolder = new ID("{EE0C0AF2-A6E6-41A8-A4C8-9DF682F0F9C6}");
                public readonly static ID SupportedChildren = new ID("{9D23A93F-A625-4E99-867D-F36A1DD248BC}");
            }
        }
        public struct DataUploadTemplate
        {
            public readonly static ID ID = new ID("{CA4856F3-38D4-4921-B32A-19020F1DF7E7}");
            public struct Fields
            {
                public readonly static ID Template = new ID("{C8B48196-A278-4978-84B9-E5F45D6AEF11}");

            }
        }
        public struct FieldMapping
        {
            public readonly static ID ID = new ID("{44ABA686-5AB4-4036-B96C-684F4A7CC4B9}");
            public struct Fields
            {
                public readonly static ID Field = new ID("{970FDDCD-D12A-43D4-A829-C8F1818AA3AA}");
                public readonly static ID Col = new ID("{71556ADD-818F-4B77-A284-9887F74275ED}");
                public readonly static ID Type = new ID("{78BBCFCA-44E8-4BB2-A2D4-637DA8961C44}");
                public readonly static ID MasterDataTemplate = new ID("{2189EFE7-FB40-4F6E-B5C1-E93261D92704}");
                public readonly static ID MasterDataLocation = new ID("{434BC141-7AF2-422D-91B5-DED6DF9D352E}");
                public readonly static ID MasterDataType = new ID("{C7632BBD-FDA4-49A5-893E-E8CC469E9289}");
                public readonly static ID IsProvince = new ID("{F6889F95-BFE3-48F9-9F49-BCB887C1692B}");
                public readonly static ID GetFromGoogle = new ID("{A0D09C20-47D7-4DC2-A79D-AEB7CA63A498}");

            }
        }
        public struct ApiDetails
        {
            public readonly static ID ID = new ID("{237E3A0D-9549-4859-BEF9-178DEBE22680}");
            public struct Fields
            {
                public readonly static ID IsEnable = new ID("{116E61D0-6EA4-488F-8B75-F940317EE182}");
                public readonly static ID Address = new ID("{F936F3FD-60AD-4046-BCC5-5FF1C71F63F7}");
                public readonly static ID Latitude = new ID("{A56E5752-9F67-4700-80C7-BAF2A2D7C3BF}");
                public readonly static ID Longitute = new ID("{A333AA1A-5B6D-4733-9A1F-BFA2040E904C}");
                public readonly static ID SetLatLong = new ID("{10AD2D7C-BA18-4FD8-BB79-E69808CBDF62}");
                public readonly static ID SetPostalCode = new ID("{8A0B8D13-9560-4273-A4BE-BCF49C20A2C2}");
                public readonly static ID PostalCode = new ID("{07FE1989-933F-4F3B-8177-1F4B67E87F1E}");
            }
        }
        public struct DataUploadSetting
        {
            public readonly static ID ID = new ID("{81DA41F4-FD7D-4F45-82AD-44E83F47F69A}");
            public struct Fields
            {
                public readonly static ID ItemNameColumn = new ID("{CF7AFE6D-5FF4-4AD5-8E4B-09224DD29B42}");
                public readonly static ID DataStartRow = new ID("{C9D0E46C-3115-40FC-AC34-201B2BF8CCA2}");
                public readonly static ID ColSeperator = new ID("{C6FA013A-E666-498D-B89D-4A7DFA62BAB0}");
                public readonly static ID IsFirstRowHeader = new ID("{DBE41A28-8B74-44D2-B5B6-219B82A04ED0}");
            }
        }
        public struct MasterDataSetting
        {
            public readonly static ID ID = new ID("{67560C0E-DDAE-497E-B108-93448B8C9229}");
            public struct Fields
            {
                public readonly static ID Property = new ID("{96B35DE2-AD0D-436F-A61D-B7AA6B578670}");
                public readonly static ID IsEnable = new ID("{781E6057-CFB0-4615-8F2B-561D89E753B2}");
                public readonly static ID Address = new ID("{8896E8B2-44D3-4C8E-81C4-057E1EDF230D}");
                public readonly static ID Latitude = new ID("{1DD15398-FC4F-429C-B8F4-AFD78A161ADD}");
                public readonly static ID Longitude = new ID("{2FA20EC6-0C47-4422-9D24-565271F4743A}");
                public readonly static ID CreateSubFolder = new ID("{340C0254-6724-4125-9319-1DD388F8FC2C}");
                public readonly static ID Alphabets = new ID("{5FBED076-2E7F-496F-BD74-D0540CBF2EDE}");
            }
        }

        public struct TagItem
        {
            public readonly static ID ID = new ID("{69B601A8-8555-4411-8C17-AA91671EEC32}");
            public struct Fields
            {
                public readonly static ID Value = new ID("{03834B49-81CD-4EA6-956B-1CF3018DCF8A}");
            }
        }

    }
}