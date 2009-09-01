using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using OAuth.Core.Interfaces;
using QueryParameter = System.Collections.Generic.KeyValuePair<string, string>;


namespace OAuth.Core
{
    public class OAuthContext : IOAuthContext
    {
        readonly BoundParameter _consumerKey;
        readonly BoundParameter _nonce;
        readonly BoundParameter _signature;
        readonly BoundParameter _signatureMethod;
        readonly BoundParameter _timestamp;
        readonly BoundParameter _token;
        readonly BoundParameter _tokenSecret;
        readonly BoundParameter _version;
        readonly BoundParameter _verifier;
        readonly BoundParameter _callback;
        NameValueCollection _authorizationHeaderParameters;
        NameValueCollection _cookies;
        NameValueCollection _formEncodedParameters;
        NameValueCollection _headers;
        string _normalizedRequestUrl;
        NameValueCollection _queryParameters;
        Uri _rawUri;

        public OAuthContext()
        {
            _consumerKey = new BoundParameter(Parameters.OAuth_Consumer_Key, this);
            _nonce = new BoundParameter(Parameters.OAuth_Nonce, this);
            _signature = new BoundParameter(Parameters.OAuth_Signature, this);
            _signatureMethod = new BoundParameter(Parameters.OAuth_Signature_Method, this);
            _timestamp = new BoundParameter(Parameters.OAuth_Timestamp, this);
            _token = new BoundParameter(Parameters.OAuth_Token, this);
            _tokenSecret = new BoundParameter(Parameters.OAuth_Token_Secret, this);
            _version = new BoundParameter(Parameters.OAuth_Version, this);
            _verifier = new BoundParameter(Parameters.OAuth_Verifier, this);
            _callback = new BoundParameter(Parameters.OAuth_Callback, this);

            FormEncodedParameters = new NameValueCollection();
            Cookies = new NameValueCollection();
            Headers = new NameValueCollection();
            AuthorizationHeaderParameters = new NameValueCollection();
            UseAuthorizationHeader = true;
        }

        public NameValueCollection Headers
        {
            get
            {
                if (_headers == null) _headers = new NameValueCollection();
                return _headers;
            }
            set { _headers = value; }
        }

        public NameValueCollection QueryParameters
        {
            get
            {
                if (_queryParameters == null) _queryParameters = new NameValueCollection();
                return _queryParameters;
            }
            set { _queryParameters = value; }
        }

        public NameValueCollection Cookies
        {
            get
            {
                if (_cookies == null) _cookies = new NameValueCollection();
                return _cookies;
            }
            set { _cookies = value; }
        }

        public NameValueCollection FormEncodedParameters
        {
            get
            {
                if (_formEncodedParameters == null) _formEncodedParameters = new NameValueCollection();
                return _formEncodedParameters;
            }
            set { _formEncodedParameters = value; }
        }

        public NameValueCollection AuthorizationHeaderParameters
        {
            get
            {
                if (_authorizationHeaderParameters == null) _authorizationHeaderParameters = new NameValueCollection();
                return _authorizationHeaderParameters;
            }
            set { _authorizationHeaderParameters = value; }
        }

        public Uri RawUri
        {
            get { return _rawUri; }
            set
            {
                _rawUri = value;

                NameValueCollection newParameters = HttpUtility.ParseQueryString(_rawUri.Query);

                // TODO: tidy this up, bit clunky

                foreach (string parameter in newParameters)
                {
                    QueryParameters[parameter] = newParameters[parameter];
                }

                _normalizedRequestUrl = UriUtility.NormalizeUri(_rawUri);
            }
        }

        public string NormalizedRequestUrl
        {
            get { return _normalizedRequestUrl; }
        }

        public string RequestMethod { get; set; }

        public string Nonce
        {
            get { return _nonce.Value; }
            set { _nonce.Value = value; }
        }

        public string Signature
        {
            get { return _signature.Value; }
            set { _signature.Value = value; }
        }

        public string SignatureMethod
        {
            get { return _signatureMethod.Value; }
            set { _signatureMethod.Value = value; }
        }

        public string Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }

        public string Version
        {
            get { return _version.Value; }
            set { _version.Value = value; }
        }

        public string Verifier
        {
            get { return _verifier.Value; }
            set { _verifier.Value = value; }
        }

        public string Callback
        {
            get { return _callback.Value; }
            set { _callback.Value = value; }
        }

        public bool UseAuthorizationHeader { get; set; }

        #region IToken Members

        public string Realm
        {
            get { return AuthorizationHeaderParameters[Parameters.Realm]; }
            set { AuthorizationHeaderParameters[Parameters.Realm] = value; }
        }

