namespace OAuth.MVC.Library.Interfaces
{
  public interface IOAuthRequestError
  {
    int ErrorResponseCode { get; }
    string ErrorMessage { get; }
  }
}