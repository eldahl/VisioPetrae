using Microsoft.AspNetCore.Mvc;

namespace VPBackend_Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InferenceController : ControllerBase
    {
        [HttpPost("request_inference_job")]
        public IActionResult RequestInferenceJob(string job)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8000");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.PostAsJsonAsync("request_inference_job", job).Result;
            if (response.IsSuccessStatusCode)
            {
                return Ok("Inference job requested.");
            return Ok("Inference job requested.");
        }
    }
} 