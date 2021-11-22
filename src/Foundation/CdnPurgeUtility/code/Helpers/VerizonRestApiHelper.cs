using FWD.Foundation.CDNPurgeUtility;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using FWD.Foundation.CdnPurgeUtility.Models;
using FWD.Foundation.Logging.CustomSitecore;
using System.Net;
using System.Text;

namespace FWD.Foundation.CdnPurgeUtility.Helpers
{
    public static class VerizonRestApiHelper
    {
        public static PurgeStatusResponse PurgeContent(string apiurl, string authToken, List<string> mediaPath, int mediaType)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(VerizonRestAPI.AuthorizationHeader, authToken);
                    RestApiInputParameter json = new RestApiInputParameter(mediaPath, mediaType);
                    var content = new StringContent(
                        JsonConvert.SerializeObject(json),
                        Encoding.UTF8,
                        "application/json"
                    );
                    HttpResponseMessage response = client.PutAsync(apiurl, content).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = JsonConvert.DeserializeObject<PurgeStatusResponse>(response.Content.ReadAsStringAsync().Result);
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in VerizonRestApiHelper PurgeContent method ", ex);
                return null;
            }
        }
        public static GetStatusResponse GetPurgeStatus(string apiurl, string authToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(VerizonRestAPI.AuthorizationHeader, authToken);
                    HttpResponseMessage response = client.GetAsync(apiurl).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = JsonConvert.DeserializeObject<GetStatusResponse>(response.Content.ReadAsStringAsync().Result);
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception in VerizonRestApiHelper GetPurgeStatus method ", ex);
                return null;
            }
        }
    }
}
