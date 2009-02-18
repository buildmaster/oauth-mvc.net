using OAuth.MVC.Library.Interfaces;

namespace OAuth.MVC.Library.Errors
{
  public abstract class OAuthRequestError:IOAuthRequestError
  {
    public abstract int ErrorResponseCode { get; }
    public abstract string ErrorMessage { get; }
  }
  public static class ErrorMessages
  {
    public const string InvalidToken = "Invalid / expired Token";
    public const string InvalidSignature = "Invalid signature";
    public const string InvalidNonce = "Invalid / used nonce";
    public const string InvalidConsumerKey = "Invalid Consumer Key";
    public const string UnsupportedParameter = "Unsupported parameter";
    public const string UnsupportedSignatureMethod = "Unsupported signature method";
    public const string MissingRequiredParameter = "Missing required parameter";
    public const string DuplicateOAuthParameter = "Duplicated OAuth Protocol Parameter";
  }
}