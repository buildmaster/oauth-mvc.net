using System;
using System.Web.Mvc;
using DevDefined.OAuth.Framework;


namespace OAuth.MVC.Library.Results
{
  public class OAuthTokenResult:ActionResult
  {
    internal readonly IToken token;

    public OAuthTokenResult(IToken token)
    {
      this.token = token;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      context.HttpContext.Response.Write(String.Format("oauth_token={0}&oauth_token_secret={1}", token.Token, token.TokenSecret));
    }
  }
}