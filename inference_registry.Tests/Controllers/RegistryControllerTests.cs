using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using inference_registry.Controllers;
using inference_registry.Models;
using inference_registry.Services;

namespace inference_registry.Tests.Controllers;

public class RegistryControllerTests
{
    private readonly Mock<ILogger<RegistryController>> _loggerMock;
    private readonly InferenceServerRegistry _registry;
    private readonly RegistryController _controller;

    public RegistryControllerTests()
    {
        _loggerMock = new Mock<ILogger<RegistryController>>();
        _registry = new InferenceServerRegistry();
        _controller = new RegistryController(_registry, _loggerMock.Object);
    }

    [Fact]
    public void GetAllServers_WhenEmpty_ReturnsEmptyList()
    {
        // Act
        var result = _controller.GetAllServers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var servers = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<string, InferenceServer>>>(okResult.Value);
        Assert.Empty(servers);
    }

    [Fact]
    public void GetAllServers_WhenServersExist_ReturnsAllServers()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host1", Port = 8000 };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);

        // Act
        var result = _controller.GetAllServers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var servers = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<string, InferenceServer>>>(okResult.Value);
        Assert.Single(servers);
        Assert.Contains(servers, s => s.Value.Uuid == server.Uuid);
    }

    [Fact]
    public void GetServer_WhenServerExists_ReturnsServer()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host1", Port = 8000 };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);

        // Act
        var result = _controller.GetServer(server.Uuid);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedServer = Assert.IsType<InferenceServer>(okResult.Value);
        Assert.Equal(server.Uuid, returnedServer.Uuid);
    }

    [Fact]
    public void GetServer_WhenServerDoesNotExist_ReturnsNotFound()
    {
        // Act
        var result = _controller.GetServer("nonexistent");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void RegisterServer_WithValidData_ReturnsCreated()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host1", Port = 8000 };

        // Act
        var result = _controller.RegisterServer(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var server = Assert.IsType<InferenceServer>(createdResult.Value);
        Assert.Equal(dto.Hostname, server.Hostname);
        Assert.Equal(dto.Port, server.Port);
    }

    [Fact]
    public void RegisterServer_WithDuplicateServer_ReturnsConflict()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host1", Port = 8000 };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);

        // Act
        var result = _controller.RegisterServer(dto);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal("A server with the same UUID already exists", conflictResult.Value);
    }

    [Fact]
    public void UpdateServer_WhenServerExists_ReturnsOk()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host1", Port = 8000 };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);
        var updateDto = new InferenceServerDTO { Hostname = "host2", Port = 8001 };

        // Act
        var result = _controller.UpdateServer(server.Uuid, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var updatedServer = Assert.IsType<InferenceServer>(okResult.Value);
        Assert.Equal(updateDto.Hostname, updatedServer.Hostname);
        Assert.Equal(updateDto.Port, updatedServer.Port);
    }

    [Fact]
    public void UpdateServer_WhenServerDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host1", Port = 8000 };

        // Act
        var result = _controller.UpdateServer("nonexistent", dto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void DeregisterServer_WhenServerExists_ReturnsOk()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host1", Port = 8000 };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);

        // Act
        var result = _controller.DeregisterServer(server.Uuid);

        // Assert
        Assert.IsType<OkResult>(result.Result);
        Assert.Null(_registry.GetServer(server.Uuid));
    }

    [Fact]
    public void DeregisterServer_WhenServerDoesNotExist_ReturnsNotFound()
    {
        // Act
        var result = _controller.DeregisterServer("nonexistent");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
} 