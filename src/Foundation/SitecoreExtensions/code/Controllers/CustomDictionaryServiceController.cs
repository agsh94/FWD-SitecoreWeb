/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Helpers;
using Sitecore.Abstractions;
using Sitecore.Globalization;
using Sitecore.JavaScriptServices.Configuration;
using Sitecore.JavaScriptServices.Globalization.Controllers;
using Sitecore.JavaScriptServices.Globalization.Dictionary;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FWD.Foundation.SitecoreExtensions.Controllers
{
    public class CustomDictionaryServiceController : ApiController
    {
        protected readonly IConfigurationResolver ConfigurationResolver;
        protected readonly BaseLanguageManager LanguageManager;
        protected readonly IApplicationDictionaryReader AppDictionaryReader;

        public CustomDictionaryServiceController(
          IConfigurationResolver configurationResolver,
          BaseLanguageManager languageManager,
          IApplicationDictionaryReader appDictionaryReader)
        {
            this.ConfigurationResolver = configurationResolver;
            this.LanguageManager = languageManager;
            this.AppDictionaryReader = appDictionaryReader;
        }

        public DictionaryServiceResult GetDictionary(
          string appName,
          string language)
        {
            AppConfiguration application = this.ConfigurationResolver.ResolveForName(appName);
            if (application == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = (HttpContent)new StringContent("Unknown app name")
                });
            Language language1 = LanguageHelper.GetContextLanguage(language);
            if (language1 == (Language)null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = (HttpContent)new StringContent("Unknown language")
                });
            IDictionary<string, string> phrases = this.AppDictionaryReader.GetPhrases(application, language1);
            if (phrases == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = (HttpContent)new StringContent("Could not resolve dictionary for app")
                });
            return new DictionaryServiceResult()
            {
                app = application.Name,
                lang = language,
                phrases = phrases
            };
        }
    }
}