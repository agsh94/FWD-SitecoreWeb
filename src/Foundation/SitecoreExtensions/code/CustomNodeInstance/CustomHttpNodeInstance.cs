/*9fbef606107a605d69c0edbcd8029e5d*/
using Newtonsoft.Json;
using Sitecore.JavaScriptServices.ViewEngine.NodeServices;
using Sitecore.JavaScriptServices.ViewEngine.NodeServices.HostingModels;
using Sitecore.JavaScriptServices.ViewEngine.NodeServices.Util;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FWD.Foundation.SitecoreExtensions.CustomNodeInstance
{
    public class CustomHttpNodeInstance : OutOfProcessNodeInstance
    {
        private static readonly Regex EndpointMessageRegex = new Regex("^\\[Sitecore.JavaScriptServices.ViewEngine.NodeServices.HttpNodeHost:Listening on {(.*?)} port (\\d+)\\]$");
        protected readonly JsonSerializerSettings JsonSerializerSettings;
        private readonly WebClient _client;
        private bool _disposed;
        private string _endpoint;

        public CustomHttpNodeInstance(NodeServicesOptions options, int port = 0)
          : base(EmbeddedResourceReader.Read(typeof(CustomHttpNodeInstance), "/HostingModels/Http/entrypoint-http-node.js"), options.ProjectPath, options.WatchFileExtensions, CustomHttpNodeInstance.MakeCommandLineOptions(port), options.NodeInstanceOutputLogger, options.EnvironmentVariables, options.InvocationTimeoutMilliseconds, options.LaunchWithDebugging, options.DebuggingPort, options.NodePath)
        {
            this.JsonSerializerSettings = options.JsonSerializerSettings;
            this._client = (WebClient)new CustomHttpNodeInstance.TimeoutCapableWebClient(options.InvocationTimeoutMilliseconds);
        }

        private static string MakeCommandLineOptions(int port)
        {
            return string.Format("--port {0}", (object)port);
        }
        protected override T InvokeExport<T>(NodeInvocationInfo invocationInfo)
        {
            try
            {

                string data = JsonConvert.SerializeObject((object)invocationInfo, this.JsonSerializerSettings);
                this._client.Headers[HttpRequestHeader.ContentType] = "application/json";
                this._client.Encoding = Encoding.UTF8;
                string response = this._client.UploadString(this._endpoint, data);
                return JsonConvert.DeserializeObject<T>(response, this.JsonSerializerSettings);
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                    throw;
                else if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    using (StreamReader streamReader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        RpcJsonResponse rpcJsonResponse = JsonConvert.DeserializeObject<RpcJsonResponse>(streamReader.ReadToEnd(), this.JsonSerializerSettings);
                        throw new NodeInvocationException(rpcJsonResponse.ErrorMessage, rpcJsonResponse.ErrorDetails);
                    }
                }
                else
                    throw;
            }
        }

        protected override void OnOutputDataReceived(string outputData)
        {
            Match match = string.IsNullOrEmpty(this._endpoint) ? CustomHttpNodeInstance.EndpointMessageRegex.Match(outputData) : (Match)null;
            if (match != null && match.Success)
            {
                int num = int.Parse(match.Groups[2].Captures[0].Value);
                string str = match.Groups[1].Captures[0].Value;
                this._endpoint = string.Format("http://{0}:{1}", str == "::1" ? (object)("[" + str + "]") : (object)str, (object)num);
            }
            else
                base.OnOutputDataReceived(outputData);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (this._disposed)
                return;
            if (disposing)
                this._client.Dispose();
            this._disposed = true;
        }

        private class RpcJsonResponse
        {
            public string ErrorMessage = string.Empty;

            public string ErrorDetails = string.Empty;
        }

        protected class TimeoutCapableWebClient : WebClient
        {
            private readonly int _timeoutMsec;

            public TimeoutCapableWebClient(int timeoutMsec)
            {
                this._timeoutMsec = timeoutMsec;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest webRequest = base.GetWebRequest(address);
                webRequest.Timeout = this._timeoutMsec;
                return webRequest;
            }
        }
    }
}