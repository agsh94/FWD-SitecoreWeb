/*9fbef606107a605d69c0edbcd8029e5d*/
#region
using log4net.Appender;
using log4net.spi;
using System.Diagnostics.CodeAnalysis;
#endregion


namespace FWD.Foundation.Logging.CustomSitecore
{
    /// <summary>
    /// CustomRollingFileAppender to append additional properties in log file related to the exception
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CustomRollingFileAppender : RollingFileAppender
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            var properties = loggingEvent?.Properties;

            if (properties != null)
            {
                properties["sitename"] = string.Empty;

                if (Sitecore.Context.Site != null)
                {
                    properties["sitename"] = Sitecore.Context.Site.Name;
                }
            }

            base.Append(loggingEvent);

        }
    }
}