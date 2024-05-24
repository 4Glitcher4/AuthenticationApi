using AuthenticationApi.DataRepository.GenericRepository;
using AuthenticationApi.DataRepository.Models;
using AuthenticationApi.ModelsDto;
using AuthenticationApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace AuthenticationApi.Controllers
{
    [ApiController]
    [Route("Profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IMongoRepository<Profile> _profileRepository;
        private readonly IUserService _userService;

        public ProfileController(IMongoRepository<Profile> profileRepository,
            IUserService userService)
        {
            _profileRepository = profileRepository;
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<Profile> Get()
        {
            try
            {
                return Ok(_profileRepository.FindOne(doc => doc.UserId == ObjectId.Parse(_userService.GetClaimValue(ClaimType.UserId))));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public IActionResult Put(ProfilePutDto profilePutDto)
        {
            var profile = _profileRepository.FindOne(doc => doc.UserId == ObjectId.Parse(_userService.GetClaimValue(ClaimType.UserId)));

            if (profile == null)
                return NotFound(profile.GetType().Name + " not found.");

            profile.Name = profilePutDto.Name ?? profile.Name;
            profile.Description = profilePutDto.Description ?? profile.Description;
            profile.UserIdentity = profilePutDto.UserIdentity ?? profile.UserIdentity;

            _profileRepository.ReplaceOne(profile);

            return NoContent();
        }
    }
}
