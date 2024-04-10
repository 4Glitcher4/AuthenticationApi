using AuthenticationApi.DataRepository.GenericRepository;
using AuthenticationApi.DataRepository.Models;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Text;

namespace AuthenticationApi.Services
{
    public class DigitalSignatureService : IDigitalSignatureService
    {
        private readonly string privatePemFile = Directory.GetCurrentDirectory() + "/Services/RSAService/RSAKeys/private_key.pem";
        private readonly string publicPemFile = Directory.GetCurrentDirectory() + "/Services/RSAService/RSAKeys/public_key.pem";

        private readonly IMongoRepository<Profile> _profileRepository;
        private readonly IUserService _userService;
        public DigitalSignatureService(IMongoRepository<Profile> profileRepository,
            IUserService userService)
        {
            _profileRepository = profileRepository;
            _userService = userService;
        }

        public async Task GenerateDigitalSignature()
        {
            try
            {
                // Загружаем PEM-ключи
                var (publicKey, privateKey) = LoadPem();

                // Подписываем данные
                string dataToSign = "Hello, World!";
                byte[] signature = SignData(Encoding.UTF8.GetBytes(dataToSign), privateKey);

                Console.WriteLine("Данные: " + dataToSign);
                Console.WriteLine("Подпись: " + Convert.ToBase64String(signature));

                // Проверяем подпись
                bool isVerified = VerifyData(Encoding.UTF8.GetBytes(dataToSign), signature, publicKey);
                Console.WriteLine("Подпись верифицирована: " + isVerified);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task<bool> ValidateDigitalSignature()
        {
            throw new NotImplementedException();
        }

        private byte[] SignData(byte[] data, AsymmetricKeyParameter privateKey)
        {
            ISigner signer = SignerUtilities.GetSigner("SHA-256withRSA");
            signer.Init(true, privateKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
        }

        private bool VerifyData(byte[] data, byte[] signature, AsymmetricKeyParameter publicKey)
        {
            ISigner signer = SignerUtilities.GetSigner("SHA-256withRSA");
            signer.Init(false, publicKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(signature);
        }

        private (AsymmetricKeyParameter Public, AsymmetricKeyParameter Private) LoadPem()
        {
            AsymmetricKeyParameter publicKey = null;
            AsymmetricKeyParameter privateKey = null;

            using (TextReader privateKeyReader = File.OpenText(privatePemFile))
            {
                PemReader pemReader = new PemReader(privateKeyReader);
                privateKey = (AsymmetricKeyParameter)pemReader.ReadObject();
            }

            using (TextReader publicKeyReader = File.OpenText(publicPemFile))
            {
                PemReader pemReader = new PemReader(publicKeyReader);
                publicKey = (AsymmetricKeyParameter)pemReader.ReadObject();
            }

            return (Public: publicKey, Private: privateKey);
        }
    }
}
