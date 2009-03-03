using System;
using System.Web.Mvc;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider;
using OAuth.MVC.Library.Results;

namespace OAuth.MVC.Library.Controllers
{
  public class OAuthController:Controller
  {
    private readonly IOAuthContextBuilder oAuthContextBuilder;
    private readonly IOAuthProvider oAuthProvider;

    public OAuthController(IOAuthContextBuilder oAuthContextBuilder,IOAuthProvider oAuthProvider)
    {
      
      this.oAuthContextBuilder = oAuthContextBuilder;
      this.oAuthProvider = oAuthProvider;
    }

    public ActionResult RequestToken()
    {
      var oauthContext = oAuthContextBuilder.FromHttpRequest(Request);
      try
      {
        var token = oAuthProvider.GrantRequestToken(oauthContext);
        return new OAuthTokenResult(token);
      }
      catch (OAuthException e)
      {
        return new OAuthExceptionResult(e);
        
      }
    }
    public ActionResult AccessToken()
    {
      var oauthContext = oAuthContextBuilder.FromHttpRequest(Request);
      try
      {
        var token = oAuthProvider.ExchangeRequestTokenForAccessToken(oauthContext);
        return new OAuthTokenResult(token);
      }
      catch (OAuthException e)
      {
        return new OAuthExceptionResult(e);
      }
    }
    
    

    
  }
}