using System;
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
        if(parts.Length>2)
        {
            var token = new OAuthToken();
            foreach(var part in parts)
            {
                var tokenparts = part.Split('=');
                if(tokenparts.Length==2)
                {
                    if(tokenparts[0].Equals("oauth_token",StringComparison.OrdinalIgnoreCase))
                    {
                        token.Token = tokenparts[1];    
                       }
                    else if (tokenparts[0].Equals("oauth_token_secret", StringComparison.OrdinalIgnoreCase))
                    {
                        token.TokenSecret = tokenparts[1];    
                    }
                }

            }
          
        }
      }
    }

    public OAuthToken Token { get; set; }
  }
}