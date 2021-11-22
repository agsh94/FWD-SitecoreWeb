/*9fbef606107a605d69c0edbcd8029e5d*/
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.JavaScriptServices.ViewEngine.NodeServices;
using Sitecore.JavaScriptServices.ViewEngine.NodeServices.HostingModels.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomNodeProfiling
    {
    }
    public class CustomHttpNodeInstance : HttpNodeInstance
    {
        public CustomHttpNodeInstance(NodeServicesOptions options, int port = 0) : base(options, port)
        {
        }

        protected override ProcessStartInfo PrepareNodeProcessStartInfo(string entryPointFilename, string projectPath, string commandLineArguments,
            IDictionary<string, string> environmentVars, bool launchWithDebugging, int debuggingPort)
        {
            var startInfo = base.PrepareNodeProcessStartInfo(entryPointFilename, projectPath, commandLineArguments,
                environmentVars, launchWithDebugging, debuggingPort);

            // You might ask why you can't simply add custom arguments to the `commandLineArguments` parameter.
            // The answer is that the `commandLineArguments` value is added to the node.exe command _after_ the script name/path that will be executed by node.
            // However, the `--prof` flag must be placed _before_ the script name/path that will be executed by node.
            // Therefore, we invoke the base `PrepareNodeProcessStartInfo` method to generate the command arguments,
            // then prepend the arguments string with `--prof`.
            startInfo.Arguments = "--prof " + startInfo.Arguments;
            return startInfo;
        }
    }
      
    public class CustomNodeServicesFactory : INodeServicesFactory
    {
        public INodeServices CreateNodeServices(NodeServicesOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            // Use our `CustomHttpNodeInstance` when creating Node instances.
            options.NodeInstanceFactory = () => new CustomHttpNodeInstance(options);
            return new DefaultNodeServices(options.NodeInstanceFactory);
        }
    }

    public class CustomRegisterDependencies : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            // Remove the existing `INodeServicesFactory` service
            var toRemove = serviceCollection.FirstOrDefault(d => d.ServiceType == typeof(INodeServicesFactory));
            serviceCollection.Remove(toRemove);

            // Add the custom service
            serviceCollection.AddTransient<INodeServicesFactory, CustomNodeServicesFactory>();
        }
    }
}