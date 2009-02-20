using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace OAuth.MVC.Library.Interfaces
{
  /// <summary>
  /// 
  /// </summary>
  public interface IOAuthService
  {
    /// <summary>
    /// Builds an oauth request request.
    /// </summary>
    /// <param name="url">The URL of the request.</param>
    /// <param name="httpMethod">The HTTP method of the request.</param>
    /// <param name="parameters">The parameters from the request.</param>
    /// <param name="Headers">The headers from the request.</param>
    /// <param name="EndPointType">EndPoint type of the request.</param>
    /// <returns></returns>
    IOAuthRequest BuildRequest(Uri url, string httpMethod, NameValueCollection parameters, NameValueCollection Headers, OAuthConstants.EndPointType EndPointType);
    /// <summary>
    /// Generates a request token for the consumer.
    /// </summary>
    /// <param name="consumer">The consumer to own the request token.</param>
    /// <returns></returns>
    IRequestToken GenerateRequestToken(IConsumer consumer);

    /// <summary>
    /// Generates the access token for this consumer and user.
    /// </summary>
    /// <param name="consumer">The consumer.</param>
    /// <param name="userID">The user ID.</param>
    /// <returns></returns>
    IAccessToken GenerateAccessToken(IConsumer consumer,Guid userID);

    IRequestToken GetRequestToken(string token);
  }
}