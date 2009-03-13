namespace OAuth.Web
{


  public class OAuthToken : IOAuthToken
  {
    public string Token { get; set; }

    public string TokenSecret { get; set; }
  }
}