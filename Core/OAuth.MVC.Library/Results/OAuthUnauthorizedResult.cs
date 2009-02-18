using System.Net;
using System.Web.Mvc;
using OAuth.MVC.Library.Configuration;

namespace OAuth.MVC.Library.Results
{
  public class OAuthUnauthorizedResult:ActionResult
  {
    public override void ExecuteResult(ControllerContext context)
    {
      context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
      context.HttpContext.Response.AppendHeader("WWW-Authenticate",
                                                     string.Format("OAuth {0}=\"{1}\"",
                                                                   OAuthConstants.PARAM_OAUTH_REALM,
                                                                   OAuthConfigurationSection.Read().Realm));
    }
  }
}