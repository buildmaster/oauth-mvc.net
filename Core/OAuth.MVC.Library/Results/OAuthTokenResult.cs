using System;
using System.Web.Mvc;
using OAuth.Core.Interfaces;


namespace OAuth.MVC.Library.Results
{
  /// <summary>
  /// Class used to send an OAuth token to the response.
  /// </summary>
  public class OAuthTokenResult:ActionResult
  {
    internal readonly IToken Token;

    /// <summary>
    /// Creates an ActionResult built around a particular OAuth token.
    /// </summary>
    /// <param name="token"></param>
    public OAuthTokenResult(IToken token)
    {
      Token = token;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      context.HttpContext.Response.Write(String.Format("oauth_token={0}&oauth_token_secret={1}", Token.Token, Token.TokenSecret));
    }
  }
}