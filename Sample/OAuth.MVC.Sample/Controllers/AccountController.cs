using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using OAuth.MVC.Library;
using OAuth.MVC.Library.Interfaces;

namespace OAuth.MVC.Sample.Controllers
{

  [HandleError]
  public class AccountController : Controller
  {
    private readonly IOAuthService oAuthService;

    public AccountController(IOAuthService oAuthService)
    {
      this.oAuthService = oAuthService;
    }

    public ActionResult AuthoriseRequestToken(string oauth_token, string oauth_callback)
    {
      if(Request.Params[OAuthConstants.PARAM_TOKEN]!=null)
        oAuthService.GetRequestToken(Request.Params[OAuthConstants.PARAM_TOKEN]).IsAuthorized = true;
      if(Request.Params[OAuthConstants.PARAM_CALLBACK]!=null)
        return new RedirectResult(Request.Params[OAuthConstants.PARAM_CALLBACK]);
      return new EmptyResult();
    }
  }
}
