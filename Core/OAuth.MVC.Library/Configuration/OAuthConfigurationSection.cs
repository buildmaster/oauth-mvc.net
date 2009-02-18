using System.Configuration;

namespace OAuth.MVC.Library.Configuration
{
  public class OAuthConfigurationSection:ConfigurationSection
  {
    public static OAuthConfigurationSection Read()
    {
      return (OAuthConfigurationSection) ConfigurationManager.GetSection("OAuth");   
    }
    [ConfigurationProperty("realm",DefaultValue="http://oauthsite.com",IsRequired = true)]
    public string Realm
    {
      get
      {
        return (string) this["realm"];
      }
      set
      {
        this["realm"] = value;
      }
    }
  }
}