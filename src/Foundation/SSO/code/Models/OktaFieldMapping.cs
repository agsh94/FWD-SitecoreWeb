using Sitecore.Xml;
using System.Collections.Generic;
using System.Xml;

namespace FWD.Foundation.SSO.Models
{
    public class OktaFieldMapping
    {
        public Dictionary<string, string> FieldMappings { get; private set; }
        public OktaFieldMapping()
        {
            this.FieldMappings = new Dictionary<string, string>();
        }

        public void AddItem(XmlNode xmlNode)
        {
            var name = XmlUtil.GetAttribute("name", xmlNode);
            var fieldName = XmlUtil.GetAttribute("OktaFieldName", xmlNode);

            this.FieldMappings.Add(name, fieldName);
        }
    }
}