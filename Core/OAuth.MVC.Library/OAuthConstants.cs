namespace OAuth.MVC.Library
{
  public class OAuthConstants
  {
    
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

    public enum EndPointType
    {
      AccessTokenRequest,
      RequestTokenRequest,
      AccessRequest,
    }
  }
}