using System;

namespace OAuth.MVC.Client.Sample.Interfaces
{
  public interface IGetRequestTokenView
  {
    event EventHandler<EventArgs> GetRequestToken;
    string Output { get; set; }
    string ConsumerKey { get; }
    string ConsumerSecret { get; }
    string RequestTokenUrl { get; }
    string TokenSecret { get; set; }
    string Token { get; set; }
    bool NextEnabled { get; set; }
  }
}