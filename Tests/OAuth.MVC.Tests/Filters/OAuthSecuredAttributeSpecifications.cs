using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OAuth.MVC.Library;
using OAuth.MVC.Library.Filters;
using OAuth.MVC.Library.Interfaces;
using OAuth.MVC.Library.Results;
using OAuth.MVC.Tests.Helpers;
using Xunit;
using Rhino.Mocks;
namespace OAuth.MVC.Tests.Filters
{
  namespace OAuthSecuredAttributeSpecifications
  {
    public class given_an_access_request_to_a_secured_resource_without_access:IUseFixture<MockRepository>
  {
    private HttpResponseBase responseMock;
    readonly NameValueCollection headers = new NameValueCollection();
      private AuthorizationContext filterContext;

      [Fact] 
      public void result_should_be_set_to_unauthorized()
    {
      Assert.IsType<OAuthUnauthorizedResult>(filterContext.Result);
    }

    public void SetFixture(MockRepository mocks)
    {
      responseMock = mocks.DynamicMock<HttpResponseBase>();
      var httpContextMock = mocks.DynamicMock<HttpContextBase>();
      var oauthServiceMock = mocks.DynamicMock<IOAuthService>();
      var httpRequestMock = mocks.DynamicMock<HttpRequestBase>();
      var requestMock = mocks.DynamicMock<IOAuthRequest>();

      var parameters = new NameValueCollection();
      var uri = new Uri("http://someaddress.com");
      var httpMethod = "GET";
      filterContext = new AuthorizationContext(new ControllerContext(httpContextMock, new RouteData(), new TestController()));

      httpContextMock.Stub(httpContext => httpContext.Request).Return(httpRequestMock);
      httpRequestMock.Stub(httpRequest => httpRequest.Params).Return(parameters);
      httpRequestMock.Stub(httpRequest => httpRequest.Url).Return(uri);

      httpRequestMock.Stub(httpRequest => httpRequest.HttpMethod).Return(httpMethod);
      responseMock.Stub(httpResponse => httpResponse.Headers).Return(headers);

      oauthServiceMock.Stub(
        service => service.BuildRequest(uri, httpMethod, parameters, OAuthConstants.EndPointType.AccessRequest)).
        Return(requestMock);
      mocks.ReplayAll();
      requestMock.Stub(request => request.IsValid()).Return(false);

      var securedAttribute = new OAuthSecuredAttribute { OAuthService = oauthServiceMock };
      securedAttribute.OnAuthorization(filterContext);
    }
  }
    public class given_an_access_request_to_a_secured_resource_with_access : IUseFixture<MockRepository>
    {
      private HttpResponseBase responseMock;
      readonly NameValueCollection headers = new NameValueCollection();

      [Fact]
      public void no_headers_should_be_set()
      {
        Assert.Empty(headers);
      } 

      public void SetFixture(MockRepository mocks)
      {
        responseMock = mocks.DynamicMock<HttpResponseBase>();
        var httpContextMock = mocks.DynamicMock<HttpContextBase>();
        var oauthServiceMock = mocks.DynamicMock<IOAuthService>();
        var httpRequestMock = mocks.DynamicMock<HttpRequestBase>();
        var requestMock = mocks.DynamicMock<IOAuthRequest>();

        var parameters = new NameValueCollection();
        var uri = new Uri("http://someaddress.com");
        var httpMethod = "GET";
        var filterContext = new AuthorizationContext(new ControllerContext(httpContextMock, new RouteData(), new TestController()));

        httpContextMock.Stub(httpContext => httpContext.Response).Return(responseMock);
        httpContextMock.Stub(httpContext => httpContext.Request).Return(httpRequestMock);
        httpRequestMock.Stub(httpRequest => httpRequest.Params).Return(parameters);
        httpRequestMock.Stub(httpRequest => httpRequest.Url).Return(uri);
        
        httpRequestMock.Stub(httpRequest => httpRequest.HttpMethod).Return(httpMethod);

        
        oauthServiceMock.Stub(
          service => service.BuildRequest(uri, httpMethod, parameters, OAuthConstants.EndPointType.AccessRequest)).
          Return(requestMock);
        mocks.ReplayAll();
        requestMock.Stub(request => request.IsValid()).Return(true);

        var securedAttribute = new OAuthSecuredAttribute {OAuthService = oauthServiceMock};
        securedAttribute.OnAuthorization(filterContext);
      }
    }
  }
}