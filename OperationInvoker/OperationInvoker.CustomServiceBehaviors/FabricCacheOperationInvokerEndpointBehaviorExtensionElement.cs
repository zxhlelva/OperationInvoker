using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using Microsoft.ApplicationServer.Caching;

namespace OperationInvoker.CustomServiceBehaviors
{
    public class FabricCacheOperationInvokerBehaviorExtensionElement : BehaviorExtensionElement
    {
        #region BehaviorExtensionElement Members
        public override Type BehaviorType
        {
            get { return typeof(FabricCacheOperationInvokerEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new FabricCacheOperationInvokerEndpointBehavior();
        }
        #endregion
    }
}
