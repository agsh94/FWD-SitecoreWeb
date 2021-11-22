/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FWD.Foundation.Multisite.Placeholders;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;

#endregion

namespace FWD.Foundation.Multisite
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.Replace(ServiceDescriptor.Singleton<BasePlaceholderCacheManager, SiteSpecificPlaceholderCacheManager>());
        }
    }
}