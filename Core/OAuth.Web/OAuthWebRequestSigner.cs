using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using OAuth.Core;
using OAuth.Core.Signing;
using QueryParameter = System.Collections.Generic.KeyValuePair<string, string>;

namespace OAuth.Web
{
  public class OAuthWebRequestSigner
  {
    static readonly IList<string> oauthBaseSignatureParams = new List<string>
                                          {
                                            Parameters.OAuth_Nonce,
                                            Parameters.OAuth_Token,
                                            Parameters.OAuth_Timestamp,
                                            Parameters.OAuth_Signature_Method,
                                            Parameters.OAuth_Consumer_Key,
                                            Parameters.OAuth_Version,
        
                                          };

    private static readonly Dictionary<string, Func<IOAuthWebRequest, string>> ParameterFetchers = new Dictionary
      <string, Func<IOAuthWebRequest, string>>
                                                                                              {{Parameters.OAuth_Token, webRequest =>
                                                                                                {
                                                                                                        if (webRequest.Token !=null)
                                                                                                        {
                                                                                                          return
                                                                                                            webRequest.Token.Token;
                                                                                                        }
                                                                                                        return null;
                                                                                                      }
                                                                                                  },
                                                                                                {Parameters.OAuth_Nonce,webRequest =>webRequest.NonceGenerator.Generate()},
                                                                                                {Parameters.OAuth_Timestamp,webRequest =>webRequest.TimestampGenerator.Generate()},
                                                                                                {Parameters.OAuth_Signature_Method,_ =>SignatureGenerator.SignatureMethod},
                                                                                                {Parameters.OAuth_Consumer_Key,webRequest =>webRequest.ConsumerToken.Token},
                                                                                                {Parameters.OAuth_Version,_ => "1.0"}
                                                                                              };



  
    private static ISignatureGenerator SignatureGenerator;
    const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~+";
    public OAuthWebRequestSigner(ISignatureGenerator signatureGenerator)
    {
      SignatureGenerator = signatureGenerator;
    }

    public void SignWebRequest(IOAuthWebRequest request)
    {
      var secret = request.ConsumerToken.TokenSecret + "&";
      if (request.Token != null)
        secret = secret + request.Token.TokenSecret;
      var parameters = GetAllParameters(request);
      var signatureBase = GetSignatureBase(request,parameters);
      var signature = SignatureGenerator.Generate(secret, signatureBase);

      //sort out a clean request
      var oauthParameters = parameters.Where(p => oauthBaseSignatureParams.Contains(p.Key)).Concat(new []{new QueryParameter(Parameters.OAuth_Signature,signature)});
      var oauthExemptQueryParameters = HttpUtility.ParseQueryString(request.RequestUri.Query).ToQueryParameters().Where(p => !oauthBaseSignatureParams.Contains(p.Key));
      if (request.Method == "POST")
      {
        IEnumerable<QueryParameter> oauthExemptFormParameters = request.Form.Where(p => !oauthBaseSignatureParams.Contains(p.Key)).ToList();
        request.Form.Clear();
        request.Form.AddRange(oauthExemptFormParameters);
      }
      if(request.UseAuthorizationHeader)
      {
        request.Headers.Add(Parameters.OAuth_Authorization_Header,GenerateOAuthHeader(oauthParameters));
      }
      else if(request.Method == "POST")
      {
        request.Form.AddRange(oauthParameters);
      }
      else
      {
        oauthExemptQueryParameters = oauthExemptQueryParameters.Concat(oauthParameters);
      }
      request.SetUri(NormalizeUri(request.RequestUri) + "?" + GetQueryString(oauthExemptQueryParameters));




    }

    private static string GetQueryString(IEnumerable<QueryParameter> parameters)
    {
      return UriUtility.FormatQueryString(parameters.ToNameValueCollection());
    }

