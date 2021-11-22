/*9fbef606107a605d69c0edbcd8029e5d*/
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWD.Foundation.AppInsight.Telemetry.Processor
{
    public class SitecoreAdaptiveSamplingTelemetryProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public SitecoreAdaptiveSamplingTelemetryProcessor(ITelemetryProcessor next)
        {
            this.Next = next;
        }
        public void Process(ITelemetry item)
        {
            if (!OKtoSend(item)) { return; }
            this.Next.Process(item);
        }
        private bool OKtoSend(ITelemetry item)
        {
            var requestTelemetry = item as RequestTelemetry;
            var traceTelemetry = item as TraceTelemetry;
            List<string> excludeurlsource = new List<string>();
            string excludeurls = Sitecore.Configuration.Settings.GetAppSetting(GlobalConstants.ExcludeUrlFromTelemetry);
            if (!string.IsNullOrEmpty(excludeurls))
                excludeurlsource = excludeurls.Split(',').ToList<string>();
            if (requestTelemetry != null && requestTelemetry.ResponseCode.Equals("200"))
            {
                string requestUrl = requestTelemetry.Url.AbsoluteUri;
                if (excludeurlsource != null && excludeurlsource.Any<string>((Func<string, bool>)(x => requestUrl.Contains(x))))
                    return false;
            }
            else if (traceTelemetry != null && traceTelemetry.SeverityLevel != SeverityLevel.Error)
            {
                return false;
            }
            return true;
        }
    }
}