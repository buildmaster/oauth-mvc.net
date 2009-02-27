
using System.Web.Mvc;

namespace OAuth.MVC.Library.Filters
{
  public class OAuthSecuredAttribute : ActionFilterAttribute, IAuthorizationFilter
  {
    


    public void OnAuthorization(AuthorizationContext filterContext)
    {
      
    }
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
      
    }
  }
}