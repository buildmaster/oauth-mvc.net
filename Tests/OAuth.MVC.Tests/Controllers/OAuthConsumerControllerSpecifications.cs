using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OAuth.MVC.Library;
using OAuth.MVC.Library.Controllers;
using OAuth.MVC.Library.Errors;
using OAuth.MVC.Library.Interfaces;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Xunit;

namespace OAuth.MVC.Tests.Controllers
{
  namespace OAuthConsumerControllerSpecifications
  {
    namespace GetRequestTokenSpecifications
    {
      public abstract class GetRequestContext : IUseFixture<MockRepository>
      {
        public abstract string Token { get; }
        public abstract string Secret { get; }
        protected string responseString;
        protected HttpResponseBase responseMock;
        protected IOAuthService oauthServiceMock;
        protected IConsumer consumerMock;
        protected IRequestToken requestTokenMock;

        public void SetFixture(MockRepository mocks)
        {
          mocks.BackToRecordAll();

          var httpContextMock = mocks.DynamicMock<HttpContextBase>();
          responseMock = mocks.DynamicMock<HttpResponseBase>();
          var httpRequestMock = mocks.DynamicMock<HttpRequestBase>();
          oauthServiceMock = mocks.DynamicMock<IOAuthService>();
          var oauthRequestMock = mocks.DynamicMock<IOAuthRequest>();
          consumerMock = mocks.DynamicMock<IConsumer>();
          requestTokenMock = mocks.DynamicMock<IRequestToken>();
          httpContextMock.Stub(httpContext => httpContext.Response).Return(responseMock);
          responseMock.Stub(response => response.Write(string.Empty))
            .Constraints(Is.TypeOf<string>())
            .Do((Action<string>)(s => responseString = s));
          var parameters = new NameValueCollection();
          const string httpMethod = "POST";
          var url = new Uri("http://someservice.com/get_request");

          httpContextMock.Stub(context => context.Request).Return(httpRequestMock);

          httpRequestMock.Stub(request => request.HttpMethod).Return(httpMethod);
          httpRequestMock.Stub(request => request.Params).Return(parameters);
          httpRequestMock.Stub(request => request.Url).Return(url);


          oauthServiceMock.Stub(oauthService => oauthService.BuildRequest(url, httpMethod, parameters.ToPairs(), OAuthConstants.EndPointType.RequestTokenRequest)).Return(oauthRequestMock);
          oauthRequestMock.Stub(oauthRequest => oauthRequest.Consumer).Return(consumerMock);
          oauthRequestMock.Stub(oauthRequest => oauthRequest.IsValid()).Return(ValidRequest);

          oauthServiceMock.Stub(oauthService => oauthService.GenerateRequestToken(consumerMock)).Return(requestTokenMock);
          requestTokenMock.Stub(requestToken => requestToken.Token).Return(Token);
          requestTokenMock.Stub(requestToken => requestToken.Secret).Return(Secret);
          oauthRequestMock.Stub(request => request.Error).Return(RequestError);


          mocks.ReplayAll();
          var OAuthController = new OAuthController(oauthServiceMock);
          var controllerContext = new ControllerContext(httpContextMock, new RouteData(), OAuthController);
          OAuthController.ControllerContext = controllerContext;





          var result = OAuthController.RequestToken();
          result.ExecuteResult(controllerContext);

        }

        protected abstract IOAuthRequestError RequestError { get; }

        protected abstract bool ValidRequest
        {
          get;
        }
      }
      public class when_a_known_provider_asks_for_a_request_token : GetRequestContext
      {

        [Fact]
        public void response_should_be_oauth_token_and_secret()
        {
          Assert.Equal(String.Format("oauth_token={0}&oauth_token_secret={1}", Token, Secret), responseString);
        }
        [Fact]
        public void consumer_is_asked_to_save_token()
        {
          oauthServiceMock.AssertWasCalled(service => service.GenerateRequestToken(consumerMock));
        }

        public override string Token
        {
          get { return "hh5s93j4hdidpola"; }
        }

        public override string Secret
        {
          get { return "hdhd0244k9j7ao03"; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return null; }
        }

