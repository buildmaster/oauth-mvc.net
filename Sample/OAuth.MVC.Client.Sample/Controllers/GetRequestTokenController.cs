using System;
using System.IO;
using System.Net;
using System.Web;
using OAuth.MVC.Client.Sample.Interfaces;
using OAuth.MVC.Library;

namespace OAuth.MVC.Client.Sample.Controllers
{
  public class GetRequestTokenController
  {
    private readonly IGetRequestTokenView view;

    public GetRequestTokenController(IGetRequestTokenView view)
    {
      this.view = view;
      view.NextEnabled = false;
      view.GetRequestToken += GetRequestToken;
    }

    private void GetRequestToken(object sender, EventArgs e)
    {
      view.Output = "";
      view.Output += "Begining to get request \n";
      var consumerKey = view.ConsumerKey;
      var consumerSecret = view.ConsumerSecret;
      if(string.IsNullOrEmpty(consumerKey))
      {
        view.Output += "No consumer key supplied\n";
        return;  
      }
      view.Output += String.Format("Using consumer key {0}\n", consumerKey);
      if (string.IsNullOrEmpty(consumerSecret))
      {
        view.Output += "No consumer secret supplied\n";
        return;
      }
      view.Output += String.Format("Using consumer secret {0}\n", consumerSecret);
      var requestTokenUrl = view.RequestTokenUrl;
      if(string.IsNullOrEmpty(requestTokenUrl))
      {
        view.Output += "No request token url supplied\n";
        return;
      }
      var oauthBase = new OAuthBase();
      var nonce = oauthBase.GenerateNonce();
      view.Output += String.Format("Generated Nonce: {0}\n", nonce);
      var timestamp = oauthBase.GenerateTimeStamp();
      view.Output += String.Format("Generated Timestamp: {0}\n", timestamp);
      var url = new Uri(requestTokenUrl);
      string normalizedUrl;
      string normalizedRequestParams;
      var signature = new OAuthBase().GenerateSignature(url, consumerKey, consumerSecret, "", "", "POST", timestamp,
                                                        nonce, OAuthBase.SignatureTypes.HMACSHA1,out normalizedUrl,out normalizedRequestParams);
      view.Output += string.Format("generated signature: {0}\n", signature);
      var queryString = "";
      queryString += queryString.AppendQuery(OAuthConstants.PARAM_CONSUMER_KEY, consumerKey);
      queryString += "&"+queryString.AppendQuery(OAuthConstants.PARAM_NONCE, nonce);
      queryString += "&" + queryString.AppendQuery(OAuthConstants.PARAM_TIMESTAMP, timestamp);
      queryString += "&" + queryString.AppendQuery(OAuthConstants.PARAM_SIGNATURE_METHOD, OAuthConstants.VALUE_HMACSHA1);
      queryString += "&" + queryString.AppendQuery(OAuthConstants.PARAM_SIGNATURE, signature);
      
      var fullUrl = new Uri(requestTokenUrl + "?" + queryString);
      view.Output += string.Format("generated url: {0}\n", fullUrl);

      WebRequest webRequest = WebRequest.Create(fullUrl);
      webRequest.Method = "POST";
      view.Output += "Making call\n";
      try
      {
        var response = webRequest.GetResponse();
        view.Output += "*****************RESPONSE************************\n";
        using (var stream = response.GetResponseStream())
        {
          var streamReader = new StreamReader(stream);
          var responseText = streamReader.ReadToEnd();
          view.Output += responseText + "\n";
          var responseTokenValuePairs = responseText.Split('&');
          if (responseTokenValuePairs.Length < 2)
          {
            view.Output += "Error: too few parameters returned in response\n";
            return;
          }
          if (responseTokenValuePairs.Length > 2)
          {
            view.Output += "Error: too many parameters returned in response\n";
            return;
          }
          foreach(var responseTokenPair in responseTokenValuePairs)
          {
            var elements = responseTokenPair.Split('=');
            if (elements.Length != 2)
            {
              view.Output += "Something went wrong decoding the output got wrong number of elements in key value pair\n";
              return;
            }
            if (elements[0] == OAuthConstants.PARAM_TOKEN)
            {
              view.Token = elements[1];
            }
            else
            {
              view.TokenSecret = elements[1];
            }

          }
          view.NextEnabled = true;
        }
      }
      catch (WebException wex)
      {
        view.Output += String.Format("got an error from server {0}\n", wex.Message);
        var response = wex.Response;
        using (var stream = response.GetResponseStream())
        {
          var streamReader = new StreamReader(stream);
          view.Output += streamReader.ReadToEnd();
        }

      }
      
    
    
    }
  }
  static class HelperMethods
  {
    public static string AppendQuery(this string queryString,string param,string value)
    {
      param = HttpUtility.UrlEncode(param);
      value = HttpUtility.UrlEncode(value);
      return String.Format("{0}={1}", param, value);
    }
  }
}