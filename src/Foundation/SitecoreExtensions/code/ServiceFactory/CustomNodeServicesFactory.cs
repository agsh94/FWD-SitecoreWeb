/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.SitecoreExtensions.CustomNodeInstance;
using Sitecore.JavaScriptServices.ViewEngine.NodeServices;
using System;

namespace FWD.Foundation.SitecoreExtensions.ServiceFactory
{
    public class CustomNodeServicesFactory : INodeServicesFactory
    {
        public INodeServices CreateNodeServices(NodeServicesOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            options.NodeInstanceFactory = () => new CustomHttpNodeInstance(options);
            return new DefaultNodeServices(options.NodeInstanceFactory);
        }
    }
}