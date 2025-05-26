using inference_registry.Models;
using inference_registry.Services;
using Microsoft.AspNetCore.Mvc;

namespace inference_registry.Controllers;

[ApiController]
[Route("[controller]")]
public class RegistryController : ControllerBase
{
    private readonly InferenceServerRegistry _registry;
    private readonly ILogger<RegistryController> _logger;

    public RegistryController(InferenceServerRegistry registry, ILogger<RegistryController> logger)
    {
        _registry = registry;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<InferenceServer>> GetAllServers()
    {
        return Ok(_registry.GetAllServers());
    }

    [HttpGet("{uuid}")]
    public ActionResult<InferenceServer> GetServer(string uuid)
    {
        var server = _registry.GetServer(uuid);
        if (server == null)
        {
            return NotFound();
        }
        return Ok(server);
    }

    [HttpPost]
    public ActionResult<InferenceServer> RegisterServer(InferenceServerDTO serverDTO)
    {
        var server = new InferenceServer(serverDTO);
        if (_registry.AddServer(server))
        {
            _logger.LogInformation("Registered new server with UUID: {Uuid}", server.Uuid);
            return CreatedAtAction(nameof(GetServer), new { uuid = server.Uuid }, server);
        }
        return Conflict("A server with the same UUID already exists");
    }

    [HttpPut("{uuid}")]
    public ActionResult<InferenceServer> UpdateServer(string uuid, InferenceServerDTO serverDTO)
    {
        var server = new InferenceServer(uuid, serverDTO);
        if (_registry.UpdateServer(server))
        {
            _logger.LogInformation("Updated server with UUID: {Uuid}", server.Uuid);
            return Ok(server);
        }
        return NotFound();
    }

    [HttpDelete("{uuid}")]
    public ActionResult<InferenceServer> DeregisterServer(string uuid)
    {
        if (_registry.RemoveServer(uuid))
        {
            _logger.LogInformation("Deregistered server with UUID: {Uuid}", uuid);
            return Ok();
        }
        return NotFound();
    }
} 