/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.JavaScriptServices.Globalization.Controllers;
using Sitecore.JavaScriptServices.Globalization.Dictionary;

namespace FWD.Foundation.SitecoreExtensions.Services
{
    public class RegisterDependencies : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<CustomDictionaryServiceController>();
            serviceCollection.AddTransient<DictionaryServiceController>();
            serviceCollection.AddSingleton<IDictionaryDomainResolver, DictionaryDomainResolver>();
            serviceCollection.AddSingleton<ITranslationDictionaryReader, TranslationDictionaryReader>();
            serviceCollection.AddScoped<IApplicationDictionaryReader, ApplicationDictionaryReader>();
        }
    }
}