using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OAuth.Core.Signing;
using Rhino.Mocks;
using Xunit;

namespace OAuth.Web.Tests
{
    // ReSharper disable InconsistentNaming
  namespace OAuthWebRequestSignerSpecifications
  {
    
    public class signer_when_signer_is_asked_to_sign_web_request:IUseFixture<MockRepository>
    {
      private IOAuthWebRequest webRequest;
      private string setUri;
      const string url = "https://www.google.com/accounts/OAuthGetRequestToken?scope=";
      const string consumerKey = "dpf43f3p2l4k3l03";
      const string consumerSecret = "kd94hf93k423kf44";
      const string timestamp = "1236209805";
      const string nonce = "788ce10e1b88b62e57a75dd3cf7c8fef";
      const string signature = "leyGd1u9SwdDP7189awXNOQoh%2Bo%3D";
      [Fact]
      public void url_should_contain_signature()
      {
        Assert.Contains(String.Format("oauth_signature={0}",signature),setUri);
      }
      [Fact]
      public void should_keep_existing_variables()
      {
        Assert.Contains("scope=&",setUri);
      }
      [Fact]
      public void url_query_string_should_contain_consumer_key()
      {
        Assert.Contains(string.Format("oauth_consumer_key={0}",consumerKey),new Uri(setUri).Query);
      }
      [Fact]
      public void url_query_string_should_contain_timestamp()
      {
        Assert.Contains(string.Format("oauth_timestamp={0}", timestamp), new Uri(setUri).Query);
      }
      [Fact]
      public void url_query_string_should_not_conatain_token()
      {
        Assert.DoesNotContain("oauth_token=", new Uri(setUri).Query);
      }
      [Fact]
      public void url_query_string_should_not_conatain_consumer_secret()
      {
        Assert.DoesNotContain(consumerSecret, new Uri(setUri).Query);
      }
      [Fact]
      public void url_query_string_should_contain_nonce()
      {
        Assert.Contains(string.Format("oauth_nonce={0}", nonce), new Uri(setUri).Query);
      }
      [Fact]
      public void url_should_be_to_correct_address()
      {
        Assert.True(setUri.StartsWith(url));
      }
      public void SetFixture(MockRepository mocks)
      {
        var signer = new OAuthWebRequestSigner(new HmacSha1SignatureGenerator());
        

        webRequest = mocks.DynamicMock<IOAuthWebRequest>();
        var mockNonceGenerator =mocks.DynamicMock<INonceGenerator>();
        var mockTimestampGenerator = mocks.DynamicMock<ITimestampGenerator>();
        var consumerToken = new OAuthToken {Token = consumerKey, TokenSecret = consumerSecret};
        mockNonceGenerator.Stub(nonceGen => nonceGen.Generate()).Return(nonce);
        mockTimestampGenerator.Stub(timestampGen => timestampGen.Generate()).Return(timestamp);
        webRequest.Stub(request => request.ConsumerToken).Return(consumerToken);
        webRequest.Stub(request=>request.NonceGenerator).Return(mockNonceGenerator);
        webRequest.Stub(request=>request.TimestampGenerator).Return(mockTimestampGenerator);
        webRequest.Stub(request => request.RequestUri).Return(new Uri(url));
        webRequest.Stub(request => request.Method).Return("GET");
        webRequest.Stub(request => request.SetUri(string.Empty)).IgnoreArguments().Do(
          (Action<string>) (uri => setUri = uri));
        mocks.ReplayAll();

        signer.SignWebRequest(webRequest);
      }
      
    }
    public class signer_when_signer_is_asked_to_sign_web_request_with_form_vars : IUseFixture<MockRepository>
    {
      private IOAuthWebRequest webRequest;
      private string setUri;
      private readonly List<KeyValuePair<string, string>> Form = new List<KeyValuePair<string, string>>(new[]{new KeyValuePair<string, string>("scope","")});
      const string url = "https://www.google.com/accounts/OAuthGetRequestToken";
      const string consumerKey = "dpf43f3p2l4k3l03";
      const string consumerSecret = "kd94hf93k423kf44";
      const string timestamp = "1236209805";
      const string nonce = "788ce10e1b88b62e57a75dd3cf7c8fef";
      const string signature = "YLfUOkBC8R8jai6DJcaKNYRz9yg=";
      
