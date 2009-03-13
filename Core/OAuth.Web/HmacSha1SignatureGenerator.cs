using System;
using System.Security.Cryptography;
using System.Text;

namespace OAuth.Web
{
  public class HmacSha1SignatureGenerator:ISignatureGenerator
  {
    public string SignatureMethod
    {
      get { return "HMAC-SHA1"; }
    }

    public string Generate(string secret, string signatureBase)
    {
      var hashAlgorithm = new HMACSHA1 {Key = Encoding.ASCII.GetBytes(secret)};
         return ComputeHash(hashAlgorithm, signatureBase);
    }
    static string ComputeHash(HashAlgorithm hashAlgorithm, string data)
    {
      if (hashAlgorithm == null)
      {
        throw new ArgumentNullException("hashAlgorithm");
      }

      if (string.IsNullOrEmpty(data))
      {
        throw new ArgumentNullException("data");
      }

      byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
      byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

      return Convert.ToBase64String(hashBytes);
    }
  }
}