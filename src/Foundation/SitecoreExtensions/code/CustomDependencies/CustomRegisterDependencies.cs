/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.ServiceFactory;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.JavaScriptServices.ViewEngine.Node;
using Sitecore.JavaScriptServices.ViewEngine.Node.Helpers;
using Sitecore.JavaScriptServices.ViewEngine.NodeServices;

namespace FWD.Foundation.SitecoreExtensions.CustomDependencies
{
    public class CustomRegisterDependencies : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAsyncHelpers, AsyncHelpers>();
            serviceCollection.AddTransient<ILogger, Logger>();
            serviceCollection.AddSingleton<INodeRenderEngineFactory, NodeRenderEngineFactory>();
            serviceCollection.AddTransient<INodeServicesFactory, CustomNodeServicesFactory>();
            serviceCollection.AddTransient<IConfigurationReader, ConfigurationReader>();
            serviceCollection.AddTransient<INodeRenderEngineOptionsResolver, NodeRenderEngineOptionsResolver>();
        }
    }
}