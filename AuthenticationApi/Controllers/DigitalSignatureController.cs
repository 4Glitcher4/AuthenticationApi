using AuthenticationApi.DataRepository.GenericRepository;
using AuthenticationApi.DataRepository.Models;
using AuthenticationApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace AuthenticationApi.Controllers
{
    [ApiController]
    [Route("DigitalSignature")]
    [Authorize]
    public class DigitalSignatureController : ControllerBase
    {
        private readonly IDigitalSignatureService _digitalSignatureService;
        private readonly IUserService _userService;
        private readonly IMongoRepository<Profile> _profileRepository;

        public DigitalSignatureController(IDigitalSignatureService digitalSignatureService,
            IUserService userService,
            IMongoRepository<Profile> profileRepository)
        {
            _digitalSignatureService = digitalSignatureService;
            _userService = userService;
            _profileRepository = profileRepository;
        }

        [HttpGet("Generate")]
        public async Task<IActionResult> Generate()
        {
            try
            {
                var profile = await _profileRepository.FindOneAsync(doc => doc.UserId == ObjectId.Parse(_userService.GetClaimValue(ClaimType.UserId)));
                if (string.IsNullOrWhiteSpace(profile.UserIdentity))
                    return BadRequest(nameof(profile.UserIdentity) + " не должен быть пустям.");

                var (signature, certificateBytes) = await _digitalSignatureService.GenerateSignature(profile);
                profile.Signature = signature;

                await _profileRepository.ReplaceOneAsync(profile);

                MemoryStream stream = new MemoryStream(certificateBytes);

                return File(stream.ToArray(), @"application/x-pkcs12", $"{profile.UserId}.p12");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Verify")]
        public async Task<ActionResult<bool>> VerifySignature(IFormFile certificateFile)
        {
            try
            {
                if (certificateFile == null || certificateFile.Length == 0)
                    return BadRequest("Файл не был загружен.");

                var profile = await _profileRepository.FindOneAsync(doc => doc.UserId == ObjectId.Parse(_userService.GetClaimValue(ClaimType.UserId)));

                // Считываем содержимое загруженного файла
                using (var memoryStream = new MemoryStream())
                {
                    await certificateFile.CopyToAsync(memoryStream);
                    byte[] certificateBytes = memoryStream.ToArray();

                    // Далее используем сертификат для верификации подписи
                    bool isSignatureValid = await _digitalSignatureService.VerifySignature(certificateBytes);

                    // Возвращаем сообщение о успешной верификации
                    return Ok(isSignatureValid);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
