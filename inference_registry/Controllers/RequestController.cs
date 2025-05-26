
using System.Diagnostics;
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

    [HttpPost("inferRequest")]
    public async Task<ActionResult<InferenceResponse>> RequestInference(InferenceRequest request)
    {     
        // TODO: Add fault tolerance logic
        var server = _registry.GetAvailableServer();
        if(server == null) { return StatusCode(503, "No available servers"); } else {
            _logger.LogInformation("Requesting inference from server: {server}\nprompt: {request}\nimage name: {image}", server.Uuid, request.prompt, request.image.FileName);
            InferenceResponse res = await server.RequestInference(request);
            return Ok(res);
        };
    }
} 