using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using Microsoft.ApplicationServer.Caching;

namespace OperationInvoker.CustomServiceBehaviors
{
    public class FabricCacheOperationInvokerEndpointBehavior : IEndpointBehavior
    {
        #region IEndpointBehavior Members
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var server = new List<DataCacheServerEndpoint>();
            server.Add(new DataCacheServerEndpoint("localhost", 22233));
            var conf = new DataCacheFactoryConfiguration();
            conf.Servers = server;
            DataCacheFactory dataCacheFactory = new DataCacheFactory(conf);
            foreach (OperationDescription operation in endpoint.Contract.Operations)
            {
                if (operation.Behaviors.Contains(typeof(FabricCacheOperationInvokerOperationBehavior)))
                {
                    continue;
                }
                operation.Behaviors.Add(new FabricCacheOperationInvokerOperationBehavior(dataCacheFactory));
            }
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
        #endregion
    }
}
