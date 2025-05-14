using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public ActionResult<IEnumerable<Profile>> GetAllProfiles()
        {
            return Ok(_profileService.GetAllProfiles());
        }

        [HttpGet("{id}")]
        public ActionResult<Profile> GetProfile(int id)
        {
            var profile = _profileService.GetProfile(id);
            if (profile == null)
            {
                return NotFound();
            }
            return Ok(profile);
        }

        [HttpGet("username/{username}")]
        public ActionResult<Profile> GetProfileByUsername(string username)
        {
            var profile = _profileService.GetProfileByUsername(username);
            if (profile == null)
            {
                return NotFound();
            }
            return Ok(profile);
        }

        [HttpPost]
        public ActionResult<Profile> CreateProfile(Profile profile)
        {
            var createdProfile = _profileService.CreateProfile(profile);
            _logger.LogInformation("Created profile with ID: {ProfileId}", createdProfile.Id);
            return CreatedAtAction(nameof(GetProfile), new { id = createdProfile.Id }, createdProfile);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProfile(int id, Profile profile)
        {
            if (id != profile.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingProfile = _profileService.GetProfile(id);
            if (existingProfile == null)
            {
                return NotFound();
            }

            if (_profileService.UpdateProfile(profile))
            {
                _logger.LogInformation("Updated profile with ID: {ProfileId}", profile.Id);
                return NoContent();
            }
            
            return BadRequest("Failed to update profile");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProfile(int id)
        {
            var profile = _profileService.GetProfile(id);
            if (profile == null)
            {
                return NotFound();
            }

            if (_profileService.DeleteProfile(id))
            {
                _logger.LogInformation("Deleted profile with ID: {ProfileId}", id);
                return NoContent();
            }
            
            return BadRequest("Failed to delete profile");
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
    }
} 