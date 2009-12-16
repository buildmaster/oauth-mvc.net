using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace OAuth.Web
{
    public class OAuthTokenResponse
    {
        public OAuthTokenResponse(WebResponse httpWebResponse)
        {
            using (var stream = httpWebResponse.GetResponseStream())
            {
                var reader = new StreamReader(stream);
                ResponseBody = reader.ReadToEnd();
                reader.Close();
            }

            if (string.IsNullOrEmpty(ResponseBody))
            {
                return;
            }

            Items = new NameValueCollection();

            foreach (string item in ResponseBody.Split('&'))
            {
                if (item.IndexOf('=') > -1)
                {
                    string[] temp = item.Split('=');
                    Items.Add(temp[0], temp[1]);
                }
                else
                {
                    Items.Add(item, string.Empty);
                }
            }

            Token = new OAuthToken() { Token = Items["oauth_token"], TokenSecret = Items["oauth_token_secret"] };
        }

        public string ResponseBody { get; private set; }

        public NameValueCollection Items { get; private set; }

        public OAuthToken Token { get; private set; }
    }
}