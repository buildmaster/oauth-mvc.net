using System.Security.Cryptography.X509Certificates;
using OAuth.Core.Interfaces;
using OAuth.Core.Signing;
using OAuth.Core.Storage.Interfaces;

namespace OAuth.Core.Provider.Inspectors
{
    public class SignatureValidationInspector : IContextInspector
    {
        readonly IConsumerStore _consumerStore;
        readonly IOAuthContextSigner _signer;

        public SignatureValidationInspector(IConsumerStore consumerStore)
            : this(consumerStore, new OAuthContextSigner())
        {
        }

        public SignatureValidationInspector(IConsumerStore consumerStore, IOAuthContextSigner signer)
        {
            _consumerStore = consumerStore;
            _signer = signer;
        }

        #region IContextInspector Members

        public virtual void InspectContext(IOAuthContext context)
        {
            SigningContext signingContext = CreateSignatureContextForConsumer(context);

            if (!_signer.ValidateSignature(context, signingContext))
            {
                throw Error.FailedToValidateSignature(context);
            }
        }

        #endregion

        protected virtual bool SignatureMethodRequiresCertificate(string signatureMethod)
        {
            return ((signatureMethod != SignatureMethod.HmacSha1) && (signatureMethod != SignatureMethod.PlainText));
        }

        protected virtual SigningContext CreateSignatureContextForConsumer(IOAuthContext context)
        {
            var signingContext = new SigningContext { ConsumerSecret = _consumerStore.GetConsumerSecret(context) };

            if (SignatureMethodRequiresCertificate(context.SignatureMethod))
            {
                X509Certificate2 cert = _consumerStore.GetConsumerCertificate(context);
                signingContext.Algorithm = cert.PublicKey.Key;
            }

            return signingContext;
        }
    }
}