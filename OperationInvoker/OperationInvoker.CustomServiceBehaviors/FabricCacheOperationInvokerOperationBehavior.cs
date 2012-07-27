using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.ApplicationServer.Caching;

namespace OperationInvoker.CustomServiceBehaviors
{
    public class FabricCacheOperationInvokerOperationBehavior : IOperationBehavior
    {
        #region Private Readonly Field
        private readonly DataCacheFactory dataCacheFactory;
        #endregion

        #region Public Constructor
        public FabricCacheOperationInvokerOperationBehavior(DataCacheFactory dataCacheFactory)
        {
            this.dataCacheFactory = dataCacheFactory;
        }
        #endregion

        #region IOperation Behavior Members
        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {

        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Invoker = new FabricCacheOperationInvoker(dispatchOperation.Invoker, this.dataCacheFactory);
        }

        public void Validate(OperationDescription operationDescription)
        {

        }
        #endregion
    }
}
