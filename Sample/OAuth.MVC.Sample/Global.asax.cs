using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject.Conditions;
using System.Web.Routing;
using Ninject.Core;
using Ninject.Framework.Mvc;
using OAuth.MVC.Library;
using OAuth.MVC.Library.Controllers;
using OAuth.MVC.Library.Interfaces;
using OAuth.MVC.Sample.Controllers;

namespace OAuth.MVC.Sample
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : NinjectHttpApplication
  {
    protected override void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
          "Default",                                              // Route name
          "{controller}/{action}/{id}",                           // URL with parameters
          new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
      );
      

    }

    protected override IKernel CreateKernel()
    {
      return new StandardKernel(new SampleApplicationNinjectModule());
    }

   
  }

  internal class SampleApplicationNinjectModule : StandardModule
  {
    public override void Load()
    {
      Bind<IController>().To<OAuthController>().Only(
        When.Context.Variable("controllerName").Matches(
          controllerName => controllerName.ToString().Equals("oauth", StringComparison.InvariantCultureIgnoreCase)));
      Bind<IController>().To<AccountController>().Only(
        When.Context.Variable("controllerName").Matches(
          controllerName => controllerName.ToString().Equals("account", StringComparison.InvariantCultureIgnoreCase)));
      Bind<IOAuthService>().To<OAuthService>();
      Bind<IOAuthRepository>().ToConstant(new SampleOAuthRepository());
      Bind<ITokenGenerator>().ToConstant(new SimpleGuidTokenGenerator());
    }
  }

  internal class SimpleGuidTokenGenerator:ITokenGenerator
  {
    public IRequestToken GenerateNewRequestToken()
    {
      return new SimpleGuidRequestToken();
    }

    public IAccessToken GenerateNewAccessToken()
    {
      return new SimpleGuidAccessToken();
    }
  }

  internal class SimpleGuidAccessToken : SimpleGuidToken, IAccessToken
  {
    public Guid UserID
    {
      get; set;
    }
  }

  internal class SimpleGuidRequestToken :SimpleGuidToken, IRequestToken
  {
    public SimpleGuidRequestToken()
    {
      IsAuthorized = false;
      UserID = Guid.Empty;
    }

    public bool IsAuthorized{get; set;}

    public Guid UserID{get; set;}
  }

  internal  class SimpleGuidToken:IToken
  {
    public SimpleGuidToken()
    {
      Token = Guid.NewGuid().ToString();
      Secret = Guid.NewGuid().ToString();
    }

    public string Token { get; set; }

    public string Secret { get; set; }
  }

  internal class SampleOAuthRepository:IOAuthRepository
  {
    private readonly IDictionary<string,IConsumer> Consumers =
       new Dictionary<string, IConsumer>();

    public IConsumer GetConsumer(string consumerKey)
    {
      if (!Consumers.Keys.Contains(consumerKey))
      {
        //create the consumer
        Consumers.Add(consumerKey,new Consumer{SecretKey= consumerKey+"secret"});
        
      }
      return Consumers[consumerKey];
    }

    public IRequestToken GetExistingToken(string requestTokenString)
    {
      foreach(var consumer in Consumers)
      {
        var requestToken = consumer.Value.GetRequestToken(requestTokenString);
        if (requestToken != null)
          return requestToken;

      }
      return null;
    }
  }

  internal class Consumer : IConsumer
  {
    readonly IList<string> usedNonces = new List<string>();
    readonly IDictionary<string,IRequestToken> requestTokens = new Dictionary<string, IRequestToken>();
    readonly IDictionary<string,IAccessToken> accessTokens = new Dictionary<string, IAccessToken>();
    public string SecretKey{ get;set;}

    public int TimeStamp{get; set;}

    public IRequestToken GetRequestToken(string requestToken)
    {
      return requestTokens[requestToken];
    }

    public void SaveNonce(string timestamp, string nonce)
    {
      int timestampInt;
      if(!int.TryParse(timestamp,out timestampInt))
      {
        throw new Exception();
      }
      if(timestampInt>TimeStamp)
      {
        usedNonces.Clear();
        TimeStamp = timestampInt;
      }
    }

    public bool IsUsedNonce(string timestamp, string nonce)
    {
      return usedNonces.Contains(nonce);
    }

    public void SaveRequestToken(IRequestToken requestToken)
    {
     requestTokens.Add(requestToken.Token,requestToken); 
    }

    public void SaveAccessToken(IAccessToken accessToken)
    {
      accessTokens.Add(accessToken.Token, accessToken); 
    }
  }
}