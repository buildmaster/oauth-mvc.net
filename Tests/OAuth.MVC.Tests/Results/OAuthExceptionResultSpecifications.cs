using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OAuth.Core;
using OAuth.Core.Interfaces;
using OAuth.MVC.Library;
using OAuth.MVC.Library.Results;
using Rhino.Mocks;
using Xunit;

namespace OAuth.MVC.Tests.Results
{
    // ReSharper disable InconsistentNaming
  namespace OAuthExceptionResultSpecifications
  {
    public class ExceptionResultContext:IUseFixture<MockRepository>
    {
      private IOAuthContext _defaultContext;
      private OAuthExceptionResult _result;
      private ControllerBase _controller;
      private HttpContextBase _httpContext;
      private RouteData _routeData;
      protected HttpResponseBase Response;
      protected HttpRequestBase Request;
      private const string DefaultRealm = "http://testauth.com";
      private const string DefaultProblem = OAuthProblems.SignatureInvalid;
      private const string AdviceString = "this is default Advice for fixing the problem";

      public virtual OAuthException Exception
      {
        get
        {
          return new OAuthException(Context,DefaultProblem,Advice);
        }
      }

      protected virtual string Advice
      {
          get { return AdviceString; }
      }

      protected virtual string Problem
      {
          get { return DefaultProblem; }
      }

      protected virtual IOAuthContext Context
      {
        get { return _defaultContext; }
      }
      protected virtual string Realm
      {
        get { return DefaultRealm; }
      }
      public void SetFixture(MockRepository mocks)
      {
        _defaultContext = mocks.DynamicMock<IOAuthContext>();
        Response = mocks.DynamicMock<HttpResponseBase>();
        _httpContext = mocks.DynamicMock<HttpContextBase>();
        _routeData = new RouteData();
        _controller = mocks.DynamicMock<ControllerBase>();
        _defaultContext.Stub(context => context.Realm).Return(Realm);
        _httpContext.Stub(context => context.Response).Return(Response);
        var controllerContext = new ControllerContext(_httpContext,_routeData,_controller);
        _result = new OAuthExceptionResult(Exception);

        mocks.ReplayAll();
        _result.ExecuteResult(controllerContext);
      }
    }

    public class given_a_result_for_a_consumer_key_unknown_exception:ExceptionResultContext

    {
      [Fact]
      public void return_status_should_be_unauthorised()
      {
        Response.AssertWasCalled(response=>response.StatusCode=(int)HttpStatusCode.Unauthorized);
      }
      [Fact]
      public void return_body_should_be_set_to_advice()
      {
        Response.AssertWasCalled(response=>response.Write(Exception.Report.ToString()));
      }
      [Fact]
      public void oauth_header_should_be_set()
      {
        Response.AssertWasCalled(response=>response.AddHeader("WWW-Authenticate",String.Format("OAuth Realm=\"{0}\"",Settings.Default.Realm)));
      }
      public override OAuthException Exception
      {
        get
        {
          return new OAuthException(Context,OAuthProblems.ConsumerKeyUnknown,Advice);
        }
      }

    }
    public class given_a_result_for_a_parameter_unknown_exception : ExceptionResultContext
    {
      public string[] parameters_rejected =new[]{"paramter1","parameter2"};
      [Fact]
      public void return_status_should_be_bad_request()
      {
        Response.AssertWasCalled(response => response.StatusCode = (int)HttpStatusCode.BadRequest);
      }
      [Fact]
      public void return_body_should_be_set_to_oauth_params()
      {
        Response.AssertWasCalled(response => response.Write(Exception.Report.ToString()));
      }

      public override OAuthException Exception
      {
        get
        {
          
          var exception= new OAuthException(Context, OAuthProblems.ParameterRejected, Advice);
          exception.Report.ParametersRejected.AddRange(parameters_rejected);
          return exception;
        }
      }

    }
    public class given_a_result_for_a_signature_method_unsupported_exception : ExceptionResultContext
    {
      public string[] parameters_rejected = new[] { "paramter1", "parameter2" };
      [Fact]
      public void return_status_should_be_bad_request()
      {
        Response.AssertWasCalled(response => response.StatusCode = (int)HttpStatusCode.BadRequest);
      }
      public override OAuthException Exception
      {
        get
        {
          var exception = new OAuthException(Context, OAuthProblems.SignatureMethodRejected, Advice);
          exception.Report.ParametersRejected.AddRange(parameters_rejected);
          return exception;
        }
      }

    }
    public class given_a_result_for_a_missing_parameter_exception : ExceptionResultContext
    {
      public string[] parameters_rejected = new[] { "paramter1", "parameter2" };
      [Fact]
      public void return_status_should_be_bad_request()
      {
        Response.AssertWasCalled(response => response.StatusCode = (int)HttpStatusCode.BadRequest);
      }
      public override OAuthException Exception
      {
        get
        {
          var exception = new OAuthException(Context, OAuthProblems.ParameterAbset, Advice);
          exception.Report.ParametersRejected.AddRange(parameters_rejected);
          return exception;
        }
      }

    }
  }
  // ReSharper restore InconsistentNaming
}