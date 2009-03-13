using System;
using System.Collections.Generic;
using System.Net;

namespace OAuth.Web
{
  public interface IOAuthWebRequest
  {
    INonceGenerator NonceGenerator { get; set; }
    ITimestampGenerator TimestampGenerator { get; set; }
    IOAuthToken ConsumerToken { get; set; }
    Uri RequestUri { get; }
    void SetUri(string newUri);
    string Method { get; set; }
    List<KeyValuePair<string, string>> Form { get; }
    WebHeaderCollection Headers { get; }
    IOAuthToken Token { get; set; }
    bool UseAuthorizationHeader { get; }
    IWebProxy Proxy { get; set; }
  }
}