using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider;
using OAuth.MVC.Library.Controllers;
using OAuth.MVC.Library.Results;
using Rhino.Mocks;
using Xunit;

namespace OAuth.MVC.Tests.Controllers
{
  namespace OAuthControllerSpecifications
  {
    public class OAuthGetRequestTokenContext:IUseFixture<MockRepository>
    {
      protected IOAuthProvider DefaultProvider;
      private IOAuthContextBuilder DefaultcontextBuilder;
      protected ActionResult result;
      protected IToken requestToken = new TestToken{Token="token",TokenSecret = "TokenSecret"};
      protected MockRepository mockRepository;
      protected IOAuthContext mockOAuthContext;

      public void SetFixture(MockRepository mocks)
      {
        mockRepository = mocks;
        DefaultProvider = mocks.DynamicMock<IOAuthProvider>();
        DefaultcontextBuilder = mocks.DynamicMock<IOAuthContextBuilder>();
        var httpRequest = mocks.DynamicMock<HttpRequestBase>();
        var httpContextMock = mocks.DynamicMock<HttpContextBase>();
        mockOAuthContext = mocks.DynamicMock<IOAuthContext>();
        DefaultcontextBuilder.Stub(contextBuilder => contextBuilder.FromHttpRequest(httpRequest)).Return(mockOAuthContext);
        DefaultProvider.Stub(provider => provider.GrantRequestToken(mockOAuthContext)).Return(requestToken);
        httpContextMock.Stub(httpcontext => httpcontext.Request).Return(httpRequest);
        var controller = new OAuthController(ContextBuilder,Provider);
        var controllerContext = new ControllerContext(httpContextMock, new RouteData(),controller);
        controller.ControllerContext = controllerContext;
        mocks.ReplayAll();
        result = controller.RequestToken();

      }

      protected virtual IOAuthContextBuilder ContextBuilder
      {
        get { return DefaultcontextBuilder; }
      }
      protected virtual IOAuthProvider Provider
      {
        get { return DefaultProvider; }
      }
    }
    public class a_valid_request:OAuthGetRequestTokenContext
    {
      [Fact]
      public void should_return_a_token_result()
      {
        Assert.IsType<OAuthTokenResult>(result);
      }

      [Fact]
      public void should_have_a_token()
      {
        Assert.Equal(requestToken,((OAuthTokenResult)result).token);
      }
    }
    public class an_invalid_request : OAuthGetRequestTokenContext
    {
      [Fact]
      public void should_return_an_exception_result()
      {
        Assert.IsType<OAuthExceptionResult>(result);
      }

      protected override IOAuthProvider Provider
      {
        get
        {
          DefaultProvider.BackToRecord(BackToRecordOptions.All);
          DefaultProvider.Stub(provider => provider.GrantRequestToken(mockOAuthContext)).Throw(new OAuthException());
          return DefaultProvider;
        }
      }
    
    }

    public class OAuthGetAccessTokenContext:IUseFixture<MockRepository>
    {
      protected IOAuthProvider DefaultProvider;
      private IOAuthContextBuilder DefaultcontextBuilder;
      protected ActionResult result;
      protected IToken requestToken = new TestToken{Token="token",TokenSecret = "TokenSecret"};
      protected MockRepository mockRepository;
      protected IOAuthContext mockOAuthContext;

      public void SetFixture(MockRepository mocks)
      {
        mockRepository = mocks;
        DefaultProvider = mocks.DynamicMock<IOAuthProvider>();
        DefaultcontextBuilder = mocks.DynamicMock<IOAuthContextBuilder>();
        var httpRequest = mocks.DynamicMock<HttpRequestBase>();
        var httpContextMock = mocks.DynamicMock<HttpContextBase>();
        mockOAuthContext = mocks.DynamicMock<IOAuthContext>();
        DefaultcontextBuilder.Stub(contextBuilder => contextBuilder.FromHttpRequest(httpRequest)).Return(mockOAuthContext);
        DefaultProvider.Stub(provider => provider.ExchangeRequestTokenForAccessToken(mockOAuthContext)).Return(requestToken);
        httpContextMock.Stub(httpcontext => httpcontext.Request).Return(httpRequest);
        var controller = new OAuthController(ContextBuilder,Provider);
        var controllerContext = new ControllerContext(httpContextMock, new RouteData(),controller);
        controller.ControllerContext = controllerContext;
        mocks.ReplayAll();
        result = controller.AccessToken();

      }

      protected virtual IOAuthContextBuilder ContextBuilder
      {
        get { return DefaultcontextBuilder; }
      }
      protected virtual IOAuthProvider Provider
      {
        get { return DefaultProvider; }
      }
    }
    public class a_valid_access_token_request : OAuthGetAccessTokenContext
    {
      [Fact]
      public void should_return_a_token_result()
      {
        Assert.IsType<OAuthTokenResult>(result);
      }

      [Fact]
      public void should_have_a_token()
      {
        Assert.Equal(requestToken,((OAuthTokenResult)result).token);
      }
    }
    public class an_invalid_access_token_request : OAuthGetAccessTokenContext
    {
      [Fact]
      public void should_return_an_exception_result()
      {
        Assert.IsType<OAuthExceptionResult>(result);
      }

      protected override IOAuthProvider Provider
      {
        get
        {
          DefaultProvider.BackToRecord(BackToRecordOptions.All);
          DefaultProvider.Stub(provider => provider.ExchangeRequestTokenForAccessToken(mockOAuthContext)).Throw(new OAuthException());
          return DefaultProvider;
        }
      }
    
    }

  }
}
