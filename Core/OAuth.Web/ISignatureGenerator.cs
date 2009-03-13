namespace OAuth.Web
{
  public interface ISignatureGenerator
  {
    string SignatureMethod { get; }
    string Generate(string secret, string signatureBase);
  }
}