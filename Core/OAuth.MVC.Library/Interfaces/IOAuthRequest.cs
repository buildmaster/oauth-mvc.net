namespace OAuth.MVC.Library.Interfaces
{
  public interface IOAuthRequest
  {
    bool IsValid();

    IConsumer Consumer { get; }
    IOAuthRequestError Error { get; }
    IRequestToken RequestToken { get; }
    IAccessToken AccessToken { get; }
  }
}