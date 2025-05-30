using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using backend.Models;
namespace VPBackend_Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class InferenceController : ControllerBase
    {
        private readonly ILogger<InferenceController> _logger;
        private readonly IConfiguration _configuration;

        public InferenceController(ILogger<InferenceController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("request_inference_job")]
        [Authorize]
        public async Task<IActionResult> RequestInferenceJob(InferenceRequest job)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_configuration.GetSection("REGISTRY_URL").Get<string>() ?? throw new Exception("REGISTRY_URL is not set"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.PostAsJsonAsync("Request/inferRequest", job);
            if (response.IsSuccessStatusCode)
            {
                var res = JsonSerializer.Deserialize<InferenceResponse>(await response.Content.ReadAsStringAsync());
                if(res is null)
                    return StatusCode(500, "Failed to deserialize inference response.");
                
                _logger.LogInformation("Inference job completed. dialog_uuid: {dialog_uuid}, server_uuid: {server_uuid}, response: {response}", res.dialog_uuid, res.server_uuid, res.response);
                return Ok(res.response);
            }
            return StatusCode(500, "Failed to request inference job.");
        }
    }
} 