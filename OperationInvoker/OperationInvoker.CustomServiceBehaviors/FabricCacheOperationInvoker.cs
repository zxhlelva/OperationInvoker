using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.ApplicationServer.Caching;
using System.ServiceModel.Web;
using System.Threading;
using System.ServiceModel;

namespace OperationInvoker.CustomServiceBehaviors
{
    public class FabricCacheOperationInvoker : IOperationInvoker
    {
        #region Private Readonly Fields
        private readonly IOperationInvoker operationInvoker;
        private readonly DataCache datacache;
        #endregion

        #region Public Constructor
        public FabricCacheOperationInvoker(IOperationInvoker operationInvoker, DataCacheFactory dataCacheFactory)
        {
            this.operationInvoker = operationInvoker;
            this.datacache = dataCacheFactory.GetDefaultCache();
        }
        #endregion

        #region  IOperationInvoker Members
        public object[] AllocateInputs()
        {
            return this.operationInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            string cacheKey = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.ToString();
            CachedResult cacheResult = this.datacache.Get(cacheKey) as CachedResult;
            if (cacheResult != null)
            {
                outputs = cacheResult.Outputs;
                return cacheResult.ReturnValue;
            }
            else
            {
                object result = this.operationInvoker.Invoke(instance, inputs, out outputs);
                cacheResult = new CachedResult { ReturnValue = result, Outputs = outputs };
                datacache.Put(cacheKey, cacheResult);
                return result;
            }
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            string cacheKey = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.ToString();
            CachedResult cacheResult = this.datacache.Get(cacheKey) as CachedResult;
            CachingUserState cachingUserState = new CachingUserState
            {
                CacheItem = cacheResult,
                CacheKey = cacheKey,
                OriginalUserCallback = callback,
                OriginalUserState = state
            };
            IAsyncResult asyncResult;
            if (cacheResult != null)
            {
                InvokerDelegate invoker = cacheResult.GetValue;
                object[] outputs;
                asyncResult = invoker.BeginInvoke(inputs, out outputs, this.InvokerCallback, cachingUserState);
            }
            else
            {
                asyncResult = this.operationInvoker.InvokeBegin(instance, inputs, this.InvokerCallback, cachingUserState);
            }

            return new CachingAsyncResult(asyncResult, cachingUserState);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult asyncResult)
        {
            CachingAsyncResult cachingAsyncResult = asyncResult as CachingAsyncResult;
            CachingUserState cachingUserState = cachingAsyncResult.CachingUserState;
            if (cachingUserState.CacheItem == null)
            {
                object result = this.operationInvoker.InvokeEnd(instance, out outputs, cachingAsyncResult.AsyncResult);
                cachingUserState.CacheItem = new CachedResult { ReturnValue = result, Outputs = outputs };
                this.datacache.Put(cachingUserState.CacheKey, cachingUserState.CacheItem);
                return result;
            }
            else
            {
                InvokerDelegate invoker = ((System.Runtime.Remoting.Messaging.AsyncResult)cachingAsyncResult.AsyncResult).AsyncDelegate as InvokerDelegate;
                invoker.EndInvoke(out outputs, cachingAsyncResult.AsyncResult);
                return cachingUserState.CacheItem.ReturnValue;
            }
        }

        public bool IsSynchronous
        {
            get { return this.operationInvoker.IsSynchronous; }
        }
        #endregion

        #region Private Help Methods
        private delegate object InvokerDelegate(object[] inputs, out object[] outputs);

        private void InvokerCallback(IAsyncResult asyncResult)
        {
            CachingUserState cachingUserState = asyncResult.AsyncState as CachingUserState;
            cachingUserState.OriginalUserCallback(new CachingAsyncResult(asyncResult, cachingUserState));
        }
        #endregion

        #region Private Help Classes
        private class CachingUserState
        {
            public CachedResult CacheItem { get; set; }
            public string CacheKey { get; set; }
            public AsyncCallback OriginalUserCallback { get; set; }
            public object OriginalUserState { get; set; }
        }

        [Serializable]
        private class CachedResult
        {
            public object ReturnValue { get; set; }
            public object[] Outputs { get; set; }

            public object GetValue(object[] inputs, out object[] outputs)
            {
                outputs = this.Outputs;
                return this.ReturnValue;
            }
        }

        private class CachingAsyncResult : IAsyncResult
        {
            IAsyncResult asyncResult;
            CachingUserState cachingUserState;
            public CachingAsyncResult(IAsyncResult asyncResult, CachingUserState cachingUserState)
            {
                this.asyncResult = asyncResult;
                this.cachingUserState = cachingUserState;
            }

            public object AsyncState
            {
                get { return this.cachingUserState.OriginalUserState; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get { return this.asyncResult.AsyncWaitHandle; }
            }

            public bool CompletedSynchronously
            {
                get { return this.asyncResult.CompletedSynchronously; }
            }

            public bool IsCompleted
            {
                get { return this.asyncResult.IsCompleted; }
            }

            internal CachingUserState CachingUserState
            {
                get { return this.cachingUserState; }
            }

            internal IAsyncResult AsyncResult
            {
                get { return this.asyncResult; }
            }
        }
        #endregion
    }
}
