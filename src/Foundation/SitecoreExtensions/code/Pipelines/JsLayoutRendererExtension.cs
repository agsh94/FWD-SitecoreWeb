/*9fbef606107a605d69c0edbcd8029e5d*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using FWD.Foundation.Logging.CustomSitecore;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Analytics.Model;
using Sitecore.Caching;
using Sitecore.Data.Items;
using Sitecore.JavaScriptServices.Configuration;
using Sitecore.JavaScriptServices.ViewEngine.Presentation;
using Sitecore.JavaScriptServices.ViewEngine.RenderingEngine;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering;
using Sitecore.LayoutService.Serialization;
using Sitecore.Mvc.Presentation;
using Sitecore.Sites;
using Sitecore.Web;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class JsLayoutRendererExtension : JsLayoutRenderer
    {
        public JsLayoutRendererExtension(Rendering rendering, AppConfiguration appConfig, NamedConfiguration layoutServiceNamedConfig, ILayoutService layoutService, ISerializerService serializerService, IJssRendererConfiguration jssRendererConfiguration) : base(rendering, appConfig, layoutServiceNamedConfig, layoutService, serializerService, jssRendererConfiguration)
        {
        }

        protected override object[] ResolveFunctionArgs()
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            LoggerSsr.Log.LogStartTime("GetJson", DateTime.Now);

            string requestPath = GetRequestPath();
            Item itemToRender = GetItemToRender();
            RenderedItem rendered = RenderItem(itemToRender, LayoutServiceNamedConfiguration);
            dynamic viewBag = GetViewBag(GetViewBagPipelineArgs(itemToRender));
            string text = SerializerService.Serialize(rendered, LayoutServiceNamedConfiguration.SerializationConfiguration);
            object obj = JsonConvert.SerializeObject(viewBag, LayoutServiceNamedConfiguration.SerializationConfiguration.JsonSerializerSettings);

            var baseCacheKey = $"{rendered.ItemId}-{rendered.ItemLanguage}-{rendered.ItemVersion}";
            var htmlCacheKey = $"{baseCacheKey}-html";
            var statusCacheKey = $"{baseCacheKey}-status";
            var redirectCacheKey = $"{baseCacheKey}-redirect";

            LoggerSsr.Log.LogExecutionTime("GetJson", stopWatch.ElapsedMilliseconds);

            return new[]
            {
                requestPath,
                text,
                obj,
                htmlCacheKey,
                statusCacheKey,
                redirectCacheKey
            };
        }

        protected override void PerformRender(TextWriter writer, IRenderEngine renderEngine, string moduleName, string functionName, object[] functionArgs)
        {
            LoggerSsr.Log.LogStartTime("InvokeNodeRequest", DateTime.Now);
            Stopwatch stopWatch = Stopwatch.StartNew();
            var htmlCacheKey = functionArgs[3] as string;
            var statusCacheKey = functionArgs[4] as string;
            var redirectCacheKey = functionArgs[5] as string;
            RenderResult renderResult;

            if (CheckifPersonalizationEnabled() || CheckForExperiencEditor() || CheckIfCachingIsDisabledOnPage())
            {
                renderResult = renderEngine.Invoke<RenderResult>(moduleName, functionName, functionArgs);
            }
            else
            {
                renderResult = GetOrCreateCache(htmlCacheKey, statusCacheKey, redirectCacheKey, () => renderEngine.Invoke<RenderResult>(moduleName, functionName, functionArgs));
            }
            LoggerSsr.Log.LogExecutionTime("InvokeNodeRequest", stopWatch.ElapsedMilliseconds);
            int? status;
            if (!string.IsNullOrWhiteSpace(renderResult.Redirect))
            {
                bool flag = false;
                if (renderResult.Status.HasValue)
                {
                    status = renderResult.Status;
                    int num = 302;
                    if (!(status.GetValueOrDefault() == num && status.HasValue))
                    {
                        this.JssRendererConfiguration.HttpContext.Response.RedirectPermanent(renderResult.Redirect);
                        flag = true;
                    }
                }
                if (!flag)
                {
                    this.JssRendererConfiguration.HttpContext.Response.Redirect(renderResult.Redirect);
                }
            }
            status = renderResult.Status;
            if (status.HasValue)
            {
                HttpResponseBase response = this.JssRendererConfiguration.HttpContext.Response;
                status = renderResult.Status;
                int num = status.Value;
                response.StatusCode = num;
            }
            writer.Write(renderResult.Html);
        }

        private RenderResult GetOrCreateCache(string htmlCacheKey, string statusCacheKey, string redirectCacheKey, Func<RenderResult> p)
        {
            SiteContext site = Context.Site;
            if (site == null)
                return null;
            HtmlCache htmlCache = CacheManager.GetHtmlCache(site);
            string html = htmlCache?.GetHtml(htmlCacheKey);
            string status = htmlCache?.GetHtml(statusCacheKey);
            int statusValue;
            int? outputStatus = null;
            if (int.TryParse(status, out statusValue))
            {
                outputStatus = statusValue;
            }
            string redirect = htmlCache?.GetHtml(redirectCacheKey);
            if (html == null)
            {
                var renderResult = p.Invoke();
                string resultStatus = string.Empty;
                if (!string.IsNullOrEmpty(renderResult.Status?.ToString()))
                {
                    resultStatus = renderResult.Status.ToString();
                }
                string redirectStatus = string.Empty;
                if (!string.IsNullOrEmpty(renderResult.Redirect))
                {
                    redirectStatus = renderResult.Redirect;
                }
                htmlCache?.SetHtml(htmlCacheKey, renderResult.Html);
                htmlCache?.SetHtml(statusCacheKey, resultStatus);
                htmlCache?.SetHtml(redirectCacheKey, redirectStatus);
                return renderResult;
            }
            return new RenderResult()
            {
                Html = html,
                Redirect = redirect,
                Status = outputStatus
            };
        }

        private bool CheckifPersonalizationEnabled()
        {
            if (Tracker.Current != null && Tracker.Current.CurrentPage != null)
            {
                bool personalizationEnabled = Tracker.Current.CurrentPage.IsTestSetIdNull();
                if (!personalizationEnabled)
                    return true;
                var personalizationList = new List<PersonalizationRuleData>(Tracker.Current.CurrentPage.Personalization.ExposedRules);
                var filteredList = personalizationList.FindAll(x => x.RuleId.Guid != Guid.Empty);
                if (filteredList.Count > 0)
                    return true;
            }
            return false;
        }
        private bool CheckForExperiencEditor()
        {
            string mode = string.Empty;
            bool checkExperienceEditor = false;
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                mode = WebUtil.ExtractUrlParm("sc_mode", HttpContext.Current.Request.RawUrl);
                if (mode == "edit" || mode == "preview")
                {
                    checkExperienceEditor = true;
                }
            }

            return checkExperienceEditor;
        }

        private bool CheckIfCachingIsDisabledOnPage()
        {
            Item item = Sitecore.Context.Item;
            Sitecore.Data.Fields.CheckboxField checkboxField = item?.Fields[CommonConstants.DisableHtmlCacheFieldID];
            return checkboxField != null && checkboxField.Checked;
        }
    }
}