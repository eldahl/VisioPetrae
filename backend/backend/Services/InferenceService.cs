using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using backend.Models;

namespace backend.Services
{
    public class InferenceService
    {
        private readonly ProfileService _profileService;
        private readonly ILogger<InferenceService> _logger;
        private const int CREDITS_PER_INFERENCE = 1;

        public InferenceService(ProfileService profileService, ILogger<InferenceService> logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        public async Task<bool> CanMakeInference(HttpContext context)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return false;
            }

            var profile = await _profileService.GetProfileByUuid(userId);
            if (profile == null)
            {
                return false;
            }

            return profile.HasEnoughCredits(CREDITS_PER_INFERENCE);
        }

        public async Task<bool> DeductCreditsForInference(HttpContext context)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return false;
            }

            var profile = await _profileService.GetProfileByUuid(userId);
            if (profile == null)
            {
                return false;
            }

            try
            {
                profile.DeductCredits(CREDITS_PER_INFERENCE);
                await _profileService.UpdateProfile(userId, new ProfileDTO
                {
                    Username = profile.Username,
                    Email = profile.Email,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Bio = profile.Bio,
                    AvatarUrl = profile.AvatarUrl
                });
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deduct credits for user {UserId}", userId);
                return false;
            }
        }
    }
} 