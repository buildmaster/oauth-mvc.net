using System;
using System.Web.Mvc;
using System.Linq;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider;
using Ninject.Core;
using OAuth.MVC.Library.Results;

namespace OAuth.MVC.Library.Filters
{
  public class OAuthSecuredAttribute : ActionFilterAttribute, IAuthorizationFilter
  {

    [Inject]
    public IOAuthContextBuilder OAuthContextBuilder { get; set; }
    [Inject]
    public IOAuthProvider OAuthProvider { get; set; }
    public virtual void OnAuthorization(AuthorizationContext filterContext)
    {
      if(OAuthContextBuilder==null)
        throw new NullReferenceException("OAuthContextBuilder wasn't set in the Authorisation filter, please use an IOC container to do this");
      if(OAuthProvider==null)
        throw new NullReferenceException("OAuthProvider wasn't set in the Authorisation filter, please use an IOC container to do this");
      try
      {
        var context = OAuthContextBuilder.FromHttpRequest(filterContext.HttpContext.Request);
        OAuthProvider.AccessProtectedResourceRequest(context);
      }
      catch (OAuthException ex)
      {

        filterContext.Result = new OAuthExceptionResult(ex);
      }

    }
    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      var context = OAuthContextBuilder.FromHttpRequest(filterContext.HttpContext.Request);
      var response = filterContext.HttpContext.Response;
      var header = string.Format("OAuth Realm=\"{0}\"", context.Realm);
      response.AddHeader("WWW-Authenticate",header);
    }
  }
}