      [Fact]
      public void url_should_contain_signature()
      {
        Assert.True(Form.Exists(kvp=>kvp.Key=="oauth_signature"&&kvp.Value==signature));
      }
      [Fact]
      public void should_keep_existing_variables_in_the_form()
      {
        Assert.True(Form.Exists(kvp=>kvp.Key=="scope"&&kvp.Value==string.Empty));
      }
      [Fact]
      public void the_consumer_key_should_be_transmitted_via_form()
      {
        Assert.DoesNotContain(string.Format("oauth_consumer_key={0}", consumerKey), new Uri(setUri).Query);
        Assert.True(Form.Exists(kvp => kvp.Key == "oauth_consumer_key" && kvp.Value == consumerKey));
      }
      [Fact]
      public void timestamp_should_be_transmitted_via_form()
      {
        Assert.DoesNotContain(string.Format("oauth_timestamp={0}", timestamp), new Uri(setUri).Query);
        Assert.True(Form.Exists(kvp => kvp.Key == "oauth_timestamp" && kvp.Value == timestamp));
      }
      [Fact]
      public void url_query_string_nor_form_should_not_conatain_token()
      {
        Assert.DoesNotContain("oauth_token=", new Uri(setUri).Query);
        Assert.False(Form.Exists(kvp => kvp.Key == "oauth_token"));
      }
      [Fact]
      public void url_query_string_nor_form_should_not_conatain_consumer_secret()
      {
        Assert.DoesNotContain(consumerSecret, new Uri(setUri).Query);
        Assert.False(Form.Exists(kvp=>kvp.Value == consumerSecret));
      }
      [Fact]
      public void nonce_should_be_transmitted_via_form()
      {
        Assert.DoesNotContain(string.Format("oauth_nonce={0}", nonce), new Uri(setUri).Query);
        Assert.True(Form.Exists(kvp => kvp.Key == "oauth_nonce" && kvp.Value == nonce));
      }
      [Fact]
      public void url_should_be_to_correct_address()
      {
        Assert.True(setUri.StartsWith(url));
      }
      public void SetFixture(MockRepository mocks)
      {
        var signer = new OAuthWebRequestSigner(new HmacSha1SignatureGenerator());


        webRequest = mocks.DynamicMock<IOAuthWebRequest>();
        var mockNonceGenerator = mocks.DynamicMock<INonceGenerator>();
        var mockTimestampGenerator = mocks.DynamicMock<ITimestampGenerator>();
        var consumerToken = new OAuthToken { Token = consumerKey, TokenSecret = consumerSecret };
        mockNonceGenerator.Stub(nonceGen => nonceGen.Generate()).Return(nonce);
        mockTimestampGenerator.Stub(timestampGen => timestampGen.Generate()).Return(timestamp);
        webRequest.Stub(request => request.ConsumerToken).Return(consumerToken);
        webRequest.Stub(request => request.NonceGenerator).Return(mockNonceGenerator);
        webRequest.Stub(request => request.TimestampGenerator).Return(mockTimestampGenerator);
        webRequest.Stub(request => request.RequestUri).Return(new Uri(url));
        webRequest.Stub(request => request.Method).Return("POST");
        webRequest.Stub(request => request.SetUri(string.Empty)).IgnoreArguments().Do(
          (Action<string>)(uri => setUri = uri));
        webRequest.Stub(request => request.Form).Return(Form);
        mocks.ReplayAll();

        signer.SignWebRequest(webRequest);
      }

    }
    public class signer_when_signer_is_asked_to_sign_web_request_with_token : IUseFixture<MockRepository>
    {
      private IOAuthWebRequest webRequest;
      private string setUri;
      const string url = "https://www.google.com/accounts/OAuthGetRequestToken?scope=";
      const string consumerKey = "dpf43f3p2l4k3l03";
      const string consumerSecret = "kd94hf93k423kf44";
      const string timestamp = "1236209805";
      const string nonce = "788ce10e1b88b62e57a75dd3cf7c8fef";
      const string signature = "ZVCY25%2B8sZd6pQGhm8TwqB1MzF0%3D";
      private const string requestTokenKey = "ad180jjd733klru7";
      private const string requestTokenSecret = "0685bd9184jfhq22";
      [Fact]
      public void url_should_contain_signature()
      {
        Assert.Contains(String.Format("oauth_signature={0}", signature), setUri);
      }
      [Fact]
      public void should_keep_existing_variables()
      {
        Assert.Contains("scope=&", setUri);
      }
      [Fact]
      public void url_query_string_should_contain_consumer_key()
      {
        Assert.Contains(string.Format("oauth_consumer_key={0}", consumerKey), new Uri(setUri).Query);
      }
      [Fact]
      public void url_query_string_should_contain_timestamp()
      {
        Assert.Contains(string.Format("oauth_timestamp={0}", timestamp), new Uri(setUri).Query);
      }
      [Fact]
      public void url_query_string_should_not_conatain_token()
      {
        Assert.Contains(string.Format("oauth_token={0}",requestTokenKey), new Uri(setUri).Query);
      }
      [Fact]
      public void url_query_string_should_not_conatain_consumer_secret()
      {
        Assert.DoesNotContain(consumerSecret, new Uri(setUri).Query);
      }
      [Fact]
      public void url_query_string_should_contain_nonce()
      {
        Assert.Contains(string.Format("oauth_nonce={0}", nonce), new Uri(setUri).Query);
      }
      [Fact]
      public void url_should_be_to_correct_address()
      {
        Assert.True(setUri.StartsWith(url));
      }
      public void SetFixture(MockRepository mocks)
      {
        var signer = new OAuthWebRequestSigner(new HmacSha1SignatureGenerator());
        webRequest = mocks.DynamicMock<IOAuthWebRequest>();
        var mockNonceGenerator = mocks.DynamicMock<INonceGenerator>();
        var mockTimestampGenerator = mocks.DynamicMock<ITimestampGenerator>();
        var consumerToken = new OAuthToken { Token = consumerKey, TokenSecret = consumerSecret };
        var requestToken = new OAuthToken { Token = requestTokenKey, TokenSecret = requestTokenSecret };
        mockNonceGenerator.Stub(nonceGen => nonceGen.Generate()).Return(nonce);
        mockTimestampGenerator.Stub(timestampGen => timestampGen.Generate()).Return(timestamp);
        webRequest.Stub(request => request.ConsumerToken).Return(consumerToken);
        webRequest.Stub(request => request.NonceGenerator).Return(mockNonceGenerator);
        webRequest.Stub(request => request.TimestampGenerator).Return(mockTimestampGenerator);
        webRequest.Stub(request => request.RequestUri).Return(new Uri(url));
        webRequest.Stub(request => request.Method).Return("GET");
        webRequest.Stub(request => request.Token).Return(requestToken);
        
        webRequest.Stub(request => request.SetUri(string.Empty)).IgnoreArguments().Do(
          (Action<string>)(uri => setUri = uri));
        mocks.ReplayAll();

        signer.SignWebRequest(webRequest);
      }

    }
    public class signer_when_signer_is_asked_to_sign_web_request_with_form_vars_with_token : IUseFixture<MockRepository>
    {
      private IOAuthWebRequest webRequest;
      private string setUri;
      private readonly List<KeyValuePair<string, string>> Form = new List<KeyValuePair<string, string>>(new[] { new KeyValuePair<string, string>("scope", "") });
      const string url = "https://www.google.com/accounts/OAuthGetRequestToken";
      const string consumerKey = "dpf43f3p2l4k3l03";
      const string consumerSecret = "kd94hf93k423kf44";
      const string timestamp = "1236209805";
      const string nonce = "788ce10e1b88b62e57a75dd3cf7c8fef";
      const string signature = "lm9WewpOaP17tgsmNONhnYt1oaA=";
      private const string requestTokenKey = "ad180jjd733klru7";
      private const string requestTokenSecret = "0685bd9184jfhq22";

