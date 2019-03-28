using System.Security.Cryptography.X509Certificates;

namespace Thorium.Aggregator.Services.Certificate
{
    public interface ICertificateService
    {
        X509Certificate2 GetCertificateFromKeyVault(string vaultCertificateName);
    }
}