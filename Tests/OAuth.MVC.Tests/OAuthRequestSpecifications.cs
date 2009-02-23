using System;
using System.Collections.Generic;
using System.Linq;
using OAuth.MVC.Library;
using OAuth.MVC.Library.Errors;
using OAuth.MVC.Library.Interfaces;
using Rhino.Mocks;
using Xunit;

namespace OAuth.MVC.Tests
{
  namespace OAuthRequestSpecifications
  {
    public abstract class OAuthRequestContext:IUseFixture<MockRepository>
    {
      #region defaults
      //oauth_consumer_key=dpf43f3p2l4k3l03&oauth_signature_method=PLAINTEXT&oauth_signature=kd94hf93k423kf44%26&oauth_timestamp=1191242090&oauth_nonce=hsu94j3884jdopsl&oauth_version=1.0
      protected const string DefaultHttpMethod = "POST";
      protected const string DefaultUriString = "https://photos.example.net/request_token?oauth_consumer_key=dpf43f3p2l4k3l03&oauth_signature_method=PLAINTEXT&oauth_signature=kd94hf93k423kf44%26";
      private const string Nonce = "hsu94j3884jdopsl";
      private const string Timestamp = "1191242090";
      private const string DefaultConsumerSecret = "kd94hf93k423kf44";
      private const bool DefaultIsUsedNonce = false;
      private const bool DefaultIsTokenAuthenticated = true;
      private const string DefaultTokenSecret = "hdhd0244k9j7ao03";
      private const OAuthConstants.EndPointType DefaultEndPointType = OAuthConstants.EndPointType.RequestTokenRequest;

      protected const int DefaultConsumerTimestamp = 1191242090;

      protected IDictionary<string, string> allAccessTokenParamters = new Dictionary<string, string>
                                                                        {
                                                                          {"oauth_token", "hh5s93j4hdidpola"},
                                                                        };
      protected IDictionary<string, string> allBasicParameters = new Dictionary<string, string>
                                                            {
                                                              {"oauth_consumer_key","dpf43f3p2l4k3l03"},
                                                              {"oauth_signature_method","PLAINTEXT"},
                                                              {"oauth_signature","kd94hf93k423kf44%26"},
                                                              {"oauth_timestamp",Timestamp},
                                                              {"oauth_nonce",Nonce},
                                                              {"oauth_version","1.0"},
                                                            };

      protected IOAuthRequest request;
      private IOAuthRepository mockIOAuthRepository;
      private IConsumer mockConsumer;
      private IRequestToken mockToken;

      #endregion

      public void SetFixture(MockRepository mocks)
      {
        mockIOAuthRepository = mocks.DynamicMock<IOAuthRepository>();
        mockConsumer = mocks.DynamicMock<IConsumer>();
        mockToken = mocks.DynamicMock<IRequestToken>();
        mockConsumer.Stub(consumer => consumer.Secret).Return(ConsumerSecret);
        mockConsumer.Stub(consumer => consumer.IsUsedNonce(Timestamp, Nonce)).Return(IsUsedNonce);
        mockConsumer.Stub(consumer => consumer.TimeStamp).Return(ConsumerTimestamp);
        

        mockIOAuthRepository.Stub(consumerRepository => consumerRepository.GetConsumer(Parameters
                                                                                         .Where(kvp =>kvp.Key == "oauth_consumer_key")
                                                                                         .Select(kvp => kvp.Value)
                                                                                         .FirstOrDefault()))
                                                                                         .Return(mockConsumer);
        
        mockConsumer.Stub(consumer => consumer.GetRequestToken(Parameters
                                                          .Where(kvp => kvp.Key == "oauth_token")
                                                          .Select(kvp => kvp.Value)
                                                          .FirstOrDefault()))
                                                          .Return(RequestToken);

        mockToken.Stub(token => token.IsAuthorized).Return(IsTokenAuthenticated);
        mockToken.Stub(token => token.Secret).Return(TokenSecret);
        mocks.ReplayAll();
        request = new OAuthRequest(URL,HttpMethod,Parameters,mockIOAuthRepository,EndPointType);
      }

      #region context overrides
      protected virtual IRequestToken RequestToken
      {
        get
        {
          return mockToken;
        }
      }
      protected virtual string TokenSecret
      {
        get
        {
          return DefaultTokenSecret;
        }
      }
      protected virtual bool IsTokenAuthenticated
      {
        get { return DefaultIsTokenAuthenticated; }
      }

