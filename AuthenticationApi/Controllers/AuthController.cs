using AuthenticationApi.DataRepository.GenericRepository;
using AuthenticationApi.DataRepository.Models;
using AuthenticationApi.ModelsDto;
using AuthenticationApi.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace AuthenticationApi.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoRepository<User> _userRepository;
        private readonly IMongoRepository<Profile> _profileRepository;
        private readonly IUserService _userService;
        private readonly ISmtpService _smtpService;

        public AuthController(IMongoRepository<User> userRepository,
            IMongoRepository<Profile> profileRepository,
            IUserService userService,
            ISmtpService smtpService)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _userService = userService;
            _smtpService = smtpService;

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                var user = await _userRepository.FindOneAsync(doc => doc.Email == userDto.Email);
                if (user != null)
                    return BadRequest("This user alreary exist.");

                user = new User
                {
                    Email = userDto.Email,
                    Password = _userService.Encrypt(userDto.Password),
                };

                var profile = new Profile
                {
                    Name = "",
                    Description = "",
                    UserId = user.Id,
                    IsVerify = false,
                };

                await _userRepository.InsertOneAsync(user);
                await _profileRepository.InsertOneAsync(profile);

                var token = _userService.CreateToken(new TokenSettings
                {
                    TokenLifeTime = DateTime.Now.AddMinutes(10),
                    UserId = user.Id.ToString(),
                });

                _smtpService.Send($"http://localhost:5181/Auth/Callback/{token}", new SendDto
                {
                    Login = userDto.Email,
                    SendType = SendType.VerifyUser,
                    Subject = "Verify register."
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            try
            {
                var user = await _userRepository.FindOneAsync(doc => doc.Email == userDto.Email);
                if (user == null)
                    return NotFound(user.GetType().Name + " not found.");
                if (_userService.Encrypt(userDto.Password) != user.Password)
                    return BadRequest();

                return Ok(new
                {
                    accessToken = _userService.CreateToken(new TokenSettings
                    {
                        UserId = user.Id.ToString(),
                        TokenLifeTime = DateTime.Now.AddDays(1),
                    }),
                    refreshToken = _userService.CreateToken(new TokenSettings
                    {
                        UserId = user.Id.ToString(),
                        TokenLifeTime = DateTime.Now.AddDays(5),
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Callback/{token}")]
        public async Task<IActionResult> Callback(string token)
        {
            try
            {
                var userId = _userService.GetClaimValue(token, ClaimType.UserId);
                var profile = await _profileRepository.FindOneAsync(doc => doc.UserId == ObjectId.Parse(userId));

                profile.IsVerify = true;

                await _profileRepository.ReplaceOneAsync(profile);

                return Redirect("http://localhost:5181/swagger");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