        protected override bool ValidRequest
        {
          get { return true; }
        }
      }
      public class when_a_client_specifies_an_unsupported_signature_method : GetRequestContext
      {
        [Fact]
        public void http_response_code_should_be_400()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 400);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Unsupported signature method", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new UnsupportedSignatureMethodError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_specifies_an_unsupported_parameter : GetRequestContext
      {
        [Fact]
        public void http_response_code_should_be_400()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 400);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Unsupported parameter", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new UnsuportedParameterError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_misses_an_oauth_parameter : GetRequestContext
      {
        [Fact]
        public void http_response_code_should_be_400()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 400);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Missing required parameter", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new MissingRequiredParameterError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_duplicates_an_oauth_parameter : GetRequestContext
      {
        [Fact]
        public void http_response_code_should_be_400()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 400);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Duplicated OAuth Protocol Parameter", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new DuplicateOAuthProtocolParameterError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_specifies_an_invalid_consumer_key : GetRequestContext
      {
        [Fact]
        public void http_response_code_should_be_401()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 401);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Invalid Consumer Key", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new InvalidConsumerKeyError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_specifies_an_invalid_token : GetRequestContext
      {
        [Fact]
        public void http_response_code_should_be_401()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 401);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Invalid / expired Token", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new InvalidTokenError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_specifies_an_invalid_signature : GetRequestContext
      {
        [Fact]
        public void http_response_code_should_be_401()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 401);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Invalid signature", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new InvalidSignatureError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_specifies_an_invalid_nonce : GetRequestContext
      {
        [Fact]
        public void http_response_code_should_be_401()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 401);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Invalid / used nonce", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new InvalidNonceError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
    }
    namespace GetAccessTokenSpecifications
    {
      public abstract class GetAccessTokenContext : IUseFixture<MockRepository>
      {
        public abstract string Token { get; }
        public abstract string Secret { get; }
        protected string responseString;
        protected HttpResponseBase responseMock;
        protected IOAuthService oauthServiceMock;
        protected IConsumer consumerMock;
        protected readonly Guid userID = Guid.NewGuid();

        public void SetFixture(MockRepository mocks)
        {
          mocks.BackToRecordAll();

          var httpContextMock = mocks.DynamicMock<HttpContextBase>();
          responseMock = mocks.DynamicMock<HttpResponseBase>();
          var httpRequestMock = mocks.DynamicMock<HttpRequestBase>();
          oauthServiceMock = mocks.DynamicMock<IOAuthService>();
          var oauthRequestMock = mocks.DynamicMock<IOAuthRequest>();
          consumerMock = mocks.DynamicMock<IConsumer>();
          var requestTokenMock = mocks.DynamicMock<IRequestToken>();
          var accessTokenMock = mocks.DynamicMock<IAccessToken>();
          httpContextMock.Stub(httpContext => httpContext.Response).Return(responseMock);
          responseMock.Stub(response => response.Write(string.Empty))
            .Constraints(Is.TypeOf<string>())
            .Do((Action<string>)(s => responseString = s));
          var parameters = new NameValueCollection();
          const string httpMethod = "POST";
          var url = new Uri("http://someservice.com/get_access_token");

          httpContextMock.Stub(context => context.Request).Return(httpRequestMock);

          httpRequestMock.Stub(request => request.HttpMethod).Return(httpMethod);
          httpRequestMock.Stub(request => request.Params).Return(parameters);
          httpRequestMock.Stub(request => request.Url).Return(url);


          oauthServiceMock.Stub(oauthService => oauthService.BuildRequest(url, httpMethod, parameters.ToPairs(), OAuthConstants.EndPointType.AccessTokenRequest)).Return(oauthRequestMock);
          oauthRequestMock.Stub(oauthRequest => oauthRequest.Consumer).Return(consumerMock);
          oauthRequestMock.Stub(oauthRequest => oauthRequest.IsValid()).Return(ValidRequest);
          oauthRequestMock.Stub(oauthRequest => oauthRequest.RequestToken).Return(requestTokenMock);
          
          consumerMock.Stub(consumer => consumer.GetRequestToken(Token)).Return(requestTokenMock);


          oauthServiceMock.Stub(oauthService => oauthService.GenerateAccessToken(consumerMock,userID)).Return(accessTokenMock);
          accessTokenMock.Stub(accessToken => accessToken.Token).Return(Token);
          accessTokenMock.Stub(accessToken => accessToken.Secret).Return(Secret);
          requestTokenMock.Stub(requestToken => requestToken.UserID).Return(userID);

          oauthRequestMock.Stub(request => request.Error).Return(RequestError);


          mocks.ReplayAll();
          var OAuthController = new OAuthController(oauthServiceMock);
          var controllerContext = new ControllerContext(httpContextMock, new RouteData(), OAuthController);
          OAuthController.ControllerContext = controllerContext;





          var result = OAuthController.AccessToken();
          result.ExecuteResult(controllerContext);

        }

