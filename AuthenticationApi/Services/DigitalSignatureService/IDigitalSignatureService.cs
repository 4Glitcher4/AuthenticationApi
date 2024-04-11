using AuthenticationApi.DataRepository.Models;
using System.Security.Cryptography.X509Certificates;

namespace AuthenticationApi.Services
{
    public interface IDigitalSignatureService
    {
        Task<(string, byte[])> GenerateSignature(Profile userProfile);
        Task<bool> VerifySignature(byte[] certificateBytes);
    }
}