      [Fact]
      public void url_should_contain_signature()
      {
        Assert.True(Form.Exists(kvp => kvp.Key == "oauth_signature" && kvp.Value == signature));
      }
      [Fact]
      public void should_keep_existing_variables_in_the_form()
      {
        Assert.True(Form.Exists(kvp => kvp.Key == "scope" && kvp.Value == string.Empty));
      }
      [Fact]
      public void the_consumer_key_should_be_transmitted_via_form()
      {
        Assert.DoesNotContain(string.Format("oauth_consumer_key={0}", consumerKey), new Uri(setUri).Query);
        Assert.True(Form.Exists(kvp => kvp.Key == "oauth_consumer_key" && kvp.Value == consumerKey));
      }
      [Fact]
      public void timestamp_should_be_transmitted_via_form()
      {
        Assert.DoesNotContain(string.Format("oauth_timestamp={0}", timestamp), new Uri(setUri).Query);
        Assert.True(Form.Exists(kvp => kvp.Key == "oauth_timestamp" && kvp.Value == timestamp));
      }
      [Fact]
      public void url_query_string_nor_form_should_not_conatain_token()
      {
        Assert.DoesNotContain("oauth_token=", new Uri(setUri).Query);
        Assert.True(Form.Exists(kvp => kvp.Key == "oauth_token"&&kvp.Value==requestTokenKey));
      }
      [Fact]
      public void url_query_string_nor_form_should_not_conatain_consumer_secret()
      {
        Assert.DoesNotContain(consumerSecret, new Uri(setUri).Query);
        Assert.False(Form.Exists(kvp => kvp.Value == consumerSecret));
      }
      [Fact]
      public void nonce_should_be_transmitted_via_form()
      {
        Assert.DoesNotContain(string.Format("oauth_nonce={0}", nonce), new Uri(setUri).Query);
        Assert.True(Form.Exists(kvp => kvp.Key == "oauth_nonce" && kvp.Value == nonce));
      }
      [Fact]
      public void url_should_be_to_correct_address()
      {
        Assert.True(setUri.StartsWith(url));
      }
      public void SetFixture(MockRepository mocks)
      {
        var signer = new OAuthWebRequestSigner(new HmacSha1SignatureGenerator());


        webRequest = mocks.DynamicMock<IOAuthWebRequest>();
        var mockNonceGenerator = mocks.DynamicMock<INonceGenerator>();
        var mockTimestampGenerator = mocks.DynamicMock<ITimestampGenerator>();
        var consumerToken = new OAuthToken { Token = consumerKey, TokenSecret = consumerSecret };
        mockNonceGenerator.Stub(nonceGen => nonceGen.Generate()).Return(nonce);
        mockTimestampGenerator.Stub(timestampGen => timestampGen.Generate()).Return(timestamp);
        var requestToken = new OAuthToken { Token = requestTokenKey, TokenSecret = requestTokenSecret };
        webRequest.Stub(request => request.ConsumerToken).Return(consumerToken);
        webRequest.Stub(request => request.NonceGenerator).Return(mockNonceGenerator);
        webRequest.Stub(request => request.TimestampGenerator).Return(mockTimestampGenerator);
        webRequest.Stub(request => request.Token).Return(requestToken);
        webRequest.Stub(request => request.RequestUri).Return(new Uri(url));
        webRequest.Stub(request => request.Method).Return("POST");
        webRequest.Stub(request => request.SetUri(string.Empty)).IgnoreArguments().Do(
          (Action<string>)(uri => setUri = uri));
        webRequest.Stub(request => request.Form).Return(Form);
        mocks.ReplayAll();

        signer.SignWebRequest(webRequest);
      }

    }
    public class normalize_url_specifications
    {
      [Fact]
      public void when_url_is_plain_standard_http_not_specifying_port()
      {
        var url = "http://www.someurl.com";
        Assert.Equal("http://www.someurl.com",OAuthWebRequestSigner.NormalizeUri(new Uri(url)));
      }
      [Fact]
      public void when_url_is_uppercase()
      {
        var url = "HTTP://www.someurl.com";
        Assert.Equal("http://www.someurl.com", OAuthWebRequestSigner.NormalizeUri(new Uri(url)));
      }
      [Fact]
      public void when_url_is_http_over_non_standard_port()
      {
        var url = "HTTP://www.someurl.com:8080";
        Assert.Equal("http://www.someurl.com:8080", OAuthWebRequestSigner.NormalizeUri(new Uri(url)));
      }
      [Fact]
      public void when_including_query_string()
      {
        var url = "HTTP://www.someurl.com/?sometoken=somevalue";
        Assert.Equal("http://www.someurl.com", OAuthWebRequestSigner.NormalizeUri(new Uri(url)));
      }
      [Fact]
      public void when_trailing_slash()
      {
        var url = "HTTP://www.someurl.com/";
        Assert.Equal("http://www.someurl.com", OAuthWebRequestSigner.NormalizeUri(new Uri(url)));
      }
    }
    public class get_query_string_parameters
    {
      [Fact]
      public void given_no_parameters_in_url()
      {
        var url = new Uri("http://someplace.com/somepath?token=value");
        var queryStringParams = OAuthWebRequestSigner.GetQueryStringParameters(url);
        Assert.Equal(1,queryStringParams.Count());
        Assert.NotNull(queryStringParams.FirstOrDefault(kvp=>kvp.Key=="token"&&kvp.Value=="value"));
      }
      [Fact]
      public void should_return_decoded_paramter_values()
      {
        var value = "this' shdo*& get encoded";
        var url = new Uri("http://someplace.com/somepath?token=value&messy="+HttpUtility.UrlEncode(value));
        var queryStringParams = OAuthWebRequestSigner.GetQueryStringParameters(url);
        Assert.Equal(2,queryStringParams.Count());
        Assert.NotNull(queryStringParams.FirstOrDefault(kvp=>kvp.Key=="messy"&&kvp.Value==value));
      }
    }
  }
  // ReSharper restore InconsistentNaming
  
}