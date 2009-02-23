using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OAuth.MVC.Library.Filters;
using OAuth.MVC.Library.Interfaces;

namespace OAuth.MVC.Sample.Controllers
{
  [HandleError]
  [OAuthSecured]
  public class HomeController : Controller
  {
   
    public ActionResult Index(IOAuthRequest request)
    {
      return Json(request);
    }

    public ActionResult About()
    {
      return View();
    }
  }
}
