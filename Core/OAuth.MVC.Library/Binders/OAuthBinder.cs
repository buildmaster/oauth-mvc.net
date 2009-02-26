using System.Web.Mvc;
using Ninject.Core;
using OAuth.MVC.Library.Interfaces;

namespace OAuth.MVC.Library.Binders
{
  public class OAuthBinder:IModelBinder
  {
    [Inject]
    public IOAuthService OAuthService
    {
      get; set;
    }
    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var httpRequest = controllerContext.HttpContext.Request;
      var oauthRequest = OAuthService.BuildRequest(httpRequest.Url, httpRequest.HttpMethod, httpRequest.Params,httpRequest.Headers,
                                                   OAuthConstants.EndPointType.AccessRequest);
      if (bindingContext.ModelType==typeof(IConsumer))
      {
       
        if (oauthRequest.IsValid())
        {
          return oauthRequest.Consumer;
        }
      }
      else if(bindingContext.ModelType==typeof(IOAuthRequest))
      {        
        return oauthRequest;
      }
      return null;
    }
  }
}