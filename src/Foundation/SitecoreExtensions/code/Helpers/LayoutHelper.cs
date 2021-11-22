/*9fbef606107a605d69c0edbcd8029e5d*/
using System;
using FWD.Foundation.Logging.CustomSitecore;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Exceptions;
using Sitecore.Layouts;
using Sitecore.SecurityModel;


namespace FWD.Foundation.SitecoreExtensions.Helpers
{
    public static class LayoutHelper
    {
        public static void ApplyActionToAllRenderings(Item item, Func<RenderingDefinition, RenderingActionResult> action)
        {
            ApplyActionToAllSharedRenderings(item, action);
        }


        public static void ApplyActionToAllSharedRenderings(Item item, Func<RenderingDefinition, RenderingActionResult> action)
        {
            ApplyActionToAllRenderings(item, FieldIDs.LayoutField, action);
        }


        public static void ApplyActionToAllFinalRenderings(Item item, Func<RenderingDefinition, RenderingActionResult> action)
        {
            ApplyActionToAllRenderings(item, FieldIDs.FinalLayoutField, action);
        }

        private static void ApplyActionToAllRenderings(Item item, ID fieldId, Func<RenderingDefinition, RenderingActionResult> action)
        {
            try
            {
                string currentLayoutXml = LayoutField.GetFieldValue(item.Fields[fieldId]);
                if (string.IsNullOrEmpty(currentLayoutXml)) return;

                var newXml = ApplyActionToLayoutXml(currentLayoutXml, action);
                if (newXml != null)
                {
                    using (new SecurityDisabler())
                    {
                        using (new EditContext(item))
                        {
                            LayoutField.SetFieldValue(item.Fields[fieldId], newXml);
                        }
                    }
                }
            }catch(EditingNotAllowedException ex)
            {
                Logger.Log.Error("Unable to edit item in edition mode ", ex);
            }
        }

        private static string ApplyActionToLayoutXml(string xml, Func<RenderingDefinition, RenderingActionResult> action)
        {
            LayoutDefinition layout = LayoutDefinition.Parse(xml);

            xml = layout.ToXml();

            for (int deviceIndex = layout.Devices.Count - 1; deviceIndex >= 0; deviceIndex--)
            {
                var device = layout.Devices[deviceIndex] as DeviceDefinition;

                if (device == null) continue;


                for (int renderingIndex = device.Renderings.Count - 1; renderingIndex >= 0; renderingIndex--)
                {
                    var rendering = device.Renderings[renderingIndex] as RenderingDefinition;

                    if (rendering == null) continue;


                    RenderingActionResult result = action(rendering);

                    if (result == RenderingActionResult.Delete)
                        device.Renderings.RemoveAt(renderingIndex);
                }
            }

            string layoutXml = layout.ToXml();


            if (layoutXml != xml)
            {
                return layoutXml;
            }

            return null;
        }
    }
}