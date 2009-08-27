using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using DevDefined.OAuth.Framework;

namespace OAuth.Web
{
    public class OAuthHttpWebRequest : WebRequest, IOAuthWebRequest
    {
        private readonly IHttpWebRequestFactory _httpWebRequestFactory;
        private string _uri;
        private List<KeyValuePair<string, string>> _formParams;
        private WebHeaderCollection _headers;
        private readonly OAuthWebRequestSigner _signer;

        public OAuthHttpWebRequest(IHttpWebRequestFactory httpWebRequestFactory, OAuthWebRequestSigner signer)
        {
            _httpWebRequestFactory = httpWebRequestFactory;
            _signer = signer;
            Timeout = int.MinValue;
            Compression = DecompressionMethods.None;
        }
        public OAuthHttpWebRequest()
        {
            _httpWebRequestFactory = new StandardHttpWebRequestFactory();
            _signer = new OAuthWebRequestSigner(new HmacSha1SignatureGenerator());
            TimestampGenerator = new StandardTimestampGenerator();
            NonceGenerator = new GuidNonceGenerator();
            Timeout = int.MinValue;
            Compression = DecompressionMethods.None;
        }

        public class GuidNonceGenerator : INonceGenerator
        {
            public string Generate()
            {
                return Guid.NewGuid().ToString();
            }
        }

        public class StandardTimestampGenerator : ITimestampGenerator
        {
            public string Generate()
            {
                return DateTime.Now.Epoch().ToString();
            }
        }

        public class StandardHttpWebRequestFactory : IHttpWebRequestFactory
        {
            public WebResponse Create(IOAuthWebRequest request)
            {
                var httpWebRequest = (HttpWebRequest) WebRequest.Create(request.RequestUri);
                httpWebRequest.AutomaticDecompression = request.Compression;
                httpWebRequest.UserAgent = request.UserAgent ?? "OAuth WebRequest";
                if (request.Timeout != int.MinValue)
                    httpWebRequest.Timeout = request.Timeout;
                if (request.Proxy != null)
                    httpWebRequest.Proxy = request.Proxy;
                httpWebRequest.Method = request.Method;
                if (request.Method == "POST" || request.Method == "PUT")
                {
                    if (request.Form.Count > 0)
                    {
                        var formPost = GetPostString(request.Form);
                        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                        var swRequestWriter = new
                        StreamWriter(httpWebRequest.GetRequestStream());
                        swRequestWriter.Write(formPost);
                        swRequestWriter.Close();
                    }
                }

                if (request.Headers.Count > 0)
                {
                    httpWebRequest.Headers.Add(request.Headers);
                }
                return httpWebRequest.GetResponse();
            }

            private static string GetPostString(IEnumerable<KeyValuePair<string, string>> form)
            {
                var postString = new StringBuilder();
                foreach (var valuePair in form)
                {
                    if (postString.Length != 0)
                    {
                        postString.Append("&");
                    }
                    postString.Append(HttpUtility.UrlEncode(valuePair.Key));
                    postString.Append("=");
                    postString.Append(HttpUtility.UrlEncode(valuePair.Value));
                }
                return postString.ToString();
            }
        }

        public override Uri RequestUri
        {
            get
            {
                return new Uri(_uri);
            }
        }

        public void SetUri(string newUri)
        {
            _uri = newUri;
        }
        public override WebResponse GetResponse()
        {
            _signer.SignWebRequest(this);
            return _httpWebRequestFactory.Create(this);
        }
        public INonceGenerator NonceGenerator { get; set; }
        public ITimestampGenerator TimestampGenerator { get; set; }
        public IOAuthToken ConsumerToken { get; set; }
        public override string Method { get; set; }
        public List<KeyValuePair<string, string>> Form
        {
            get
            {
                if (_formParams == null)
                    _formParams = new List<KeyValuePair<string, string>>();
                return _formParams;
            }
        }

        public override WebHeaderCollection Headers
        {
            get
            {
                if (_headers == null)
                    _headers = new WebHeaderCollection();
                return _headers;
            }
            set
            {
                _headers = value;
            }
        }


        public IOAuthToken Token { set; get; }

        public bool UseAuthorizationHeader
        {
            get { return false; }
        }

        public override IWebProxy Proxy
        {
            get;
            set;
        }

        public string UserAgent
        {
            get;
            set;
        }
        public override sealed int Timeout { get; set; }
        public DecompressionMethods Compression
        {
            get; set;
        }
    }
}