namespace OAuth.MVC.Library.Interfaces
{
  public interface IToken
  {
    string Token { get; }
    string Secret { get; }
  }
}