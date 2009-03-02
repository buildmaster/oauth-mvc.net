using System;
using System.Web.Mvc;
using DevDefined.OAuth.Storage;

namespace OAuth.MVC.Sample.Controllers
{

  [HandleError]
  public class AccountController : Controller
  {
    readonly TokenRepository tokenRepository;

    public AccountController(TokenRepository tokenRepository)
    {
      this.tokenRepository = tokenRepository;
    }

    public ActionResult AuthoriseRequestToken(string oauth_token, string oauth_callback)
    {
      var requestToken = tokenRepository.GetRequestToken(oauth_token);
      if(requestToken!=null)
      {
        requestToken.AccessDenied = false;
        var accessToken = new AccessToken
                            {
                              ConsumerKey = requestToken.ConsumerKey,
                              ExpireyDate = DateTime.UtcNow.AddDays(20),
                              Realm = requestToken.Realm,
                              Token = Guid.NewGuid().ToString(),
                              TokenSecret = Guid.NewGuid().ToString(),
                              UserName = Guid.NewGuid().ToString(),
                            };
        requestToken.AccessToken = accessToken;
        tokenRepository.SaveAccessToken(accessToken);
        tokenRepository.SaveRequestToken(requestToken);
        return new JsonResult(){Data=accessToken};
      }
      return new EmptyResult();
     
    }
  }
}
