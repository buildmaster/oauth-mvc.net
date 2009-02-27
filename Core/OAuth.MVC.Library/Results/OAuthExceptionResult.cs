using System.Net;
using System.Web.Mvc;
using DevDefined.OAuth.Framework;

namespace OAuth.MVC.Library.Results
{
  public class OAuthExceptionResult:ActionResult
  {
    private readonly OAuthException exception;

    public OAuthExceptionResult(OAuthException exception)
    {
      this.exception = exception;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      var response = context.HttpContext.Response;
      
      switch (exception.Report.Problem)
      {

        case OAuthProblems.ParameterRejected:
        case OAuthProblems.SignatureMethodRejected:
        case OAuthProblems.ParameterAbset:
          response.StatusCode = (int) HttpStatusCode.BadRequest;
          break;
        default:
          response.StatusCode = (int)HttpStatusCode.Unauthorized;
          break;
      }
      
      
      response.Write(exception.Report.ToString());
    }
  }
}