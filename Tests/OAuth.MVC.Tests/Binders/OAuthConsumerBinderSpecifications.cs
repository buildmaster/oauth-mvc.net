using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OAuth.MVC.Library;
using OAuth.MVC.Library.Binders;
using OAuth.MVC.Library.Interfaces;
using Rhino.Mocks;
using Xunit;

namespace OAuth.MVC.Tests.Binders
{
  namespace OAuthConsumerBinderSpecifications
  {
    public class given_a_consumer_bind_request_for_a_valid_oauth_request : OAuthConsumerBindContext
    {
      [Fact]
      public void object_returned_should_be_the_request_consumer()
      {
        Assert.Same(mockConsumer,bindedValue);
      }
    }
    public class given_a_consumer_bind_request_for_an_invalid_oauth_request : OAuthConsumerBindContext
    {
      [Fact]
      public void object_returned_should_be_null()
      {
        Assert.Null(bindedValue);
      }
      protected override bool OAuthRequestValid
      {
        get
        {
          return false;
        }
      }
    }
    public class given_a_consumer_bind_request_for_a_concret_iconsumer_type : OAuthConsumerBindContext
    {
      [Fact]
      public void object_returned_should_be_null()
      {
        Assert.Null(bindedValue);
      }
      protected override Type ModelType
      {
        get
        {
          return typeof(TestConsumer);
        }
      }
    }
    public class given_a_consumer_bind_request_for_an_incorrect_interface_type : OAuthConsumerBindContext
    {
      [Fact]
      public void object_returned_should_be_null()
      {
        Assert.Null(bindedValue);
      }
      protected override Type ModelType
      {
        get
        {
          return typeof(IOAuthRequest);
        }
      }
    }

    public class OAuthConsumerBindContext:IUseFixture<MockRepository>
    {
      protected object bindedValue;
      protected IConsumer mockConsumer;
      private const bool DefaultOAuthRequestValid = true;
      private readonly Type DefaultModelType = typeof (IConsumer);
      public void SetFixture(MockRepository mocks)
      {
        var mockOAuthService = mocks.DynamicMock<IOAuthService>();
        mockConsumer = mocks.DynamicMock<IConsumer>();
        var mockController = mocks.DynamicMock<ControllerBase>();
        var mockHttpContext = mocks.DynamicMock<HttpContextBase>();
        var mockHttpRequest = mocks.DynamicMock<HttpRequestBase>();
        var mockOAuthRequest = mocks.DynamicMock<IOAuthRequest>();

        var parameters = new NameValueCollection();
        var headers = new NameValueCollection();
        var controllerContext = new ControllerContext(mockHttpContext, new RouteData(), mockController);
        var bindingContext = new ModelBindingContext {ModelType = ModelType};
        var url = new Uri("http://somewhere.com");
        const string httpMethod = "GET";

        mockHttpContext.Stub(context => context.Request).Return(mockHttpRequest);

        mockHttpRequest.Stub(request => request.HttpMethod).Return(httpMethod);
        mockHttpRequest.Stub(request => request.Params).Return(parameters);
        mockHttpRequest.Stub(request => request.Url).Return(url);
        mockHttpRequest.Stub(request => request.Headers).Return(headers);
        mockOAuthService.Stub(
          service => service.BuildRequest(url, httpMethod, parameters,headers, OAuthConstants.EndPointType.AccessRequest)).Return(mockOAuthRequest);

        mockOAuthRequest.Stub(request => request.Consumer).Return(mockConsumer);
        mockOAuthRequest.Stub(request => request.IsValid()).Return(OAuthRequestValid);
        mocks.ReplayAll();
        var consumerBinder = new OAuthBinder { OAuthService = mockOAuthService };


        bindedValue = consumerBinder.BindModel(controllerContext, bindingContext);
      }
      protected virtual Type ModelType
      {
        get { return DefaultModelType; }
      }
      protected virtual bool OAuthRequestValid
      {
        get { return DefaultOAuthRequestValid; }
      }
    }
    public class TestConsumer:IConsumer
    {

      #region IConsumer Members

      public string SecretKey
      {
        get { throw new NotImplementedException(); }
      }

      public int TimeStamp
      {
        get { throw new NotImplementedException(); }
      }

      public IRequestToken GetRequestToken(string requestToken)
      {
        throw new NotImplementedException();
      }

      public void SaveNonce(string timestamp, string nonce)
      {
        throw new NotImplementedException();
      }

      public bool IsUsedNonce(string timestamp, string nonce)
      {
        throw new NotImplementedException();
      }

      public void SaveRequestToken(IRequestToken requestToken)
      {
        throw new NotImplementedException();
      }

      public void SaveAccessToken(IAccessToken token)
      {
        throw new NotImplementedException();
      }

      #endregion
    }
  }
}