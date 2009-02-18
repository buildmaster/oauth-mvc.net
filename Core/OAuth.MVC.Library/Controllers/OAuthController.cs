using System.Web.Mvc;
using OAuth.MVC.Library.Interfaces;
using OAuth.MVC.Library.Results;

namespace OAuth.MVC.Library.Controllers
{
  public class OAuthController:Controller
  {
    private readonly IOAuthService oauthService;

    public OAuthController(IOAuthService oauthService)
    {
      this.oauthService = oauthService;
    }

    public ActionResult RequestToken()
    {
      var oauthRequest = CreateRequest(OAuthConstants.EndPointType.RequestTokenRequest);
      if(oauthRequest.IsValid())
      {
        var requestToken = oauthService.GenerateRequestToken(oauthRequest.Consumer);
        return new OAuthTokenResult(requestToken);
      }
      return new OAuthRequestErrorResult(oauthRequest.Error);
    }
    public ActionResult AccessToken()
    {
      var oauthRequest = CreateRequest(OAuthConstants.EndPointType.AccessTokenRequest);
      if(oauthRequest.IsValid())
      {
        var accessToken = oauthService.GenerateAccessToken(oauthRequest.Consumer, oauthRequest.RequestToken.UserID);
        return new OAuthTokenResult(accessToken);
      }
      return new OAuthRequestErrorResult(oauthRequest.Error);
    }
    
    private IOAuthRequest CreateRequest(OAuthConstants.EndPointType endPointType)
    {
      return oauthService.BuildRequest(Request.Url, Request.HttpMethod, Request.Params,endPointType);
    }

    
  }
}