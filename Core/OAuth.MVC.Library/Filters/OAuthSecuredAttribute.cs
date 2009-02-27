
using System.Web.Mvc;
using System.Linq;
namespace OAuth.MVC.Library.Filters
{
  public class OAuthSecuredAttribute : ActionFilterAttribute, IAuthorizationFilter,IResultFilter
  {
    


    public void OnAuthorization(AuthorizationContext filterContext)
    {
      
    }
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
      
    }
  }
}