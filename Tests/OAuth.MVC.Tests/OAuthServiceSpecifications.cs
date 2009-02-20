using System;
using System.Collections.Specialized;
using System.Web;
using OAuth.MVC.Library;
using OAuth.MVC.Library.Interfaces;
using Rhino.Mocks;
using Xunit;
using System.Linq;

namespace OAuth.MVC.Tests
{
  namespace OAuthServiceSpecifications
  {
    public class when_service_is_asked_to_get_request : IUseFixture<MockRepository>
    {
      private OAuthRequest request;
      private NameValueCollection parameters;
      private const string version = "1.0";
      private const string nonce = "4572616e48616d6d65724c61686176";
      private const string timestamp = "137131200";
      private const string signature = "wOJIO9A2W5mFwDgiDvZbTSMK/PY=";
      private const string signatureMethod = "HMAC-SHA1";
      private const string token = "ad180jjd733klru7";
      private const string consumerKey = "0685bd9184jfhq22";

      [Fact]
      public void a_request_object_should_be_created()
      {
        Assert.NotNull(request);
      }

      [Fact]
      public void oauth_consumer_key_param_should_be_read_from_header_but_should_not_be_added_to_parameters()
      {
        Assert.False(parameters.AllKeys.Contains(OAuthConstants.PARAM_CONSUMER_KEY));
        Assert.Equal(consumerKey,request.ConsumerKey);
      }
      [Fact]
      public void oauth_token_param_should_be_added_from_header_but_should_not_be_added_to_parameters()
      {
        Assert.False(parameters.AllKeys.Contains(OAuthConstants.PARAM_TOKEN));
        Assert.Equal(token, request.Token);
      }
      [Fact]
      public void signature_method_param_should_be_added_from_header_but_should_not_be_added_to_parameters()
      {
        Assert.False(parameters.AllKeys.Contains(OAuthConstants.PARAM_SIGNATURE_METHOD));
        Assert.Equal(OAuthBase.SignatureTypes.HMACSHA1, request.SignatureType);
      }
      [Fact]
      public void signature_param_should_be_added_from_header_but_should_not_be_added_to_parameters()
      {
        Assert.False(parameters.AllKeys.Contains(OAuthConstants.PARAM_SIGNATURE));
        Assert.Equal(signature,request.Signature);
      }
      [Fact]
      public void timestamp_param_should_be_added_from_header_but_should_not_be_added_to_parameters()
      {
        Assert.False(parameters.AllKeys.Contains(OAuthConstants.PARAM_TIMESTAMP));
        Assert.Equal(timestamp, request.TimeStamp);
      }
      [Fact]
      public void nonce_param_should_be_added_from_header_but_should_not_be_added_to_parameters()
      {
        Assert.False(parameters.AllKeys.Contains(OAuthConstants.PARAM_NONCE));
        Assert.Equal(nonce, request.Nonce);
      }
      [Fact]
      public void version_param_should_be_added_from_header_but_should_not_be_added_to_parameters()
      {
        Assert.False(parameters.AllKeys.Contains(OAuthConstants.PARAM_VERSION));
        
      }
      public void SetFixture(MockRepository mocks)
      {
        var oAuthRepositoryMock = mocks.DynamicMock<IOAuthRepository>();
        IOAuthService oauthService = new OAuthService(oAuthRepositoryMock, null);
        var mockConsumer = mocks.DynamicMock<IConsumer>();
        oAuthRepositoryMock.Stub(oauthRepository => oauthRepository.GetConsumer(consumerKey)).Return(mockConsumer);
        mockConsumer.Stub(consumer => consumer.TimeStamp).Return(137131100);
        mockConsumer.Stub(consumer => consumer.IsUsedNonce(timestamp, nonce)).Return(false);
        mockConsumer.Stub(consumer => consumer.SecretKey).Return("someSecret");
        var headers = new NameValueCollection
                        {{"Authorization",String.Format("OAuth realm=\"http://sp.example.com/\","+
                "oauth_consumer_key=\"{0}\","+
                "oauth_token=\"{1}\","+
                "oauth_signature_method=\"{2}\","+
                "oauth_signature=\"{3}\","+
                "oauth_timestamp=\"{4}\","+
                "oauth_nonce=\"{5}\","+
                "oauth_version=\"{6}\"",
                HttpUtility.UrlEncode(consumerKey),
                HttpUtility.UrlEncode(token),
                HttpUtility.UrlEncode(signatureMethod),
                HttpUtility.UrlEncode(signature),
                HttpUtility.UrlEncode(timestamp),
                HttpUtility.UrlEncode(nonce),
                HttpUtility.UrlEncode(version))}};
        parameters = new NameValueCollection();
        mocks.ReplayAll();

        request = (OAuthRequest)oauthService.BuildRequest(new Uri("http://someserver.com/somepath"), "POST", parameters, headers, OAuthConstants.EndPointType.RequestTokenRequest);
      }
    }
    public class when_service_is_asked_to_generate_a_request_token : IUseFixture<MockRepository>
    {
      private IRequestToken token;
      private IConsumer mockConsumer;

