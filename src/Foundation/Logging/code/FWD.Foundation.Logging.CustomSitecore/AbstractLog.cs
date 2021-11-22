/*9fbef606107a605d69c0edbcd8029e5d*/
using log4net;
using System;

namespace FWD.Foundation.Logging.CustomSitecore
{
    public abstract class AbstractLog
    {
        /// <summary>Gets the log.</summary>
        protected abstract ILog Log4NetLogger { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Sitecore.ContentSearch.Diagnostics.AbstractLog" /> is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if initialized; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool Initialized
        {
            get
            {
                return this.Log4NetLogger != null;
            }
        }

        /// <summary>The error.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(string message, Exception exception = null)
        {
            if (!this.Initialized)
                return;
            if (exception == null)
                this.Log4NetLogger.Error((object)message);
            else
                this.Log4NetLogger.Error((object)message, exception);
        }

        /// <summary>The info.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(string message, Exception exception = null)
        {
            if (!this.Initialized)
                return;
            if (exception == null)
                this.Log4NetLogger.Info((object)message);
            else
                this.Log4NetLogger.Info((object)message, exception);
        }

        /// <summary>The warn.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(string message, Exception exception = null)
        {
            if (!this.Initialized)
                return;
            if (exception == null)
                this.Log4NetLogger.Warn((object)message);
            else
                this.Log4NetLogger.Warn((object)message, exception);
        }

        /// <summary>The fatal.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(string message, Exception exception = null)
        {
            if (!this.Initialized)
                return;
            if (exception == null)
                this.Log4NetLogger.Fatal((object)message);
            else
                this.Log4NetLogger.Fatal((object)message, exception);
        }

        /// <summary>The debug.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(string message, Exception exception = null)
        {
            if (!this.Initialized)
                return;
            if (exception == null)
                this.Log4NetLogger.Debug((object)message);
            else
                this.Log4NetLogger.Debug((object)message, exception);
        }

        /// <summary>Debugs the specified message delegate.</summary>
        /// <param name="messageDelegate">The message delegate.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(Func<string> messageDelegate, Exception exception = null)
        {
            if (!this.Initialized || !this.Log4NetLogger.IsDebugEnabled)
                return;
            this.Debug(messageDelegate(), exception);
        }

        public void LogStartTime(string method, DateTime startTime, string message = null)
        {
            if (!this.Initialized)
                return;

            this.Log4NetLogger.Info(String.Format("StartTime {0} {1} {2} {3} {4}",Sitecore.Context.RequestID, method, startTime.ToString("dd-MM-yyyy HH:mm:ss.fff"),0, message));
        }
        public void LogExecutionTime(string method, long executionTime, string message = null)
        {
            if (!this.Initialized)
                return;
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(Sitecore.Context.RequestID))
                return;
            this.Log4NetLogger.Info(String.Format("ExecutionTime {0} {1} {2} {3} {4}", Sitecore.Context.RequestID, method,DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff"), executionTime, message));
        }

    }
}