namespace OAuth.Core.Interfaces
{
    public interface IOAuthProvider
    {
        IToken GrantRequestToken(IOAuthContext authContext);
        IToken ExchangeRequestTokenForAccessToken(IOAuthContext authContext);
        void AccessProtectedResourceRequest(IOAuthContext authContext);
    }
}