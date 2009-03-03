using System.Web.Mvc;
using DevDefined.OAuth.Framework;
using OAuth.MVC.Library.Filters;

namespace OAuth.MVC.Sample.Controllers
{
  [HandleError]
  [OAuthSecured]
  public class HomeController : Controller
  {
    readonly TokenRepository repository;

    public HomeController(TokenRepository repository)
   {
     this.repository = repository;
   }

    public ActionResult Index(IOAuthContext context)
    {
      context.TokenSecret = repository.GetAccessToken(context.Token).TokenSecret;
      return Json(context);
    }
    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult PostToMe(IOAuthContext context)
    {
      return Json(Request.Form);
    }
    
  }
}
