using OAuth.Core.Interfaces;

namespace OAuth.Core.Signing
{
    public class PlainTextSignatureImplementation : IContextSignatureImplementation
    {
        #region IContextSignatureImplementation Members

        public string MethodName
        {
            get { return SignatureMethod.PlainText; }
        }

        public void SignContext(IOAuthContext authContext, SigningContext signingContext)
        {
            authContext.Signature = GenerateSignature(authContext, signingContext);
        }

        public bool ValidateSignature(IOAuthContext authContext, SigningContext signingContext)
        {
            return (authContext.Signature == GenerateSignature(authContext, signingContext));
        }

        #endregion

        static string GenerateSignature(IToken authContext, SigningContext signingContext)
        {
            return UriUtility.UrlEncode(string.Format("{0}&{1}", signingContext.ConsumerSecret, authContext.TokenSecret));
        }
    }
}