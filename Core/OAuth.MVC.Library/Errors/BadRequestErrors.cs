namespace OAuth.MVC.Library.Errors
{
  public abstract class BadRequestError:OAuthRequestError
  {
    public override int ErrorResponseCode
    {
      get { return 400; }
    }
    
  }
  public class UnsupportedSignatureMethodError:BadRequestError
  {
    public override string ErrorMessage
    {
      get { return ErrorMessages.UnsupportedSignatureMethod; }
    }
  }
  public class UnsuportedParameterError : BadRequestError
  {
    public override string ErrorMessage
    {
      get { return ErrorMessages.UnsupportedParameter; }
    }
  }
  public class MissingRequiredParameterError : BadRequestError
  {
    public override string ErrorMessage
    {
      get { return ErrorMessages.MissingRequiredParameter; }
    }
  }
  public class DuplicateOAuthProtocolParameterError : BadRequestError
  {
    public override string ErrorMessage
    {
      get { return ErrorMessages.DuplicateOAuthParameter; }
    }
  }
}