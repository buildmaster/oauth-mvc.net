using System.Web.Mvc;
using OAuth.MVC.Library.Filters;

namespace OAuth.MVC.Sample.Controllers
{
  [HandleError]
  [OAuthSecured]
  public class HomeController : Controller
  {
   
    public ActionResult Index()
    {
      return new EmptyResult();
    }

    
  }
}
