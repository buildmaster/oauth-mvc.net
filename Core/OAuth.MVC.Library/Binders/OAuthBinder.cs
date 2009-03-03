using System;
using System.Web.Mvc;
using DevDefined.OAuth.Framework;
using Ninject.Core;

namespace OAuth.MVC.Library.Binders
{
  public class OAuthBinder:IModelBinder
  {
    
    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      if (OAuthContextBuilder == null)
        throw new NullReferenceException("OAuthContextBinder was not set please use an IoC container to do this");
      if (bindingContext.ModelType.IsAssignableFrom(typeof(IOAuthContext)))
        return OAuthContextBuilder.FromHttpRequest(controllerContext.HttpContext.Request);
      return null;
    }
    [Inject]
    public IOAuthContextBuilder OAuthContextBuilder { get; set; }
  }
}