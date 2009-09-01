using System;
using System.Web.Mvc;

namespace OAuth.MVC.Sample.Controllers
{

  [HandleError]
  public class AccountController : Controller
  {
    readonly TokenRepository _tokenRepository;

    public AccountController(TokenRepository tokenRepository)
    {
      _tokenRepository = tokenRepository;
    }

// ReSharper disable InconsistentNaming
    public ActionResult AuthoriseRequestToken(string oauth_token, string oauth_callback)
// ReSharper restore InconsistentNaming
    {
      var requestToken = _tokenRepository.GetRequestToken(oauth_token);
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
        _tokenRepository.SaveAccessToken(accessToken);
        _tokenRepository.SaveRequestToken(requestToken);
        return new JsonResult {Data=accessToken};
      }
      return new EmptyResult();
     
    }
  }
}
