using System.Net;
using System.Web.Mvc;
using OAuth.Core;

namespace OAuth.MVC.Library.Results
{
  public class OAuthExceptionResult:ActionResult
  {
    private readonly OAuthException _exception;

    public OAuthExceptionResult(OAuthException exception)
    {
      _exception = exception;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      var response = context.HttpContext.Response;
      
      switch (_exception.Report.Problem)
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
      
      response.AddHeader("WWW-Authenticate",string.Format("OAuth Realm=\"{0}\"",_exception.Context.Realm));
      response.Write(_exception.Report.ToString());
    }
  }
}