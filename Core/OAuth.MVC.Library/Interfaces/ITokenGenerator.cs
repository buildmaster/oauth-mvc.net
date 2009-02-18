namespace OAuth.MVC.Library.Interfaces
{
  public interface ITokenGenerator
  {
    /// <summary>
    /// Generates a new request token.
    /// </summary>
    /// <returns></returns>
    IRequestToken GenerateNewRequestToken();

    IAccessToken GenerateNewAccessToken();
  }
}