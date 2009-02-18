using System;
using System.Collections.Specialized;
using OAuth.MVC.Library;
using OAuth.MVC.Library.Interfaces;
using Rhino.Mocks;
using Xunit;

namespace OAuth.MVC.Tests
{
  namespace OAuthServiceSpecifications
  {
    public class when_service_is_asked_to_get_request : IUseFixture<MockRepository>
    {
      private IOAuthRequest request;

      [Fact]
      public void a_request_object_should_be_created()
      {
        Assert.NotNull(request);
      }

      public void SetFixture(MockRepository mocks)
      {
        var oAuthRepositoryMock = mocks.DynamicMock<IOAuthRepository>();
        IOAuthService oauthService = new OAuthService(oAuthRepositoryMock, null);
        request = oauthService.BuildRequest(new Uri("http://someserver.com/somepath"), "POST", new NameValueCollection(), OAuthConstants.EndPointType.RequestTokenRequest);
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