using OAuth.Core.Interfaces;

namespace OAuth.Core.Signing
{
    public interface IOAuthContextSigner
    {
        void SignContext(IOAuthContext authContext, SigningContext signingContext);
        bool ValidateSignature(IOAuthContext authContext, SigningContext signingContext);
    }

}