using System;
using System.Collections.Specialized;
using OAuth.MVC.Library.Interfaces;

namespace OAuth.MVC.Library
{
  public class OAuthService : IOAuthService
  {
    private readonly IOAuthRepository oAuthRepository;
    private readonly ITokenGenerator tokenGenerator;

    public OAuthService(IOAuthRepository oAuthRepository, ITokenGenerator tokenGenerator)
    {
      this.oAuthRepository = oAuthRepository;
      this.tokenGenerator = tokenGenerator;
    }

    public IOAuthRequest BuildRequest(Uri url, string httpMethod, NameValueCollection parameters,NameValueCollection Headers, OAuthConstants.EndPointType EndPointType)
    {
      //we have to copy the params as http params are read only
      var copiedParams = new NameValueCollection {parameters,Helpers.GetAuthHeaderParameters(Headers)};
      return new OAuthRequest(url, httpMethod, copiedParams, oAuthRepository, EndPointType);
    }

    public IRequestToken GenerateRequestToken(IConsumer consumer)
    {
      var token = tokenGenerator.GenerateNewRequestToken();
      consumer.SaveRequestToken(token);
      return token;
    }

    public IAccessToken GenerateAccessToken(IConsumer consumer, Guid userID)
    {
       var accessToken = tokenGenerator.GenerateNewAccessToken();
      accessToken.UserID = userID;
      consumer.SaveAccessToken(accessToken);
      return accessToken;
    }

    public IRequestToken GetRequestToken(string token)
    {
      return oAuthRepository.GetExistingToken(token);
    }

    public void AuthorizeRequestToken(string requestToken, Guid userId)
    {
      var requestTokenObject = oAuthRepository.GetExistingToken(requestToken);
      if (requestTokenObject != null && !requestTokenObject.IsAuthorized)
      {
        requestTokenObject.IsAuthorized = true;
        requestTokenObject.UserID = userId;
      }

    }
  }
}