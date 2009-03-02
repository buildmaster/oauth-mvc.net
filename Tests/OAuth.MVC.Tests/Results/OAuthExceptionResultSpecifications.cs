using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DevDefined.OAuth.Framework;
using OAuth.MVC.Library.Results;
using Rhino.Mocks;
using Xunit;

namespace OAuth.MVC.Tests.Results
{
  namespace OAuthExceptionResultSpecifications
  {
    public class ExceptionResultContext:IUseFixture<MockRepository>
    {
      private IOAuthContext defaultContext;
      private OAuthExceptionResult result;
      private ControllerBase controller;
      private HttpContextBase httpContext;
      private RouteData routeData;
      protected HttpResponseBase Response;
      protected HttpRequestBase Request;
      private const string defaultRealm = "http://testauth.com";
      private const string defaultProblem = OAuthProblems.SignatureInvalid;
      private const string advice = "this is default advice for fixing the problem";

      public virtual OAuthException Exception
      {
        get
        {
          return new OAuthException(Context,DefaultProblem,Advice);
        }
      }

      protected virtual string Advice
      {
        get { return advice; }
      }

      protected virtual string DefaultProblem
      {
        get { return defaultProblem; }
      }

      protected virtual IOAuthContext Context
      {
        get { return defaultContext; }
      }
      protected virtual string Realm
      {
        get { return defaultRealm; }
      }
      public void SetFixture(MockRepository mocks)
      {
        defaultContext = mocks.DynamicMock<IOAuthContext>();
        Response = mocks.DynamicMock<HttpResponseBase>();
        httpContext = mocks.DynamicMock<HttpContextBase>();
        routeData = new RouteData();
        controller = mocks.DynamicMock<ControllerBase>();
        defaultContext.Stub(context => context.Realm).Return(Realm);
        httpContext.Stub(context => context.Response).Return(Response);
        var controllerContext = new ControllerContext(httpContext,routeData,controller);
        result = new OAuthExceptionResult(Exception);

        mocks.ReplayAll();
        result.ExecuteResult(controllerContext);
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
        Response.AssertWasCalled(response=>response.AddHeader("WWW-Authenticate",String.Format("OAuth Realm=\"{0}\"",Realm)));
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
}