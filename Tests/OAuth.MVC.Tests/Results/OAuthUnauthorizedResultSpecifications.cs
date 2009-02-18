using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OAuth.MVC.Library.Results;
using Rhino.Mocks;
using Xunit;

namespace OAuth.MVC.Tests.Results
{
  namespace OAuthUnauthorizedResultSpecifications
  {
    public class when_unauthorized_result_iS_executed:IUseFixture<MockRepository>
    {
      private HttpResponseBase mockHttpResponse;
      [Fact]
      public void www_authenticate_header_should_be_added()
      {
        mockHttpResponse.AssertWasCalled(response => response.AppendHeader("WWW-Authenticate","OAuth realm=\"http://testsite.com\""));
      }
      [Fact]
      public void http_response_code_should_be_401_Unauthorized()
      {
        mockHttpResponse.AssertWasCalled(response=>response.StatusCode=401);
      }


      public void SetFixture(MockRepository mocks)
      {
        mockHttpResponse = mocks.DynamicMock<HttpResponseBase>();
        var mockHttpContext = mocks.DynamicMock<HttpContextBase>();
        var mockController = mocks.DynamicMock<ControllerBase>();

        mockHttpContext.Stub(context => context.Response).Return(mockHttpResponse);
        var mockContext = new ControllerContext(mockHttpContext,new RouteData(),mockController);
        mocks.ReplayAll();

        var result = new OAuthUnauthorizedResult();
        
        result.ExecuteResult(mockContext);

      }
    }
  }
}