        protected abstract IOAuthRequestError RequestError { get; }

        protected abstract bool ValidRequest
        {
          get;
        }
      }
      public class when_a_client_asks_for_an_access_token_with_a_valid_access_token_request : GetAccessTokenContext
      {
        [Fact]
        public void response_should_be_oauth_token_and_secret()
        {
          Assert.Equal(String.Format("oauth_token={0}&oauth_token_secret={1}", Token, Secret), responseString);
        }
        [Fact]
        public void service_should_be_asked_to_generate_access_token()
        {
          oauthServiceMock.AssertWasCalled(service=>service.GenerateAccessToken(consumerMock,userID));
        }
        public override string Token
        {
          get { return @"token"; }
        }

        public override string Secret
        {
          get { return @"tokensecret"; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return null; }
        }

        protected override bool ValidRequest
        {
          get { return true;}
        }
      }
      public class when_a_client_specifies_an_unsupported_signature_method : GetAccessTokenContext
      {
        [Fact]
        public void http_response_code_should_be_400()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 400);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Unsupported signature method", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new UnsupportedSignatureMethodError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_specifies_an_unsupported_parameter : GetAccessTokenContext
      {
        [Fact]
        public void http_response_code_should_be_400()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 400);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Unsupported parameter", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new UnsuportedParameterError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_misses_an_oauth_parameter : GetAccessTokenContext
      {
        [Fact]
        public void http_response_code_should_be_400()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 400);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Missing required parameter", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new MissingRequiredParameterError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_duplicates_an_oauth_parameter : GetAccessTokenContext
      {
        [Fact]
        public void http_response_code_should_be_400()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 400);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Duplicated OAuth Protocol Parameter", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new DuplicateOAuthProtocolParameterError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_specifies_an_invalid_consumer_key : GetAccessTokenContext
      {
        [Fact]
        public void http_response_code_should_be_401()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 401);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Invalid Consumer Key", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new InvalidConsumerKeyError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_specifies_an_invalid_token : GetAccessTokenContext
      {
        [Fact]
        public void http_response_code_should_be_401()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 401);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Invalid / expired Token", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new InvalidTokenError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_specifies_an_invalid_signature : GetAccessTokenContext
      {
        [Fact]
        public void http_response_code_should_be_401()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 401);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Invalid signature", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new InvalidSignatureError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
      public class when_a_client_specifies_an_invalid_nonce : GetAccessTokenContext
      {
        [Fact]
        public void http_response_code_should_be_401()
        {
          responseMock.AssertWasCalled(response => response.StatusCode = 401);
        }
        [Fact]
        public void correct_message_should_be_sent()
        {
          Assert.Equal("Invalid / used nonce", responseString);
        }
        [Fact]
        public void request_token_should_not_Be_generated()
        {
          oauthServiceMock.AssertWasNotCalled(service => service.GenerateRequestToken(consumerMock));
        }
        public override string Token
        {
          get { return ""; }
        }

        public override string Secret
        {
          get { return ""; }
        }

        protected override IOAuthRequestError RequestError
        {
          get { return new InvalidNonceError(); }
        }

        protected override bool ValidRequest
        {
          get { return false; }
        }
      }
    }
  }
}
