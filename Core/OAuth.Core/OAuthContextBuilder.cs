using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Linq;
using OAuth.Core.Interfaces;

namespace OAuth.Core
{
    public class OAuthContextBuilder : IOAuthContextBuilder
    {
        public IOAuthContext FromUri(string httpMethod, Uri uri)
        {
            uri = CleanUri(uri);

            if (httpMethod == null) throw new ArgumentNullException("httpMethod");
            if (uri == null) throw new ArgumentNullException("uri");

            return new OAuthContext
            {
                RawUri = uri,
                RequestMethod = httpMethod
            };
        }

        static Uri CleanUri(Uri uri)
        {
            // this is a fix for OpenSocial platforms sometimes appending an empty query string parameter
            // to their url.

            string originalUrl = uri.OriginalString;
            return originalUrl.EndsWith("&") ? new Uri(originalUrl.Substring(0, originalUrl.Length - 1)) : uri;
        }
        public IOAuthContext FromHttpRequest(HttpRequest request)
        {
            return FromHttpRequest(new HttpRequestWrapper(request));
        }
        public IOAuthContext FromHttpRequest(HttpRequestBase request)
        {
            var context = new OAuthContext
            {
                RawUri = CleanUri(request.Url),
                Cookies = CollectCookies(request),
                Headers = request.Headers,
                RequestMethod = request.HttpMethod,
                FormEncodedParameters = request.Form,
                QueryParameters = request.QueryString,
            };
            if (request.Headers.AllKeys.Contains("Authorization"))
            {
                context.AuthorizationHeaderParameters = UriUtility.GetHeaderParameters(request.Headers["Authorization"]).ToNameValueCollection();
            }

            return context;
        }

        public IOAuthContext FromWebRequest(HttpWebRequest request, Stream rawBody)
        {
            using (var reader = new StreamReader(rawBody))
            {
                return FromWebRequest(request, reader.ReadToEnd());
            }
        }

        public IOAuthContext FromWebRequest(HttpWebRequest request, string body)
        {
            var context = new OAuthContext
            {
                RawUri = CleanUri(request.RequestUri),
                Cookies = CollectCookies(request),
                Headers = request.Headers,
                RequestMethod = request.Method
            };

            if (request.Headers[HttpRequestHeader.ContentType] == "application/x-www-form-urlencoded")
            {
                context.FormEncodedParameters = HttpUtility.ParseQueryString(body);
            }

            return context;
        }

        static NameValueCollection CollectCookies(WebRequest request)
        {
            return CollectCookiesFromHeaderString(request.Headers[HttpRequestHeader.Cookie]);
        }

        static NameValueCollection CollectCookies(HttpRequestBase request)
        {
            return CollectCookiesFromHeaderString(request.Headers["Set-Cookie"]);
        }

        static NameValueCollection CollectCookiesFromHeaderString(string cookieHeader)
        {
            var cookieCollection = new NameValueCollection();

            if (!string.IsNullOrEmpty(cookieHeader))
            {
                string[] cookies = cookieHeader.Split(';');
                foreach (string cookie in cookies)
                {
                    //Remove the trailing and Leading white spaces
                    string strCookie = cookie.Trim();

                    var reg = new Regex(@"^(\S*)=(\S*)$");
                    if (reg.IsMatch(strCookie))
                    {
                        Match match = reg.Match(strCookie);
                        if (match.Groups.Count > 2)
                        {
                            cookieCollection.Add(match.Groups[1].Value,
                                                 HttpUtility.UrlDecode(match.Groups[2].Value).Replace(' ', '+'));
                            //HACK: find out why + is coming in as " "
                        }
                    }
                }
            }

            return cookieCollection;
        }
    }
}