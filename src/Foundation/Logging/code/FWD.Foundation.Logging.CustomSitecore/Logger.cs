/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System.Diagnostics.CodeAnalysis;
using log4net;
using Sitecore.Diagnostics;

#endregion

namespace FWD.Foundation.Logging.CustomSitecore
{
    /// <summary>
    ///     Logger
    /// </summary>
    /// <seealso cref="ILogger" />
    [ExcludeFromCodeCoverage]
    public class Logger : AbstractLog
    {
        /// <summary>The syncronization object</summary>
        private readonly object syncObj = new object();
        /// <summary>The log.</summary>
        private static readonly Logger Instance = new Logger();
        /// <summary>The logger name.</summary>
        private const string LoggerName = "FWD.Foundation.CustomLogger";
        /// <summary>The logger.</summary>
        private volatile ILog logger;

        /// <summary>
        /// Initializes static members of the <see cref="T:Sitecore.ContentSearch.Diagnostics.CrawlingLog" /> class.
        /// </summary>
        static Logger()
        {
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="T:Sitecore.ContentSearch.Diagnostics.CrawlingLog" /> class from being created.
        /// </summary>
        private Logger()
        {
        }

        /// <summary>Gets the log.</summary>
        public static Logger Log
        {
            get
            {
                return Logger.Instance;
            }
        }

        /// <summary>Gets the real logger.</summary>
        protected override ILog Log4NetLogger
        {
            get
            {
                if (this.logger == null)
                {
                    lock (this.syncObj)
                    {
                        if (this.logger == null)
                            this.logger = LogManager.GetLogger(LoggerName) ?? LoggerFactory.GetLogger(typeof(Logger));
                    }
                }
                return this.logger;
            }
        }
    }
}