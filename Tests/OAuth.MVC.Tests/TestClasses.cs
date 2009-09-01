
using OAuth.Core.Interfaces;

namespace OAuth.MVC.Tests
{
  public class TestToken:IToken
  {
    public string ConsumerKey { get; set; }
    public string Realm { get; set; }
    public string TokenSecret { get; set; }
    public string Token { get; set; }
  }
}