using System.Text.RegularExpressions;

namespace OAuth.MVC.Library
{
  public class OAuthConstants
  {
    public const string HEADER_AUTHORIZATION = "Authorization";

    public const string VALUE_HMACSHA1 = "HMAC-SHA1";
    public const string VALUE_PLAINTEXT = "PLAINTEXT";
    public const string VALUE_RSASHA1 = "RSA-SHA1";

    public const string PARAM_OAUTH_REALM = "realm";
    public const string PARAM_CONSUMER_KEY = "oauth_consumer_key";
    public const string PARAM_SIGNATURE_METHOD = "oauth_signature_method";
    public const string PARAM_SIGNATURE = "oauth_signature";
    public const string PARAM_TIMESTAMP = "oauth_timestamp";
    public const string PARAM_NONCE = "oauth_nonce";
    public const string PARAM_VERSION = "oauth_version";
    public const string PARAM_TOKEN = "oauth_token";
    public const string PARAM_CALLBACK = "oauth_callback";

    /// <summary>
    /// The OAuth RFC-2617 auth-scheme
    /// </summary>
    public const string OAuthAuthScheme = "OAuth";

    /// <summary>
    /// The OAuth credentials prefix for the Authorization HTTP header
    /// </summary>
    public static readonly Regex OAuthCredentialsRegex = new Regex(@"^" + OAuthAuthScheme + @"\s+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// String escape sequences
    /// </summary>
    public static readonly Regex StringEscapeSequence = new Regex(@"\\([""'\0abfnrtv]|U[0-9a-fA-F]{8}|u[0-9a-fA-F]{4}|x[0-9a-fA-F]+)", RegexOptions.Compiled);



    public enum EndPointType
    {
      AccessTokenRequest,
      RequestTokenRequest,
      AccessRequest,
    }
  }
}