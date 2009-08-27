using System.Web;
using Xunit;

namespace OAuth.Web.Tests
{
  public class HmacSha1SignatureGeneratorSpecifications
  {
    [Fact]
    public void signature_should_be_created_correctly()
    {
      var signatureGenerator = new HmacSha1SignatureGenerator();
      var signature = signatureGenerator.Generate("kd94hf93k423kf44&", "GET&https%3A%2F%2Fwww.google.com%2Faccounts%2FOAuthGetRequestToken&oauth_consumer_key%3Ddpf43f3p2l4k3l03%26oauth_nonce%3D788ce10e1b88b62e57a75dd3cf7c8fef%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1236209805%26oauth_version%3D1.0%26scope%3D");
      Assert.Equal(HttpUtility.UrlDecode("leyGd1u9SwdDP7189awXNOQoh%2Bo%3D"), signature);

    }
    [Fact]
    public void signature_should_be_created_correctly_for_POST()
    {
      var signatureGenerator = new HmacSha1SignatureGenerator();
      var signature = signatureGenerator.Generate("kd94hf93k423kf44&", "POST&https%3A%2F%2Fwww.google.com%2Faccounts%2FOAuthGetRequestToken&oauth_consumer_key%3Ddpf43f3p2l4k3l03%26oauth_nonce%3D788ce10e1b88b62e57a75dd3cf7c8fef%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1236209805%26oauth_version%3D1.0%26scope%3D");
      Assert.Equal("YLfUOkBC8R8jai6DJcaKNYRz9yg=", signature);

    }
    [Fact]
    public void signature_should_be_created_correctly_with_token()
    {
      var signatureGenerator = new HmacSha1SignatureGenerator();
      var signature = signatureGenerator.Generate("kd94hf93k423kf44&0685bd9184jfhq22", 
        "GET&https%3A%2F%2Fwww.google.com%2Faccounts%2FOAuthGetRequestToken&oauth_consumer_key%3Ddpf43f3p2l4k3l03%26oauth_nonce%3D788ce10e1b88b62e57a75dd3cf7c8fef%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1236209805%26oauth_token%3Dad180jjd733klru7%26oauth_version%3D1.0%26scope%3D");
      Assert.Equal("ZVCY25+8sZd6pQGhm8TwqB1MzF0=", signature);

    }
    [Fact]
    public void signature_should_be_created_correctly_for_POST_with_token()
    {
      var signatureGenerator = new HmacSha1SignatureGenerator();
      var signature = signatureGenerator.Generate("kd94hf93k423kf44&0685bd9184jfhq22",
        "POST&https%3A%2F%2Fwww.google.com%2Faccounts%2FOAuthGetRequestToken&oauth_consumer_key%3Ddpf43f3p2l4k3l03%26oauth_nonce%3D788ce10e1b88b62e57a75dd3cf7c8fef%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1236209805%26oauth_token%3Dad180jjd733klru7%26oauth_version%3D1.0%26scope%3D");
      Assert.Equal("lm9WewpOaP17tgsmNONhnYt1oaA=", signature);

    }
      [Fact]
      public void correct_signature_for_base_string()
      {
          var signatureGenerator = new HmacSha1SignatureGenerator();
          var signature = signatureGenerator.Generate("NDYWNGMZMZG5NWZHNDYZMJHIMTM4MD&",
                                                      "POST&http%3A%2F%2Fapi.xero.test%2Foauth%2FRequestToken&oauth_callback%3Dhttp%253A%252F%252Fapi.xero.test%252FTestHarness%252FAuthorised%26oauth_consumer_key%3DMJRMYWE1ODIWMMEZNGRKMGI5MDG0ZD%26oauth_nonce%3Dc7fef605-2f23-48f0-9b7a-ecca28c64941%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1249607199%26oauth_version%3D1.0");
          Assert.Equal("", signature);

      }
  }
}