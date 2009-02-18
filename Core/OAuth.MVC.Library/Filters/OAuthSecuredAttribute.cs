using System;
using System.Net;
using System.Web.Mvc;
using OAuth.MVC.Library.Configuration;
using OAuth.MVC.Library.Interfaces;

namespace OAuth.MVC.Library.Filters
{
  public class OAuthSecuredAttribute : FilterAttribute, IAuthorizationFilter
  {
    public IOAuthService OAuthService
    {
      get;
      set;
    }
    public void OnAuthorization(AuthorizationContext filterContext)
    {
      if (OAuthService == null)
      {
        throw new NullReferenceException("OAuthService needs to be set for OAuthSecuredAttribute, please use an IOC Container to do this");
      }
      var request = filterContext.HttpContext.Request;
      var oauthRequest = OAuthService.BuildRequest(filterContext.HttpContext.Request.Url, request.HttpMethod,
                                                   request.Params, OAuthConstants.EndPointType.AccessRequest);
      if (!oauthRequest.IsValid())
      {
        filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        filterContext.HttpContext.Response.Headers.Add("WWW-Authenticate",
                                                       string.Format("OAuth {0}=\"{1}\"",
                                                                     OAuthConstants.PARAM_OAUTH_REALM,
                                                                     OAuthConfigurationSection.Read().Realm));
      }
    }
  }
}