using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using OAuth.MVC.Library.Errors;
using OAuth.MVC.Library.Interfaces;

namespace OAuth.MVC.Library
{
  public class OAuthRequest:OAuthBase,IOAuthRequest
  {
    private readonly IEnumerable<KeyValuePair<string,Action<OAuthRequest,string>>> parameterActions = new Dictionary<string,Action<OAuthRequest,string>>
                                                   {
                                                     {OAuthConstants.PARAM_CONSUMER_KEY,(request, consumerKey)=>request.ConsumerKey = consumerKey},
                                                     {OAuthConstants.PARAM_NONCE,(request, nonce)=>request.Nonce = nonce},
                                                     {OAuthConstants.PARAM_SIGNATURE,(request, signature)=>request.Signature = signature},
                                                     {OAuthConstants.PARAM_SIGNATURE_METHOD,SetSignatureMethodFromString},
                                                     {OAuthConstants.PARAM_TIMESTAMP,(request, timestamp)=>request.TimeStamp = timestamp},
                                                     {OAuthConstants.PARAM_TOKEN,(request, token)=>request.Token = token},
                                                     {OAuthConstants.PARAM_VERSION,(request, version)=>request.Version = version},
                                                   };

    private readonly IOAuthRepository ioAuthRepository;

    private static void SetSignatureMethodFromString(OAuthRequest request, string signatureMethod)
    {
      switch(signatureMethod)
      {
        case OAuthConstants.VALUE_HMACSHA1:
          request.SignatureType = SignatureTypes.HMACSHA1;
          break;
        case OAuthConstants.VALUE_RSASHA1:
          request.SignatureType = SignatureTypes.RSASHA1;
          break;
        case OAuthConstants.VALUE_PLAINTEXT:
          request.SignatureType = SignatureTypes.PLAINTEXT;
          break;
        default:
          request.Error = new UnsupportedSignatureMethodError();
          break;
      }
    }

    public OAuthRequest(Uri url, string httpMethod,NameValueCollection parameters, IOAuthRepository ioAuthRepository, OAuthConstants.EndPointType endPointType):this(url,httpMethod,parameters.ToPairs(),ioAuthRepository,endPointType)
    {
    }

    public OAuthRequest(Uri url, string httpMethod, IEnumerable<KeyValuePair<string, string>> parameters, IOAuthRepository ioAuthRepository, OAuthConstants.EndPointType endPointType)
    {
      URL = url;
      HttpMethod = httpMethod;
      this.ioAuthRepository = ioAuthRepository;
      ReadParameters(parameters);
      CheckRequiredParameters(endPointType);
      isValid = ValidateRequest(endPointType);
    }

    private void CheckRequiredParameters(OAuthConstants.EndPointType endPointType)
    {
      if (
        string.IsNullOrEmpty(ConsumerKey)||
        string.IsNullOrEmpty(Signature)||
        string.IsNullOrEmpty(TimeStamp)||
        string.IsNullOrEmpty(Nonce)
        )
        Error = new MissingRequiredParameterError();
      if(endPointType==OAuthConstants.EndPointType.AccessTokenRequest)
      {
        if (string.IsNullOrEmpty(Token))
          Error = new MissingRequiredParameterError();
      }
    }

    private void ReadParameters(IEnumerable<KeyValuePair<string, string>> parameters)
    {
      foreach(var parameterAction in parameterActions)
      {
        KeyValuePair<string, Action<OAuthRequest, string>> parameterActionValue = parameterAction;
        var matchingParameters = parameters.Where(param => param.Key == parameterActionValue.Key);
        if(matchingParameters.Count()>1)
        {
          Error = new DuplicateOAuthProtocolParameterError();
          return;
        }
        if(matchingParameters.Count()>0)
        {
          var parameter = matchingParameters.First();
          //call the setter for this parameter
          parameterAction.Value.Invoke(this, parameter.Value);
        }
      }
    }


    /// <summary>
    /// Determines whether this request is valid.
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if this request is valid; otherwise, <c>false</c>.
    /// </returns>
    public bool ValidateRequest(OAuthConstants.EndPointType endPointType)
    {
      if(Error!=null)
        return false;
      //we don't care about these
      string normalisedRequetParameters;
      string normalisedUrl;

      
      var tokenSecret = "";
      if (endPointType == OAuthConstants.EndPointType.AccessTokenRequest)
      {
       RequestToken = Consumer.GetRequestToken(Token);
       if (RequestToken != null)
        {
          tokenSecret = RequestToken.Secret;
        }
       if (RequestToken == null || !RequestToken.IsAuthorized)
        {
          Error = new InvalidTokenError();
          return false;
        }
      }
      if(endPointType == OAuthConstants.EndPointType.AccessRequest)
      {
        AccessToken = Consumer.GetAccessToken(Token);
        if (AccessToken != null)
        {
          tokenSecret = AccessToken.Secret;
        }
        if (AccessToken == null)
        {
          Error = new InvalidTokenError();
          return false;
        }
      }
      int timeStampInt;
      if (!int.TryParse(TimeStamp, out timeStampInt))
      {
        Error = new InvalidNonceError();
        return false;
      }
      if (Consumer.TimeStamp > timeStampInt)
      {
        Error = new InvalidNonceError();
        return false;
      }
      if(Consumer.IsUsedNonce(TimeStamp,Nonce))
      {
        Error = new InvalidNonceError();
        return false;
      }
     
      var expectedSignature = GenerateSignature(URL, ConsumerKey, ConsumerSecret, Token, tokenSecret, HttpMethod, TimeStamp, Nonce, SignatureType, out normalisedUrl, out normalisedRequetParameters);
      if (expectedSignature == Signature)
      {
        Consumer.SaveNonce(TimeStamp,Nonce);
        return true;
      }
      
      Error = new InvalidSignatureError();
      return false;
    }

    internal SignatureTypes SignatureType { get; set; }
    internal string Nonce { get; set; }
    internal string TimeStamp { get; set; }
    internal string HttpMethod { get; set; }
    internal string Token { get; set; }
    internal string ConsumerKey { get; set; }
    internal Uri URL { get; set; }
    public IOAuthRequestError Error { get; set; }
    public IRequestToken RequestToken { get; set; }
    public IAccessToken AccessToken{ get; set; }
    

    private readonly bool isValid;

    private string Version
    {
      set
      {
        if (value != "1.0")
          Error = new UnsuportedParameterError();
      }
      
    }

    internal string Signature { get; set; }
    private string ConsumerSecret
    {
      get { return Consumer.Secret; }
    }

    public bool IsValid()
    {
      return isValid;
    }

    public IConsumer Consumer
    {
      get { return ioAuthRepository.GetConsumer(ConsumerKey); }
    }


  }

}