      [Fact]
      public void consumer_should_be_asked_to_save_token()
      {
        mockConsumer.AssertWasCalled(consumer => consumer.SaveRequestToken(token));
      }


      public void SetFixture(MockRepository mocks)
      {

        var oAuthRepositoryMock = mocks.DynamicMock<IOAuthRepository>();
        mockConsumer = mocks.DynamicMock<IConsumer>();
        var mockTokenGenerator = mocks.DynamicMock<ITokenGenerator>();
        token = mocks.DynamicMock<IRequestToken>();
        mockTokenGenerator.Stub(tokenGenerator => tokenGenerator.GenerateNewRequestToken()).Return(token);
        mocks.ReplayAll();
        IOAuthService oauthService = new OAuthService(oAuthRepositoryMock, mockTokenGenerator);
        token = oauthService.GenerateRequestToken(mockConsumer);

      }
    }
    public class when_service_is_asked_to_generate_an_access_token : IUseFixture<MockRepository>
    {
      private IAccessToken mockAccessToken;
      private IConsumer mockConsumer;
      private readonly Guid userID = Guid.NewGuid();

      [Fact]
      public void consumer_should_be_asked_to_save_token()
      {
        mockConsumer.AssertWasCalled(consumer => consumer.SaveAccessToken(mockAccessToken));
      }
      [Fact]
      public void user_id_should_be_set_on_access_token()
      {
        mockAccessToken.AssertWasCalled(token=>token.UserID=userID);
      }

      public void SetFixture(MockRepository mocks)
      {
        var oAuthRepositoryMock = mocks.DynamicMock<IOAuthRepository>();
        mockConsumer = mocks.DynamicMock<IConsumer>();
        var mockTokenGenerator = mocks.DynamicMock<ITokenGenerator>();
        mockAccessToken = mocks.DynamicMock<IAccessToken>();
        mockTokenGenerator.Stub(tokenGenerator => tokenGenerator.GenerateNewAccessToken()).Return(mockAccessToken);
        mocks.ReplayAll();
        IOAuthService oauthService = new OAuthService(oAuthRepositoryMock, mockTokenGenerator);
        mockAccessToken = oauthService.GenerateAccessToken(mockConsumer,userID);
      }
    }
    public class when_a_user_allows_access_to_a_request_token : IUseFixture<MockRepository>
    {
      private IOAuthRepository oAuthRepositoryMock;
      private IRequestToken tokenMock;
      readonly Guid userId = Guid.NewGuid();
      const string requestToken = "sa0e32039203sladk";
      [Fact]
      public void request_token_should_be_marked_as_authorized()
      {
        tokenMock.AssertWasCalled(token => token.IsAuthorized = true);
      }
      [Fact]
      public void request_token_user_id_should_be_set()
      {
        tokenMock.AssertWasCalled(token => token.UserID = userId);
      }

      public void SetFixture(MockRepository mocks)
      {
        oAuthRepositoryMock = mocks.DynamicMock<IOAuthRepository>();
        var tokenGeneratorMock = mocks.DynamicMock<ITokenGenerator>();
        tokenMock = mocks.DynamicMock<IRequestToken>();
        tokenMock.Stub(token => token.IsAuthorized).Return(false);
        oAuthRepositoryMock.Stub(repository => repository.GetExistingToken(requestToken)).Return(tokenMock);
        mocks.ReplayAll();

        var service = new OAuthService(oAuthRepositoryMock, tokenGeneratorMock);


        service.AuthorizeRequestToken(requestToken, userId);
      }
    }
    public class when_a_user_allows_access_to_a_request_token_that_is_already_authorized : IUseFixture<MockRepository>
    {
      private IOAuthRepository oAuthRepositoryMock;
      private IRequestToken tokenMock;
      readonly Guid userId = Guid.NewGuid();
      const string requestToken = "sa0e32039203sladk";
      [Fact]
      public void request_token_should_not_be_marked_as_authorized_again()
      {
        tokenMock.AssertWasNotCalled(token => token.IsAuthorized = true);
      }
      [Fact]
      public void request_token_user_id_should_not_be_re_set()
      {
        tokenMock.AssertWasNotCalled(token => token.UserID = userId);
      }

      public void SetFixture(MockRepository mocks)
      {
        oAuthRepositoryMock = mocks.DynamicMock<IOAuthRepository>();
        var tokenGeneratorMock = mocks.DynamicMock<ITokenGenerator>();
        tokenMock = mocks.DynamicMock<IRequestToken>();
        tokenMock.Stub(token => token.IsAuthorized).Return(true);
        oAuthRepositoryMock.Stub(repository => repository.GetExistingToken(requestToken)).Return(tokenMock);
        mocks.ReplayAll();

        var service = new OAuthService(oAuthRepositoryMock, tokenGeneratorMock);


        service.AuthorizeRequestToken(requestToken, userId);
      }
    }
  }
}