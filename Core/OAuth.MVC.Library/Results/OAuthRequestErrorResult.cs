using System.Web.Mvc;
using OAuth.MVC.Library.Interfaces;

namespace OAuth.MVC.Library.Results
{
  public class OAuthRequestErrorResult:ActionResult
  {
    private readonly IOAuthRequestError authRequestError;

    public OAuthRequestErrorResult(IOAuthRequestError authRequestError)
    {
      this.authRequestError = authRequestError;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      context.HttpContext.Response.StatusCode = authRequestError.ErrorResponseCode;
      context.HttpContext.Response.Write(authRequestError.ErrorMessage);
    }
  }
}