using System;
using System.Net;
using Rhino.Mocks;
using Xunit;

namespace OAuth.Web.Tests
{
  public class OAuthHttpWebRequestSpecifications
  {
    private string actualUrl;

    [Fact]
    public void an_oauth_web_request_should_attach_oauth_parameter_without_token_if_not_set()
    {
      var mocks = new MockRepository();
      var mockWebRequestFactory= mocks.DynamicMock<IHttpWebRequestFactory>();
      const string baseRequestUri = @"https://photos.example.net/request_token";
      mockWebRequestFactory
        .Stub(factory => factory.Create(null)).IgnoreArguments()
        .Do((Func<string, WebResponse>) (url =>
                                           {
                                             actualUrl = url;
                                             return null;
                                           }));

      mocks.ReplayAll();
      
      var request = new OAuthHttpWebRequest(mockWebRequestFactory,null);
      request.SetUri(baseRequestUri);
      request.GetResponse();

      Assert.Equal("https://photos.example.net/request_token?oauth_consumer_key=dpf43f3p2l4k3l03&oauth_signature_method=PLAINTEXT&oauth_signature=kd94hf93k423kf44%26&oauth_timestamp=1191242090&oauth_nonce=hsu94j3884jdopsl&oauth_version=1.0",actualUrl);
    }

    
  }
}