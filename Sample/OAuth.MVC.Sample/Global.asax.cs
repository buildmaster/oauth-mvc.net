using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Mvc;
using OAuth.Core;
using OAuth.Core.Interfaces;
using OAuth.Core.Provider;
using OAuth.Core.Provider.Inspectors;
using OAuth.Core.Storage;
using OAuth.Core.Storage.Interfaces;


namespace OAuth.MVC.Sample
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : NinjectHttpApplication
  {
      protected static void RegisterRoutes(RouteCollection routes)
      {
          routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

          routes.MapRoute(
              "Default",                                              // Route name
              "{controller}/{action}/{id}",                           // URL with parameters
              new { controller = "SignIn", action = "Login", id = "" }  // Parameter defaults
          );

      }
      protected override void OnApplicationStarted()
      {
          base.OnApplicationStarted();
          RegisterRoutes(RouteTable.Routes);
          RegisterAllControllersIn(Assembly.GetExecutingAssembly());
      }
      protected override IKernel CreateKernel()
      {
          return new StandardKernel(new SampleModule());
      }
   
  }

  internal class SampleModule : NinjectModule
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
    public Dictionary<string,string> Secrets = new Dictionary<string, string>();
    public bool IsConsumer(IConsumer consumer)
    {
      return true;
    }

    public void SetConsumerSecret(IConsumer consumer, string consumerSecret)
    {
      Secrets[consumer.ConsumerKey] = consumerSecret;
    }

    public string GetConsumerSecret(IConsumer consumer)
    {
      if(!Secrets.ContainsKey(consumer.ConsumerKey))
        Secrets[consumer.ConsumerKey] = consumer.ConsumerKey + "Secret";
      return consumer.ConsumerKey;
    }

    public void SetConsumerCertificate(IConsumer consumer, X509Certificate2 certificate)
    {
      throw new NotImplementedException();
    }

    public X509Certificate2 GetConsumerCertificate(IConsumer consumer)
    {
      throw new NotImplementedException();
    }
  }

  internal class SampleNonceStore:INonceStore
  {
    readonly Dictionary<string,List<string>> _noncesForConsumer = new Dictionary<string, List<string>>();
    public bool RecordNonceAndCheckIsUnique(IConsumer consumer, string nonce)
    {
      if(nonce==null||consumer.ConsumerKey==null)
        return true;
      if (!_noncesForConsumer.ContainsKey(consumer.ConsumerKey))
        _noncesForConsumer[consumer.ConsumerKey] = new List<string>();
      if(_noncesForConsumer[consumer.ConsumerKey].Contains(nonce))
        return false;
        _noncesForConsumer[consumer.ConsumerKey].Add(nonce);
        return true;
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
  public class TestConsumerStore : IConsumerStore
  {
      #region IConsumerStore Members

      public bool IsConsumer(IConsumer consumer)
      {
          return (consumer.ConsumerKey == "key" && string.IsNullOrEmpty(consumer.Realm));
      }

      public void SetConsumerSecret(IConsumer consumer, string consumerSecret)
      {
          throw new NotImplementedException();
      }

      public string GetConsumerSecret(IConsumer consumer)
      {
          return "secret";
      }

      public void SetConsumerCertificate(IConsumer consumer, X509Certificate2 certificate)
      {
          throw new NotImplementedException();
      }

      public X509Certificate2 GetConsumerCertificate(IConsumer consumer)
      {
          return TestCertificates.OAuthTestCertificate();
      }

      #endregion
  }
  public static class TestCertificates
  {
      const string Certificate =
        @"-----BEGIN CERTIFICATE-----
MIIBpjCCAQ+gAwIBAgIBATANBgkqhkiG9w0BAQUFADAZMRcwFQYDVQQDDA5UZXN0
IFByaW5jaXBhbDAeFw03MDAxMDEwODAwMDBaFw0zODEyMzEwODAwMDBaMBkxFzAV
BgNVBAMMDlRlc3QgUHJpbmNpcGFsMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKB
gQC0YjCwIfYoprq/FQO6lb3asXrxLlJFuCvtinTF5p0GxvQGu5O3gYytUvtC2JlY
zypSRjVxwxrsuRcP3e641SdASwfrmzyvIgP08N4S0IFzEURkV1wp/IpH7kH41Etb
mUmrXSwfNZsnQRE5SYSOhh+LcK2wyQkdgcMv11l4KoBkcwIDAQABMA0GCSqGSIb3
DQEBBQUAA4GBAGZLPEuJ5SiJ2ryq+CmEGOXfvlTtEL2nuGtr9PewxkgnOjZpUy+d
4TvuXJbNQc8f4AMWL/tO9w0Fk80rWKp9ea8/df4qMq5qlFWlx6yOLQxumNOmECKb
WpkUQDIDJEoFUzKMVuJf4KO/FJ345+BNLGgbJ6WujreoM1X/gYfdnJ/J
-----END CERTIFICATE-----";

      const string PrivateKey =
        @"MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBALRiMLAh9iimur8VA7qVvdqxevEuUkW4K+2KdMXmnQbG9Aa7k7eBjK1S+0LYmVjPKlJGNXHDGuy5Fw/d7rjVJ0BLB+ubPK8iA/Tw3hLQgXMRRGRXXCn8ikfuQfjUS1uZSatdLB81mydBETlJhI6GH4twrbDJCR2Bwy/XWXgqgGRzAgMBAAECgYBYWVtleUzavkbrPjy0T5FMou8HX9u2AC2ry8vD/l7cqedtwMPp9k7TubgNFo+NGvKsl2ynyprOZR1xjQ7WgrgVB+mmuScOM/5HVceFuGRDhYTCObE+y1kxRloNYXnx3ei1zbeYLPCHdhxRYW7T0qcynNmwrn05/KO2RLjgQNalsQJBANeA3Q4Nugqy4QBUCEC09SqylT2K9FrrItqL2QKc9v0ZzO2uwllCbg0dwpVuYPYXYvikNHHg+aCWF+VXsb9rpPsCQQDWR9TT4ORdzoj+NccnqkMsDmzt0EfNaAOwHOmVJ2RVBspPcxt5iN4HI7HNeG6U5YsFBb+/GZbgfBT3kpNGWPTpAkBI+gFhjfJvRw38n3g/+UeAkwMI2TJQS4n8+hid0uus3/zOjDySH3XHCUnocn1xOJAyZODBo47E+67R4jV1/gzbAkEAklJaspRPXP877NssM5nAZMU0/O/NGCZ+3jPgDUno6WbJn5cqm8MqWhW1xGkImgRk+fkDBquiq4gPiT898jusgQJAd5Zrr6Q8AO/0isr/3aa6O6NLQxISLKcPDk2NOccAfS/xOtfOz4sJYM3+Bs4Io9+dZGSDCA54Lw03eHTNQghS0A==";

      public static X509Certificate2 OAuthTestCertificate()
      {
          return CertificateUtility.LoadCertificateFromStrings(PrivateKey, Certificate);
      }
  }
  /// <summary>
  /// A simple nonce store that just tracks all nonces by consumer key in memory.
  /// </summary>
  public class TestNonceStore : INonceStore
  {
      readonly Dictionary<string, List<string>> _nonces = new Dictionary<string, List<string>>();

      #region INonceStore Members

      public bool RecordNonceAndCheckIsUnique(IConsumer consumer, string nonce)
      {
          List<string> list = GetNonceListForConsumer(consumer.ConsumerKey);
          lock (list)
          {
              if (list.Contains(nonce)) return false;
              list.Add(nonce);
              return true;
          }
      }

      #endregion

      List<string> GetNonceListForConsumer(string consumerKey)
      {
          List<string> list;

          if (!_nonces.TryGetValue(consumerKey, out list))
          {
              lock (_nonces)
              {
                  if (!_nonces.TryGetValue(consumerKey, out list))
                  {
                      list = new List<string>();
                      _nonces[consumerKey] = list;
                  }
              }
          }

          return list;
      }
  }
}