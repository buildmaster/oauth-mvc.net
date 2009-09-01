using System.Web.Mvc;
using OAuth.Core.Interfaces;
using OAuth.MVC.Library.Filters;

namespace OAuth.MVC.Sample.Controllers
{
  [HandleError]
  [OAuthSecured]
  public class HomeController : Controller
  {
    readonly TokenRepository _repository;

    public HomeController(TokenRepository repository)
   {
     _repository = repository;
   }

    public ActionResult Index(IOAuthContext context)
    {
      context.TokenSecret = _repository.GetAccessToken(context.Token).TokenSecret;
      return Json(context);
    }
    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult PostToMe(IOAuthContext context)
    {
      return Json(Request.Form);
    }
    
  }
}
