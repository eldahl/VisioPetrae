using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Security.Claims;
using backend.Controllers;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profileService;
        private readonly JwtService _jwtService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ProfileService profileService, JwtService jwtService, ILogger<ProfileController> logger)
        {
            _profileService = profileService;
            _jwtService = jwtService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profile>>> GetAllProfiles()
        {
            return Ok(await _profileService.GetAllProfiles());
        }

        [Authorize]
        [HttpGet("{uuid}")]
        public async Task<ActionResult<Profile>> GetProfile(string uuid)
        {
            var profile = await _profileService.GetProfileByUuid(uuid);
            if (profile == null)
                return NotFound();
            return Ok(profile);
        }

        [Authorize]
        [HttpGet("username/{username}")]
        public async Task<ActionResult<Profile>> GetProfileByUsername(string username)
        {
            var profile = await _profileService.GetProfileByUsername(username);
            if (profile == null)
                return NotFound();
            return Ok(profile);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<Profile>> CreateProfile(ProfileDTO dto)
        {
            var profile = await _profileService.CreateProfile(dto);
            _logger.LogInformation("Created profile with UUID: {Uuid}", profile.Uuid);
            return Ok(profile);
        }

        [Authorize]
        [HttpPut("{uuid}")]
        public async Task<ActionResult<Profile>> UpdateProfile(string uuid, ProfileDTO dto)
        {
            var exists = await _profileService.ProfileExists(uuid);
            if (!exists)
                return NotFound();
            
            var profile = await _profileService.UpdateProfile(uuid, dto);
            return Ok(profile);
        }

        [Authorize]
        [HttpDelete("{uuid}")]
        public async Task<IActionResult> DeleteProfile(string uuid)
        {
            // Verify the user is deleting their own profile
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != uuid)
            {
                return Forbid();
            }

            var success = await _profileService.DeleteProfile(uuid);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<Profile>> Login(LoginDTO loginDto)
        {
            var profile = await _profileService.GetProfileByUsername(loginDto.Username);
            if (profile == null)
            {
                return Unauthorized();
            }

            if (!await _profileService.IsPasswordCorrect(profile.Uuid.ToString(), loginDto.Password))
            {
                return Unauthorized();
            }

            var token = _jwtService.GenerateToken(profile);
            return Ok(new { profile, token });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<Profile>> GetCurrentProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var profile = await _profileService.GetProfileByUuid(userId);
            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        // TODO: Add a method to add credits to a profile
        [Authorize]
        [HttpPost("add_credits")]
        public async Task<IActionResult> AddCredits(string uuid, int amount)
        {
            var profile = await _profileService.GetProfileByUuid(uuid);
            if (profile == null)
            {
                return NotFound();
            }
            
            profile.AddCredits(amount);
            await _profileService.UpdateProfile(uuid, profile);
            return Ok();
        }
    }
} 