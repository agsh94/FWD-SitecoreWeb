/*9fbef606107a605d69c0edbcd8029e5d*/
using HtmlAgilityPack;
using Sitecore.Shell.Controls.RichTextEditor.Pipelines.SaveRichTextContent;
using System.Linq;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class RteConfiguration
    {
        public void Process(SaveRichTextContentArgs args)
        {
            // Load the HTML into the HtmlAgilityPack
            var doc = new HtmlDocument { OptionWriteEmptyNodes = true };
            doc.LoadHtml(args.Content);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(args.Content);

            var nodes = (from node in htmlDoc.DocumentNode.Descendants()
                         where node.Name == "table" || node.Name == "td" || node.Name == "tr"
                         || node.Name == "col" || node.Name == "tbody" || node.Name == "colgroup"
                         select node);
            foreach (HtmlNode node in nodes)
            {
                if (node.Attributes["style"] != null)
                {
                    node.Attributes["style"].Value = node.Attributes["style"].Value.Replace("pt", "px");
                }
            }

            // Replace the Rich Text content with the modified content
            args.Content = htmlDoc.DocumentNode.OuterHtml;
        }
    }
}