      protected virtual OAuthConstants.EndPointType EndPointType
      {
        get
        {
          return DefaultEndPointType;
        }
      }
      protected virtual string ConsumerSecret
      {
        get
        {
          return DefaultConsumerSecret;
        }
      }
      protected virtual bool IsUsedNonce
      {
        get
        {
          return DefaultIsUsedNonce;
        }
      }
      protected virtual int ConsumerTimestamp
      {
        get
        {
          return DefaultConsumerTimestamp;
        }
      }
      protected abstract IEnumerable<KeyValuePair<string, string>>Parameters{ get;}
      protected virtual string HttpMethod{ get{ return DefaultHttpMethod;}}
      protected virtual Uri URL{ get{ return new Uri(DefaultUriString);} }
      #endregion
    }
    public class a_request_without_a_signature:OAuthRequestContext
    {
      [Fact]
      public void should_not_be_valid()
      {
        Assert.False(request.IsValid());
      }
      [Fact]
      public void should_have_a_missing_parameter_error()
      {
        Assert.IsType<MissingRequiredParameterError>(request.Error);
      }

      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get { return allBasicParameters.Where(kvp => kvp.Key != "oauth_signature"); }
      }
    }
    public class a_request_without_a_consumer_key : OAuthRequestContext
    {
      [Fact]
      public void should_not_be_valid()
      {
        Assert.False(request.IsValid());
      }
      [Fact]
      public void should_have_a_missing_parameter_error()
      {
        Assert.IsType<MissingRequiredParameterError>(request.Error);
      }

      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get { return allBasicParameters.Where(kvp => kvp.Key != "oauth_consumer_key"); }
      }
    }
    public class a_request_without_a_timestamp : OAuthRequestContext
    {
      [Fact]
      public void should_not_be_valid()
      {
        Assert.False(request.IsValid());
      }
      [Fact]
      public void should_have_a_missing_parameter_error()
      {
        Assert.IsType<MissingRequiredParameterError>(request.Error);
      }

      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get { return allBasicParameters.Where(kvp => kvp.Key != "oauth_timestamp"); }
      }
    }
    public class a_request_without_a_nonce : OAuthRequestContext
    {
      [Fact]
      public void should_not_be_valid()
      {
        Assert.False(request.IsValid());
      }
      [Fact]
      public void should_have_a_missing_parameter_error()
      {
        Assert.IsType<MissingRequiredParameterError>(request.Error);
      }

      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get { return allBasicParameters.Where(kvp => kvp.Key != "oauth_nonce"); }
      }
    }
    public class a_request_without_a_version : OAuthRequestContext
    {
      [Fact]
      public void should_be_valid()
      {
        Assert.True(request.IsValid());
      }
      [Fact]
      public void should_have_no_error()
      {
        Assert.Null(request.Error);
      }

      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get { return allBasicParameters.Where(kvp => kvp.Key != "oauth_version"); }
      }
    }
    public class a_request_without_an_incorrect_version : OAuthRequestContext
    {
      [Fact]
      public void should_be_valid()
      {
        Assert.False(request.IsValid());
      }
      [Fact]
      public void should_have_unsupported_parameter_error()
      {
        Assert.IsType<UnsuportedParameterError>(request.Error);
      }

      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get
        {
          allBasicParameters["oauth_version"] = "2.0";
          return allBasicParameters; 
        }
      }

    }
    public class a_request_with_an_unknown_signature_method : OAuthRequestContext
    {
      [Fact]
      public void should_be_valid()
      {
        Assert.False(request.IsValid());
      }
      [Fact]
      public void should_have_unsupported_signature_method_error()
      {
        Assert.IsType<UnsupportedSignatureMethodError>(request.Error);
      }

      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get
        {
          allBasicParameters["oauth_signature_method"] = "ImAMadeup-SigMethod";
          return allBasicParameters;
        }
      }
    }
    public class a_request_with_an_incorrect_signature : OAuthRequestContext
    {
      [Fact]
      public void should_be_valid()
      {
        Assert.False(request.IsValid());
      }
      [Fact]
      public void should_have_invalid_signature_error()
      {
        Assert.IsType<InvalidSignatureError>(request.Error);
      }

      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get
        {
          allBasicParameters["oauth_signature"] = "sdaddwekasdl";
          return allBasicParameters;
        }
      }
    }
    public class a_request_with_a_used_nonce:OAuthRequestContext
    {
      [Fact]
      public void should_not_be_valid()
      {
        Assert.False(request.IsValid());
      }
      [Fact]
      public void should_have_an_invalid_nonce_error()
      {
        Assert.IsType<InvalidNonceError>(request.Error);
      }
      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get { return allBasicParameters; }
      }

      protected override bool IsUsedNonce
      {
        get
        {
          return true;
        }
      }
    }
    public class a_request_with_an_outdated_timestamp : OAuthRequestContext
    {
      [Fact]
      public void should_not_be_valid()
      {
        Assert.False(request.IsValid());
      }
      [Fact]
      public void should_have_an_invalid_nonce_error()
      {
        Assert.IsType<InvalidNonceError>(request.Error);
      }
      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get { return allBasicParameters; }
      }
      protected override int ConsumerTimestamp
      {
        get
        {
          return DefaultConsumerTimestamp + 1;
        }
      }
    }
    public class a_valid_request_token_request:OAuthRequestContext
    {
      [Fact]
      public void should_not_be_valid()
      {
        Assert.True(request.IsValid());
      }
      [Fact]
      public void should_have_an_invalid_nonce_error()
      {
        Assert.Null(request.Error);
      }
      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get { return allBasicParameters; }
      }
    }
    public class an_access_token_request_without_request_token : OAuthRequestContext
    {
      [Fact]
      public void should_not_be_valid()
      {
        Assert.False(request.IsValid());
      }
      [Fact]
      public void should_have_an_missing_required_parameter()
      {
        Assert.IsType<MissingRequiredParameterError>(request.Error);
      }
      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get { return allBasicParameters.Concat(allAccessTokenParamters).Where(kvp=>kvp.Key!="oauth_token"); }
      }

      protected override OAuthConstants.EndPointType EndPointType
      {
        get
        {
          return OAuthConstants.EndPointType.AccessTokenRequest;
        }
      }
    }
    public class an_access_token_request_without_authorised_request_token : OAuthRequestContext
    {
      [Fact]
      public void should_not_be_valid()
      {
        Assert.False(request.IsValid());
      }
      [Fact]
      public void should_have_an_invalid_token_error()
      {
        Assert.IsType<InvalidTokenError>(request.Error);
      }
      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get { return allBasicParameters.Concat(allAccessTokenParamters); }
      }
      protected override bool IsTokenAuthenticated
      {
        get
        {
          return false;
        }
      }
      protected override OAuthConstants.EndPointType EndPointType
      {
        get
        {
          return OAuthConstants.EndPointType.AccessTokenRequest;
        }
      }
    }
    public class an_access_token_request_without_a_non_existant_request_token : OAuthRequestContext
    {
      [Fact]
      public void should_not_be_valid()
      {
        Assert.False(request.IsValid());
      }
      [Fact]
      public void should_have_an_invalid_token_error()
      {
        Assert.IsType<InvalidTokenError>(request.Error);
      }
      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get { return allBasicParameters.Concat(allAccessTokenParamters); }
      }
      protected override IRequestToken RequestToken
      {
        get
        {
          return null;
        }
      }
      protected override OAuthConstants.EndPointType EndPointType
      {
        get
        {
          return OAuthConstants.EndPointType.AccessTokenRequest;
        }
      }
    }
    public class a_valid_access_token_request:OAuthRequestContext
    {
      [Fact]
      public void should_not_be_valid()
      {
        Assert.True(request.IsValid());
      }
      [Fact]
      public void should_have_an_invalid_nonce_error()
      {
        Assert.Null(request.Error);
      }
      protected override IEnumerable<KeyValuePair<string, string>> Parameters
      {
        get
        {
          allBasicParameters["oauth_signature"] = "kd94hf93k423kf44%26hdhd0244k9j7ao03";
          allBasicParameters["oauth_timestamp"] = "1191242092";
          allBasicParameters["oauth_nonce"] = "dji430splmx33448";
          
          return allBasicParameters.Concat(allAccessTokenParamters);
        }
      }
      protected override OAuthConstants.EndPointType EndPointType
      {
        get
        {
          return OAuthConstants.EndPointType.AccessTokenRequest;
        }
      }
    }
  }
}