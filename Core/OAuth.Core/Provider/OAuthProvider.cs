using System;
using System.Collections.Generic;
using OAuth.Core.Interfaces;
using OAuth.Core.Provider.Inspectors;
using OAuth.Core.Storage;
using OAuth.Core.Storage.Interfaces;

namespace OAuth.Core.Provider
{
    public class OAuthProvider : IOAuthProvider
    {
        readonly List<IContextInspector> _inspectors = new List<IContextInspector>();
        readonly ITokenStore _tokenStore;

        public OAuthProvider(ITokenStore tokenStore, params IContextInspector[] inspectors)
        {
            if (tokenStore == null) throw new ArgumentNullException("tokenStore");
            _tokenStore = tokenStore;

            if (inspectors != null) _inspectors.AddRange(inspectors);
        }

        #region IOAuthProvider Members

        public virtual IToken GrantRequestToken(IOAuthContext context)
        {
            InspectRequest(context);

            return _tokenStore.CreateRequestToken(context);
        }

        public virtual IToken ExchangeRequestTokenForAccessToken(IOAuthContext context)
        {
            var token = _tokenStore.GetToken(context);
            if (token != null)
                context.TokenSecret = token.TokenSecret;
            InspectRequest(context);

            _tokenStore.ConsumeRequestToken(context);

            switch (_tokenStore.GetStatusOfRequestForAccess(context))
            {
                case RequestForAccessStatus.Granted:
                    break;
                case RequestForAccessStatus.Unknown:
                    throw Error.ConsumerHasNotBeenGrantedAccessYet(context);
                default:
                    throw Error.ConsumerHasBeenDeniedAccess(context);
            }

            return _tokenStore.GetAccessTokenAssociatedWithRequestToken(context);
        }

        public virtual void AccessProtectedResourceRequest(IOAuthContext context)
        {
            var token = _tokenStore.GetToken(context);
            if (token != null)
                context.TokenSecret = token.TokenSecret;
            InspectRequest(context);

            _tokenStore.ConsumeAccessToken(context);
        }

        #endregion

        public void AddInspector(IContextInspector inspector)
        {
            _inspectors.Add(inspector);
        }

        protected virtual void InspectRequest(IOAuthContext context)
        {
            foreach (IContextInspector inspector in _inspectors)
            {
                inspector.InspectContext(context);
            }
        }
    }
}