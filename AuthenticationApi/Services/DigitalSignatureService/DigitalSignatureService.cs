using AuthenticationApi.DataRepository.GenericRepository;
using AuthenticationApi.DataRepository.Models;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthenticationApi.Services
{
    public class DigitalSignatureService : IDigitalSignatureService
    {
        private readonly IMongoRepository<Profile> _profileRepository;
        private readonly IMongoRepository<User> _userRepository;
        private readonly IUserService _userService;
        private RSACryptoServiceProvider rsa;
        public DigitalSignatureService(IMongoRepository<Profile> profileRepository,
            IMongoRepository<User> userRepository,
            IUserService userService)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _userService = userService;

            rsa = new RSACryptoServiceProvider();
        }

        public async Task<(string, byte[])> GenerateSignature(Profile userProfile)
        {
            try
            {
                // Полученениче пользователя с базы данных, на основание данных из токена 
                var user = await _userRepository.FindOneAsync(doc => doc.Id == ObjectId.Parse(_userService.GetClaimValue(ClaimType.UserId)));

                // Создаем сертификат
                var certificate = GenerateCertificate(userProfile.UserId.ToString());

                // Создаем подпись для данных
                string signature = GenerateSignature(userProfile.UserIdentity);

                string certificatePassword = _userService.Decrypt(user.Password); // Пароль для защиты файла PKCS#12

                return (signature, SaveCertificateToFile(certificate, certificatePassword));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> VerifySignature(byte[] certificateBytes)
        {

            // Полученениче пользователя профиля с базы данных, на основание данных из токена 
            var user = await _userRepository.FindOneAsync(doc => doc.Id == ObjectId.Parse(_userService.GetClaimValue(ClaimType.UserId)));
            var userProfile = await _profileRepository.FindOneAsync(doc => doc.UserId == ObjectId.Parse(_userService.GetClaimValue(ClaimType.UserId)));

            var certificate = new X509Certificate2(certificateBytes, _userService.Decrypt(user.Password));

            // Преобразуем подпись из Base64 обратно в массив байтов
            byte[] signatureBytes = Convert.FromBase64String(userProfile.Signature);

            try
            {
                // Получаем открытый ключ сертификата
                RSA rsa = certificate.GetRSAPublicKey();

                // Преобразуем данные в массив байтов
                byte[] dataBytes = Encoding.UTF8.GetBytes(userProfile.UserIdentity);

                // Вычисляем хэш от данных
                byte[] hashBytes;
                using (var sha256 = SHA256.Create())
                {
                    hashBytes = sha256.ComputeHash(dataBytes);
                }

                // Проверяем подпись
                return rsa.VerifyHash(hashBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            catch (CryptographicException)
            {
                // В случае ошибки при верификации возвращаем false
                return false;
            }
        }

        private X509Certificate2 GenerateCertificate(string subjectName)
        {
            // Создаем запрос на сертификат
            var request = new CertificateRequest($"CN={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Создаем самоподписанный сертификат
            var certificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

            return certificate;
        }

        private byte[] SaveCertificateToFile(X509Certificate2 certificate, string password)
        {
            try
            {
                // Возвращаем массив байтов сертификатв формата PKCS#12
                return certificate.Export(X509ContentType.Pkcs12, password);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string GenerateSignature(string data)
        {
            try
            {
                // Преобразуем данные в массив байтов
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                // Вычисляем хэш от данных
                byte[] hashBytes;
                using (var sha256 = SHA256.Create())
                {
                    hashBytes = sha256.ComputeHash(dataBytes);
                }

                // Создаем подпись для хэша данных
                byte[] signatureBytes = rsa.SignHash(hashBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                // Возвращаем подпись в формате Base64
                return Convert.ToBase64String(signatureBytes);
            }
            finally
            {
                // Освобождаем ресурсы RSA
                rsa.Dispose();
            }
        }
    }
}
