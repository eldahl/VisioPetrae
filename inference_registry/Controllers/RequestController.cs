
using inference_registry.Models;
using inference_registry.Services;
using Microsoft.AspNetCore.Mvc;

namespace inference_registry.Controllers;

[ApiController]
[Route("[controller]")]
public class RequestController : ControllerBase
{
    private readonly InferenceServerRegistry _registry;
    private readonly ILogger<RequestController> _logger;

    public RequestController(InferenceServerRegistry registry, ILogger<RequestController> logger)
    {
        _registry = registry;
        _logger = logger;
    }

    [HttpPost]
    public ActionResult<InferenceRequest> RequestInference(InferenceRequest request)
    {
        var server = _registry.GetAvailableServer();
        server == null ? return StatusCode(503, "No available servers") : {
            InferenceResponse res = server.RequestInference(request);
            return Ok(res);
        };
    }
} 