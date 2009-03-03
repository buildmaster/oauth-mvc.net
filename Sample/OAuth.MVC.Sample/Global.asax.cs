using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using Codeclimber.Ninject.FilterInjector;
using System.Web.Routing;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider;
using DevDefined.OAuth.Provider.Inspectors;
using DevDefined.OAuth.Storage;
using DevDefined.OAuth.Testing;
using Ninject.Core;
using Ninject.Framework.Mvc;
using OAuth.MVC.Library.Binders;
using OAuth.MVC.Library.Controllers;

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


      ModelBinders.Binders.Add(typeof(IOAuthContext), new OAuthBinder {OAuthContextBuilder = KernelContainer.Kernel.Get<IOAuthContextBuilder>() });
    }

    protected override IKernel CreateKernel()
    {
      return new StandardKernel(new AutoControllerModuleWithFilters(Assembly.GetExecutingAssembly(),typeof(OAuthController).Assembly),new SampleModuler());
    }

   
  }

  internal class SampleModuler : StandardModule
  {
    public override void Load()
    {
      Bind<IOAuthContextBuilder>().To<OAuthContextBuilder>();
      var nonceStoreInspector = new NonceStoreInspector(new TestNonceStore());
      var consumerStore = new TestConsumerStore();
      var signatureInspector = new SignatureValidationInspector(consumerStore);
      var consumerValidationInspector = new ConsumerValidationInspector(consumerStore);
      var timestampInspector = new TimestampRangeInspector(new TimeSpan(1,0 , 0));
      var tokenRepository = new TokenRepository();
      var tokenStore = new SampleMemoryTokenStore(tokenRepository);
      var oauthProvider = new OAuthProvider(tokenStore,consumerValidationInspector, nonceStoreInspector,timestampInspector, signatureInspector);
      Bind<IOAuthProvider>().ToConstant(oauthProvider);
      Bind<TokenRepository>().ToConstant(tokenRepository);
    }
  }

  internal class SampleConsumerStore:IConsumerStore
  {
    public Dictionary<string,string> secrets = new Dictionary<string, string>();
    public bool IsConsumer(IConsumer consumer)
    {
      return true;
    }

    public void SetConsumerSecret(IConsumer consumer, string consumerSecret)
    {
      secrets[consumer.ConsumerKey] = consumerSecret;
    }

    public string GetConsumerSecret(IConsumer consumer)
    {
      if(!secrets.ContainsKey(consumer.ConsumerKey))
        secrets[consumer.ConsumerKey] = consumer.ConsumerKey + "Secret";
      return consumer.ConsumerKey;
    }

    public void SetConsumerCertificate(IConsumer consumer, X509Certificate2 certificate)
    {
      throw new System.NotImplementedException();
    }

    public X509Certificate2 GetConsumerCertificate(IConsumer consumer)
    {
      throw new System.NotImplementedException();
    }
  }

  internal class SampleNonceStore:INonceStore
  {
    readonly Dictionary<string,List<string>> noncesForConsumer = new Dictionary<string, List<string>>();
    public bool RecordNonceAndCheckIsUnique(IConsumer consumer, string nonce)
    {
      if(nonce==null||consumer.ConsumerKey==null)
        return true;
      if (!noncesForConsumer.ContainsKey(consumer.ConsumerKey))
        noncesForConsumer[consumer.ConsumerKey] = new List<string>();
      if(noncesForConsumer[consumer.ConsumerKey].Contains(nonce))
        return false;
      else
      {
        noncesForConsumer[consumer.ConsumerKey].Add(nonce);
        return true;
      }
    }
  }

  internal class SampleMemoryTokenStore:ITokenStore
  {
 readonly TokenRepository _repository;

 public SampleMemoryTokenStore(TokenRepository repository)
    {
      _repository = repository;
    }

    #region ITokenStore Members

    public IToken CreateRequestToken(IOAuthContext context)
    {
      var token = new RequestToken
                    {
                      ConsumerKey = context.ConsumerKey,
                      Realm = context.Realm,
                      Token = Guid.NewGuid().ToString(),
                      TokenSecret = Guid.NewGuid().ToString(),
                      AccessDenied = true,
        };

      _repository.SaveRequestToken(token);

      return token;
    }

    public void ConsumeRequestToken(IOAuthContext requestContext)
    {
      RequestToken requestToken = _repository.GetRequestToken(requestContext.Token);

      if (requestToken.UsedUp)
      {
        throw new OAuthException(requestContext, OAuthProblems.TokenRejected,
                                 "The request token has already be consumed.");
      }
      if(!requestToken.AccessDenied)
        requestToken.UsedUp = true;

      _repository.SaveRequestToken(requestToken);
    }

    public void ConsumeAccessToken(IOAuthContext accessContext)
    {
      AccessToken accessToken = _repository.GetAccessToken(accessContext.Token);

      if (accessToken.ExpireyDate < DateTime.Now)
      {
        throw new OAuthException(accessContext, OAuthProblems.TokenExpired,
                                 "Token has expired (they're only valid for 1 minute)");
      }
    }

    public IToken GetAccessTokenAssociatedWithRequestToken(IOAuthContext requestContext)
    {
      RequestToken request = _repository.GetRequestToken(requestContext.Token);
      return request.AccessToken;
    }

    public RequestForAccessStatus GetStatusOfRequestForAccess(IOAuthContext accessContext)
    {
      RequestToken request = _repository.GetRequestToken(accessContext.Token);

      if (request.AccessDenied) return RequestForAccessStatus.Denied;

      if (request.AccessToken == null) return RequestForAccessStatus.Unknown;

      return RequestForAccessStatus.Granted;
    }

    public IToken GetToken(IOAuthContext context)
    {
      var token = (IToken)null;
      if (!string.IsNullOrEmpty(context.Token))
      {
        token = _repository.GetAccessToken(context.Token) ??
                (IToken)_repository.GetRequestToken(context.Token);
      }
      return token;
    }

    #endregion
  }

  /// <summary>
  /// A simplistic in-memory repository for access and request token models - the example implementation of
  /// <see cref="ITokenStore" /> relies on this repository - normally you would make use of repositories
  /// wired up to your domain model i.e. NHibernate, Entity Framework etc.
  /// </summary>    
  public class TokenRepository
  {
    readonly Dictionary<string, AccessToken> _accessTokens = new Dictionary<string, AccessToken>();
    readonly Dictionary<string, RequestToken> _requestTokens = new Dictionary<string, RequestToken>();

    public RequestToken GetRequestToken(string token)
    {
      if(_requestTokens.ContainsKey(token))
        return _requestTokens[token];
      return null;
    }

    public AccessToken GetAccessToken(string token)
    {
      if(_accessTokens.ContainsKey(token))
         return _accessTokens[token];
      return null;
    }

    public void SaveRequestToken(RequestToken token)
    {
      _requestTokens[token.Token] = token;
    }

    public void SaveAccessToken(AccessToken token)
    {
      _accessTokens[token.Token] = token;
    }
  }
  public class RequestToken : TokenBase
  {
    public bool AccessDenied { get; set; }
    public bool UsedUp { get; set; }
    public AccessToken AccessToken { get; set; }
  }
  public class AccessToken : TokenBase
  {
    public string UserName { get; set; }
    public DateTime ExpireyDate { get; set; }
  }
}