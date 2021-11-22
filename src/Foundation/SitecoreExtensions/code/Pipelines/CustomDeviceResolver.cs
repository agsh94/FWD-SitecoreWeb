using FWD.Foundation.Logging.CustomSitecore;
using Sitecore;
using Sitecore.Globalization;
using Sitecore.Pipelines.HttpRequest;
using System;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomDeviceResolver : DeviceResolver
    {
        public override void Process(HttpRequestArgs args)
        {
            try
            {
                base.Process(args);
            }
            catch (Exception ex)
            {
                Language language;
                string defaultLanguageSetting = Sitecore.Configuration.Settings.GetSetting(CommonConstants.DefaultLanguage);
                if (Language.TryParse(defaultLanguageSetting, out language))
                {
                    Context.Language = language;
                }
                Logger.Log.Error("Exception while setting context language " + ex);
            }
        }
    }
}