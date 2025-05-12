using inference_registry.Models;
using inference_registry.Services;
using Microsoft.AspNetCore.Mvc;

namespace inference_registry.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServersController : ControllerBase
{
    private readonly InferenceServerRegistry _registry;
    private readonly ILogger<ServersController> _logger;

    public ServersController(InferenceServerRegistry registry, ILogger<ServersController> logger)
    {
        _registry = registry;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<InferenceServer>> GetAllServers()
    {
        return Ok(_registry.GetAllServers());
    }

    [HttpGet("{id}")]
    public ActionResult<InferenceServer> GetServer(int id)
    {
        var server = _registry.GetServer(id);
        if (server == null)
        {
            return NotFound();
        }
        return Ok(server);
    }

    [HttpPost]
    public ActionResult<InferenceServer> RegisterServer(InferenceServer server)
    {
        if (_registry.AddServer(server))
        {
            _logger.LogInformation("Registered new server with ID: {ServerId}", server.Id);
            return CreatedAtAction(nameof(GetServer), new { id = server.Id }, server);
        }
        return Conflict("A server with the same ID already exists");
    }
} 