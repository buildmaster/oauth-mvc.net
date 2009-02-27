using System.Reflection;
using System.Web.Mvc;
using Codeclimber.Ninject.FilterInjector;
using System.Web.Routing;
using Ninject.Core;
using Ninject.Framework.Mvc;
using OAuth.MVC.Library.Controllers;

namespace OAuth.MVC.Sample
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : NinjectHttpApplication
  {
    protected override void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
          "Default",                                              // Route name
          "{controller}/{action}/{id}",                           // URL with parameters
          new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
      );
      
     
      
    }

    protected override IKernel CreateKernel()
    {

      return new StandardKernel(new AutoControllerModuleWithFilters(Assembly.GetExecutingAssembly(),typeof(OAuthController).Assembly));
    }

   
  }

 
}