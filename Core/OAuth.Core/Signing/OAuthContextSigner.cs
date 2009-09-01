using System.Collections.Generic;
using System.Linq;
using OAuth.Core.Interfaces;

namespace OAuth.Core.Signing
{
    public class OAuthContextSigner : IOAuthContextSigner
    {
        readonly List<IContextSignatureImplementation> _implementations =
          new List<IContextSignatureImplementation>();

        public OAuthContextSigner(params IContextSignatureImplementation[] implementations)
        {
            if (implementations != null) _implementations.AddRange(implementations);
        }

        public OAuthContextSigner()
            : this(
              new RsaSha1SignatureImplementation(), new HmacSha1SignatureImplementation(),
              new PlainTextSignatureImplementation())
        {
        }

        #region IOAuthContextSigner Members

        public void SignContext(IOAuthContext authContext, SigningContext signingContext)
        {
            signingContext.SignatureBase = authContext.GenerateSignatureBase();
            FindImplementationForAuthContext(authContext).SignContext(authContext, signingContext);
        }

        public bool ValidateSignature(IOAuthContext authContext, SigningContext signingContext)
        {
            signingContext.SignatureBase = authContext.GenerateSignatureBase();
            return FindImplementationForAuthContext(authContext).ValidateSignature(authContext, signingContext);
        }

        #endregion

        IContextSignatureImplementation FindImplementationForAuthContext(IOAuthContext authContext)
        {
            IContextSignatureImplementation impl =
              _implementations.FirstOrDefault(i => i.MethodName == authContext.SignatureMethod);

            if (impl != null) return impl;

            throw Error.UnknownSignatureMethod(authContext.SignatureMethod);
        }
    }
}