    private static string GenerateOAuthHeader(IEnumerable<QueryParameter> oauthParameters)
    {
      var builder = new StringBuilder();
      //if (Realm != null) builder.Append("realm=\"").Append(Realm).Append("\"");

      foreach (
        var parameter in oauthParameters.Where(p => p.Key != Parameters.Realm)
        )
      {
        if (builder.Length > 0) builder.Append(",");
        builder.Append(UriUtility.UrlEncode(parameter.Key)).Append("=\"").Append(
          UriUtility.UrlEncode(parameter.Value)).Append("\"");
      }

      builder.Insert(0, "OAuth ");

      return builder.ToString();
    
    }

    private string GetSignatureBase(IOAuthWebRequest request, List<QueryParameter> parameters)
    {
      
      var method = request.Method;
      return GetSignatureBase(method, request.RequestUri, parameters);
    }

    internal static List<QueryParameter> GetAllParameters(IOAuthWebRequest request)
    {
      var parameters = new List<QueryParameter>();
      parameters.AddRange(GetQueryStringParameters(request.RequestUri));
      if (request.Method == "POST")
      {
        parameters.AddRange(request.Form);
      }
      parameters.AddRange(GetSignatureBaseParameters(parameters, request));
      return parameters;
    }

    private static IEnumerable<QueryParameter> GetSignatureBaseParameters(List<QueryParameter> parameters, IOAuthWebRequest request)
    {
      var baseParameters = new List<QueryParameter>();
      
      foreach (var parameter in oauthBaseSignatureParams)
      {
        string expectedParameter = parameter;
        if (!parameters.Exists(p => p.Key == expectedParameter))
        {
          baseParameters.Add(new QueryParameter(parameter,ParameterFetchers[parameter].Invoke(request)));
        }
        else
        {
          var param = parameters.Single(kvp => kvp.Key == expectedParameter);
          parameters.Remove(param);
          baseParameters.Add(param);
        }
      }

      return baseParameters.Where(param => !(param.Key == Parameters.OAuth_Token && string.IsNullOrEmpty(param.Value)));
    }

    


    internal static IEnumerable<QueryParameter> GetQueryStringParameters(Uri requestUri)
    {
      var queryString = requestUri.Query;
      return HttpUtility.ParseQueryString(queryString).ToQueryParameters();
    }

    //taken from DevDefined.UriUtility
    public string GetSignatureBase(string httpMethod, Uri url, List<QueryParameter> parameters)
    {
      string normalizedRequestParameters = NormalizeRequestParameters(parameters);

      var signatureBase = new StringBuilder();
      signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());

      signatureBase.AppendFormat("{0}&", UrlEncode(NormalizeUri(url)));
      signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

      return signatureBase.ToString();
    }
    //taken from DevDefined.UriUtility
    public static string NormalizeRequestParameters(IEnumerable<QueryParameter> parameters)
    {
      IEnumerable<QueryParameter> orderedParameters = parameters
        .OrderBy(x => x.Key)
        .ThenBy(x => x.Value)
        .Select(
        x => new QueryParameter(x.Key, UrlEncode(x.Value)));

      var builder = new StringBuilder();

      foreach (var parameter in orderedParameters)
      {
        if (builder.Length > 0) builder.Append("&");

        builder.Append(parameter.Key).Append("=").Append(parameter.Value);
      }

      return builder.ToString();
    }

    //taken from DevDefined.UriUtility
    public static string UrlEncode(string value)
    {
      if (value == null) return null;

      var result = new StringBuilder();

      foreach (char symbol in value)
      {
        if (UnreservedChars.IndexOf(symbol) != -1)
        {
          result.Append(symbol);
        }
        else
        {
          result.Append('%' + String.Format("{0:X2}", (int)symbol));
        }
      }

      return result.ToString();
    }
    //taken from normalise request uri
    internal static string NormalizeUri(Uri uri)
    {
      string normalizedUrl = string.Format("{0}://{1}", uri.Scheme, uri.Host);

      if (!((uri.Scheme == "http" && uri.Port == 80) ||
            (uri.Scheme == "https" && uri.Port == 443)))
      {
        normalizedUrl += ":" + uri.Port;
      }

      return normalizedUrl + ((uri.AbsolutePath == "/") ? "" : uri.AbsolutePath);
    }
  }
}