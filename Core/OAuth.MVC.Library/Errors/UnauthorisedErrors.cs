namespace OAuth.MVC.Library.Errors
{
  public abstract class UnauthorisedError:OAuthRequestError
  {
    public override int ErrorResponseCode
    {
      get { return 401; }
    }
  }
  public class InvalidConsumerKeyError:UnauthorisedError
  {
    public override string ErrorMessage
    {
      get { return ErrorMessages.InvalidConsumerKey; }
    }
  }
  public class InvalidTokenError : UnauthorisedError
  {
    public override string ErrorMessage
    {
      get { return ErrorMessages.InvalidToken; }
    }
  }
  public class InvalidSignatureError : UnauthorisedError
  {
    public override string ErrorMessage
    {
      get { return ErrorMessages.InvalidSignature; }
    }
  }
  public class InvalidNonceError : UnauthorisedError
  {
    public override string ErrorMessage
    {
      get { return ErrorMessages.InvalidNonce; }
    }
  }
}