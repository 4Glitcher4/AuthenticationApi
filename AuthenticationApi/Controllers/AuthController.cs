using AuthenticationApi.DataRepository.GenericRepository;
using AuthenticationApi.DataRepository.Models;
using AuthenticationApi.ModelsDto;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoRepository<User> _userRepository;
        private readonly IMongoRepository<Profile> _profileRepository;

        public AuthController(IMongoRepository<User> userRepository,
            IMongoRepository<Profile> profileRepository)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                var user = await _userRepository.FindOneAsync(doc => doc.Email == userDto.Email);
                if (user != null)
                    return BadRequest("This user alreary exist.");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
