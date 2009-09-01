using OAuth.Core.Interfaces;

namespace OAuth.Core.Signing
{
    public interface IContextSignatureImplementation
    {
        string MethodName { get; }
        void SignContext(IOAuthContext authContext, SigningContext signingContext);
        bool ValidateSignature(IOAuthContext authContext, SigningContext signingContext);
    }
}