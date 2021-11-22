/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Logging.CustomSitecore;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace FWD.Foundation.API
{
    public static class WebApi
    {
        public static JObject GetResponseFromApi(string apiEndPoint, string authToken, string locationType, string headerData)
        {
            JObject responseObject = null;
            if (string.IsNullOrEmpty(apiEndPoint) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(locationType))
            {
                return responseObject;
            }

            try
            {
                string userAgent = System.Configuration.ConfigurationManager.AppSettings[Constants.UserAgent];
                if(string.IsNullOrEmpty(userAgent))
                {
                    userAgent = Constants.Googlebot;
                }

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(apiEndPoint);
                webRequest.Method = Constants.PostMethod;

                byte[] data = Encoding.UTF8.GetBytes(headerData);

                webRequest.Headers.Add(Constants.Authorization, authToken);
                webRequest.Headers.Add(Constants.LocationType, locationType);

                
                webRequest.UserAgent = userAgent;
                webRequest.ContentType = Constants.ContentType;
                webRequest.ContentLength = data.Length;

                Stream requestStream = webRequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                Stream responseStream = response.GetResponseStream();

                StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8);

                string responseContent = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                responseStream.Close();
                response.Close();
                response.Dispose();

                if(!string.IsNullOrEmpty(responseContent))
                {
                    responseObject = JObject.Parse(responseContent);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("GetResponseFromAPI : " + apiEndPoint, ex);
            }

            return responseObject;
        }
    }
}