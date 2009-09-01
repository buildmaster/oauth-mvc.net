using OAuth.Core.Interfaces;

namespace OAuth.Core.Storage.Interfaces
{
    public interface ITokenStore
    {
        /// <summary>
        /// Creates a request token for the consumer.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IToken CreateRequestToken(IOAuthContext context);

        /// <summary>
        /// Should consume a use of the request token, throwing a <see cref="OAuthException" /> on failure.
        /// </summary>
        /// <param name="requestContext"></param>
        void ConsumeRequestToken(IOAuthContext requestContext);

        /// <summary>
        /// Should consume a use of an access token, throwing a <see cref="OAuthException" /> on failure.
        /// </summary>
        /// <param name="accessContext"></param>
        void ConsumeAccessToken(IOAuthContext accessContext);

        /// <summary>
        /// Get the access token associated with a request token.
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        IToken GetAccessTokenAssociatedWithRequestToken(IOAuthContext requestContext);

        /// <summary>
        /// Returns the status for a request to access a consumers resources.
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        RequestForAccessStatus GetStatusOfRequestForAccess(IOAuthContext requestContext);

        IToken GetToken(IOAuthContext context);
    }
}