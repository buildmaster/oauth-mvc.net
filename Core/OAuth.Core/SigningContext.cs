using System.Security.Cryptography;

namespace OAuth.Core
{
    public class SigningContext
    {
        public AsymmetricAlgorithm Algorithm { get; set; }
        public string ConsumerSecret { get; set; }
        public string SignatureBase { get; set; }
    }
}