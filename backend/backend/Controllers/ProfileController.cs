using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profileService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ProfileService profileService, ILogger<ProfileController> logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profile>>> GetAllProfiles()
        {
            return Ok(await _profileService.GetAllProfiles());
        }

        [HttpGet("{uuid}")]
        public async Task<ActionResult<Profile>> GetProfile(string uuid)
        {
            var profile = await _profileService.GetProfileByUuid(uuid);
            if (profile == null)
                return NotFound();
            return Ok(profile);
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<Profile>> GetProfileByUsername(string username)
        {
            var profile = await _profileService.GetProfileByUsername(username);
            if (profile == null)
                return NotFound();
            return Ok(profile);
        }

        [HttpPost]
        public async Task<ActionResult<Profile>> CreateProfile(ProfileDTO profileDTO)
        {
            var createdProfile = await _profileService.CreateProfile(profileDTO);
            _logger.LogInformation("Created profile with UUID: {Uuid}", createdProfile.Uuid);
            return Ok(createdProfile);
        }

        [HttpPut("{uuid}")]
        public async Task<IActionResult> UpdateProfile(string uuid, ProfileDTO profileDTO)
        {
            if (!await _profileService.ProfileExists(uuid))
                return NotFound();

            if (await _profileService.UpdateProfile(uuid, profileDTO))
            {
                _logger.LogInformation("Updated profile with UUID: {uuid}", uuid);
                return Ok();
            }

            return BadRequest("Failed to update profile");
        }

        [HttpDelete("{uuid}")]
        public async Task<IActionResult> DeleteProfile(string uuid)
        {
            if (!await _profileService.ProfileExists(uuid))
                return NotFound();

            if (await _profileService.DeleteProfile(uuid))
            {
                _logger.LogInformation("Deleted profile with UUID: {uuid}", uuid);
                return Ok();
            }

            return BadRequest("Failed to delete profile");
        }
    }
} 