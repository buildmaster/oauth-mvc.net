using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OAuth.Core;
using OAuth.Core.Interfaces;
using OAuth.MVC.Library.Filters;
using OAuth.MVC.Library.Results;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Xunit;

namespace OAuth.MVC.Tests.Filters
{
    // ReSharper disable InconsistentNaming
  namespace OAuthSecuredAttributeSpecifications
  {
    public class OAuthOnAuthContext:IUseFixture<MockRepository>
    {
      protected OAuthSecuredAttribute AuthFiliter;
      protected AuthorizationContext FilterContext;
      protected Exception Exception;
      protected IOAuthContextBuilder DefaultoAuthContextBuilder;
      protected IOAuthProvider DefaultoAuthPovider;
      protected IOAuthContext DefaultoAuthContext;

      public void SetFixture(MockRepository mocks)
      {
        AuthFiliter = new OAuthSecuredAttribute();
        DefaultoAuthContextBuilder = mocks.DynamicMock<IOAuthContextBuilder>();
        DefaultoAuthPovider = mocks.DynamicMock<IOAuthProvider>();
        var controllerBase = mocks.DynamicMock<ControllerBase>();
        var httpContext = mocks.DynamicMock<HttpContextBase>();
        var mockHttpRequest = mocks.DynamicMock<HttpRequestBase>();
        DefaultoAuthContext = mocks.DynamicMock<IOAuthContext>();

        var controllerContext = new ControllerContext(httpContext,new RouteData(),controllerBase);
        httpContext.Stub(context => context.Request).Return(mockHttpRequest);
        
        DefaultoAuthContextBuilder.Stub(contextBuilder => contextBuilder.FromHttpRequest(mockHttpRequest)).Return(OAuthContext);
        SetupExpectations();
        FilterContext = new AuthorizationContext(controllerContext);
        AuthFiliter.OAuthContextBuilder = OAuthContextBuilder;
        AuthFiliter.OAuthProvider = OAuthPovider;
        mocks.ReplayAll();
        Exception = Record.Exception(()=>AuthFiliter.OnAuthorization(FilterContext));
      }

      protected virtual void SetupExpectations()
      {
       
      }

      private IOAuthContext OAuthContext
      {
        get { return DefaultoAuthContext; }
      }

      protected virtual IOAuthContextBuilder OAuthContextBuilder
      {
        get { return DefaultoAuthContextBuilder; }
      }
      protected virtual IOAuthProvider OAuthPovider
      {
        get { return DefaultoAuthPovider; }
      }
    }

    public class given_a_valid_oauth_request:OAuthOnAuthContext
    {
      [Fact]
      public void result_should_not_be_set()
      {
        Assert.Null(FilterContext.Result);
      }      
    }
    public class given_an_invalid_oauth_request:OAuthOnAuthContext
    {
      [Fact]
      public void result_should_be_oauth_exception_result()
      {
        Assert.IsType<OAuthExceptionResult>(FilterContext.Result);
      }
      protected override void SetupExpectations()
      {
        
        
        DefaultoAuthPovider.Stub(provider => provider.AccessProtectedResourceRequest(DefaultoAuthContext)).Throw(
          new OAuthException());
      }
    }
    public class given_an_oauth_request_filter_isnt_setup_with_oauth_context_through_ioc:OAuthOnAuthContext
    {
      protected override IOAuthContextBuilder OAuthContextBuilder
      {
        get
        {
          return null;
        }
      }
      [Fact]
      public void null_reference_exception_should_be_thrown()
      {
        Assert.IsType<NullReferenceException>(Exception);
      }
    }
    public class given_an_oauth_request_filter_isnt_setup_with_oauth_provider_through_ioc : OAuthOnAuthContext
    {
      protected override IOAuthProvider OAuthPovider
      {
        get
        {
          return null;
        }
      }
      [Fact]
      public void null_reference_exception_should_be_thrown()
      {
        Assert.IsType<NullReferenceException>(Exception);
      }
    }


    public class OAuthOnResultExcecutedContext:IUseFixture<MockRepository>
    {
      private readonly NameValueCollection DefaultHeaders = new NameValueCollection();
      protected HttpResponseBase mockResponse;
      protected const string DefaultRealm = "http://testurl.com";
      protected IOAuthContext oauthContext;
      protected IOAuthContextBuilder contextBuilder;
      public void SetFixture(MockRepository mocks)
      {
        var securedFilter = new OAuthSecuredAttribute();
        var mockControllerBase = mocks.DynamicMock<ControllerBase>();
        var mockHttpContext = mocks.DynamicMock<HttpContextBase>();
        mockResponse = mocks.DynamicMock<HttpResponseBase>();
        contextBuilder = mocks.DynamicMock<IOAuthContextBuilder>();
        oauthContext = mocks.DynamicMock < IOAuthContext>();
        var httpRequest = mocks.DynamicMock<HttpRequestBase>();
        var controllerContext = new ControllerContext(mockHttpContext,new RouteData(),mockControllerBase);
        var filterContext = new ResultExecutedContext(controllerContext,new EmptyResult(),false,null);
        mockHttpContext.Stub(context => context.Response).Return(mockResponse);
        mockHttpContext.Stub(context => context.Request).Return(httpRequest);
        mockResponse.Stub(response => response.Headers).Return(Headers);
        contextBuilder.Stub(builder => builder.FromHttpRequest(httpRequest)).Return(oauthContext);
        oauthContext.Stub(context => context.Realm).Return(DefaultRealm);
        mocks.ReplayAll();
        securedFilter.OAuthContextBuilder = contextBuilder;
        securedFilter.OnResultExecuted(filterContext);
      }

      protected virtual NameValueCollection Headers
      {
        get { return DefaultHeaders; }
      }
    }
    public class given_an_oauth_request_sets_a_www_auth_header:OAuthOnResultExcecutedContext
    {
      private readonly NameValueCollection headers= new NameValueCollection {{"WWW-Authenticate","NTLM"}};

      protected override NameValueCollection Headers
      {
        get
        {
          return headers;
        }
      }
      [Fact]
      public void oauth_header_should_be_added_to_existing_header()
      {
        mockResponse.AssertWasCalled(response=>response.AddHeader("",""),options=>options.Constraints(Is.Equal("WWW-Authenticate"),Is.Equal("OAuth Realm=\""+DefaultRealm+"\"")));
      }
    }
    public class given_an_oauth_request_hasnt_set_a_www_auth_header : OAuthOnResultExcecutedContext
    {
      
      [Fact]
      public void oauth_header_should_be_added()
      {
        mockResponse.AssertWasCalled(response=>response.AddHeader("WWW-Authenticate",String.Format("OAuth Realm=\"{0}\"",DefaultRealm)));
      }
    }
  }
  // ReSharper restore InconsistentNaming
}