using System.IO;
using System.Net;

namespace OAuth.Web
{
  public class OAuthTokenResponse
  {
    public OAuthTokenResponse(WebResponse httpWebResponse)
    {
      string tokenString;
        using(var stream = httpWebResponse.GetResponseStream())
        {
          var reader = new StreamReader(stream);
           tokenString = reader.ReadToEnd();
          reader.Close();
        }
      if(!string.IsNullOrEmpty(tokenString))
      {
        var parts = tokenString.Split('&');
        if(parts.Length==2)
        {
          Token = new OAuthToken {Token = parts[0].Trim().Replace("oauth_token=",""), TokenSecret = parts[1].Trim().Replace("oauth_token_secret=","")};
        }
      }
    }

    public OAuthToken Token { get; set; }
  }
}