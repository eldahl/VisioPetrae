using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using backend.Models;
using System.Net;
using System.Security.Authentication;
using backend.Services;
using System.Security.Claims;

namespace VPBackend_Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class InferenceController : ControllerBase
    {
        private readonly ILogger<InferenceController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ProfileService _profileService;
        public InferenceController(ILogger<InferenceController> logger, IConfiguration configuration, ProfileService profileService)
        {
            _logger = logger;
            _configuration = configuration;
            _profileService = profileService;
        }

        [HttpPost("request_inference_job")]
        [Authorize]
        public async Task<IActionResult> RequestInferenceJob(InferenceRequest job)
        {
            // get user id
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            // get profile
            var profile = await _profileService.GetProfileByUuid(userId);
            if (profile == null)
                return NotFound();

            // check if user has enough credits
            var creditsPerInference = _configuration.GetValue<int>("Billing:CreditsPerInference");
            if(profile.Credits <= creditsPerInference)
                return StatusCode(400, "Insufficient credits.");

            // create http client
            HttpClient client = new HttpClient(new HttpClientHandler() {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
            });
            client.BaseAddress = new Uri(_configuration["REGISTRY_URL"] ?? throw new Exception("REGISTRY_URL is not set"));
            
            // form data
            var formData = new MultipartFormDataContent();
            // image
            var imageContent = new StreamContent(job.image.OpenReadStream());
            imageContent.Headers.ContentType = new MediaTypeHeaderValue(job.image.ContentType);
            formData.Add(imageContent, "image", job.image.FileName);
            // prompt
            formData.Add(new StringContent(job.prompt), "prompt");
            
            // request inference
            var response = await client.PostAsync("Request/inferRequest", formData);
            if (response.IsSuccessStatusCode)
            {
                // deserialize response
                var res = JsonSerializer.Deserialize<InferenceResponse>(await response.Content.ReadAsStringAsync());
                if(res is null)
                    return StatusCode(500, "Failed to deserialize inference response.");

                // update profile credits
                profile.DeductCredits(creditsPerInference);
                await _profileService.UpdateProfile(profile.Uuid, profile);
                
                _logger.LogInformation("Inference job completed. dialog_uuid: {dialog_uuid}, server_uuid: {server_uuid}, response: {response}", res.dialog_uuid, res.server_uuid, res.response);
                return Ok(res.response);
            }
            _logger.LogError("Failed to request inference job. status code: {statusCode}, response: {response}", response.StatusCode, await response.Content.ReadAsStringAsync());
            return StatusCode(500, "Failed to request inference job.");
        }
    }
} 