namespace OAuth.Core.Signing
{
    public interface ISignatureGenerator
    {
        string SignatureMethod { get; }
        string Generate(string secret, string signatureBase);
    }
}