        public string ConsumerKey
        {
            get { return _consumerKey.Value; }
            set { _consumerKey.Value = value; }
        }

        public string Token
        {
            get { return _token.Value; }
            set { _token.Value = value; }
        }

        public string TokenSecret
        {
            get;
            set;
        }

        #endregion

        public Uri GenerateUri()
        {
            var builder = new UriBuilder(NormalizedRequestUrl);

            UriUtility.FormatQueryString(QueryParameters);

            builder.Query = UriUtility.FormatQueryString(QueryParameters);
            return builder.Uri;
        }

        public string GenerateUrl()
        {
            var builder = new UriBuilder(NormalizedRequestUrl) { Query = "" };

            return builder.Uri + "?" + UriUtility.FormatQueryString(QueryParameters);
        }

        public string GenerateOAuthParametersForHeader()
        {
            var builder = new StringBuilder();

            if (Realm != null) builder.Append("realm=\"").Append(Realm).Append("\"");

            foreach (
                var parameter in AuthorizationHeaderParameters.ToQueryParameters().Where(p => p.Key != Parameters.Realm)
                )
            {
                if (builder.Length > 0) builder.Append(",");
                builder.Append(UriUtility.UrlEncode(parameter.Key)).Append("=\"").Append(
                    UriUtility.UrlEncode(parameter.Value)).Append("\"");
            }

            builder.Insert(0, "OAuth ");

            return builder.ToString();
        }

        public Uri GenerateUriWithoutOAuthParameters()
        {
            var builder = new UriBuilder(NormalizedRequestUrl);

            NameValueCollection parameters = QueryParameters.ToQueryParameters()
                .Where(q => !q.Key.StartsWith(Parameters.OAuthParameterPrefix))
                .ToNameValueCollection();

            builder.Query = UriUtility.FormatQueryString(parameters);

            return builder.Uri;
        }


        public string GenerateSignatureBase()
        {
            if (Token == null)
            {
                Token = string.Empty;
            }

            if (string.IsNullOrEmpty(ConsumerKey))
            {
                throw Error.MissingRequiredOAuthParameter(this, Parameters.OAuth_Consumer_Key);
            }

            if (string.IsNullOrEmpty(SignatureMethod))
            {
                throw Error.MissingRequiredOAuthParameter(this, Parameters.OAuth_Signature_Method);
            }

            if (string.IsNullOrEmpty(RequestMethod))
            {
                throw Error.RequestMethodHasNotBeenAssigned("RequestMethod");
            }

            var allParameters = new List<QueryParameter>();

            if (FormEncodedParameters != null && RequestMethod == "POST") allParameters.AddRange(FormEncodedParameters.ToQueryParameters());
            if (QueryParameters != null) allParameters.AddRange(QueryParameters.ToQueryParameters());
            if (Cookies != null) allParameters.AddRange(Cookies.ToQueryParameters());
            if (AuthorizationHeaderParameters != null)
                allParameters.AddRange(AuthorizationHeaderParameters.ToQueryParameters().Where(q => q.Key != Parameters.Realm));

            // remove the signature parameter and the token parameter if it's not specified
            if (string.IsNullOrEmpty(Token))
                allParameters.RemoveAll(param => param.Key == Parameters.OAuth_Token);

            allParameters.RemoveAll(param => param.Key == Parameters.OAuth_Signature);

            //allParameters.RemoveAll(param => param.Key == Parameters.OAuth_Token && string.IsNullOrEmpty(param.Value));

            // build the uri

            return UriUtility.FormatParameters(RequestMethod, new Uri(NormalizedRequestUrl), allParameters);
        }

        #region Nested type: BoundParameter

        class BoundParameter
        {
            readonly IOAuthContext _context;
            readonly string _name;

            public BoundParameter(string name, IOAuthContext context)
            {
                _name = name;
                _context = context;
            }

            public string Value
            {
                get { return Collection[_name]; }
                set
                {
                    if (value == null)
                    {
                        Collection.Remove(_name);
                    }
                    else
                    {
                        Collection[_name] = value;
                    }
                }
            }

            NameValueCollection Collection
            {
                get
                {
                    var parameters = new NameValueCollection();
                    if (_context.UseAuthorizationHeader) parameters.Add(_context.AuthorizationHeaderParameters);
                    if (_context.RequestMethod != "GET") parameters.Add(_context.FormEncodedParameters);
                    parameters.Add(_context.QueryParameters);
                    return parameters;
                }
            }
        }

        #endregion
    }
}