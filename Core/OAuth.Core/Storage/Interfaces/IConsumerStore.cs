using System.Security.Cryptography.X509Certificates;
using OAuth.Core.Interfaces;

namespace OAuth.Core.Storage.Interfaces
{
    public interface IConsumerStore
    {
        bool IsConsumer(IConsumer consumer);
        void SetConsumerSecret(IConsumer consumer, string consumerSecret);
        string GetConsumerSecret(IConsumer consumer);
        void SetConsumerCertificate(IConsumer consumer, X509Certificate2 certificate);
        X509Certificate2 GetConsumerCertificate(IConsumer consumer);
    }
}