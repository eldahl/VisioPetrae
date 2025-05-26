using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;
using inference_registry.Services;
using inference_registry.Models;
using System.Threading;
using System.Threading.Tasks;

namespace inference_registry.Tests.Services;

public class InferenceServerRegistryTests
{
    private readonly InferenceServerRegistry _registry;

    public InferenceServerRegistryTests()
    {
        _registry = new InferenceServerRegistry();
    }

    [Fact]
    public void GetAllServers_WhenEmpty_ReturnsEmptyCollection()
    {
        // Act
        var servers = _registry.GetAllServers();

        // Assert
        Assert.Empty(servers);
    }

    [Fact]
    public void GetAllServers_WhenServersExist_ReturnsAllServers()
    {
        // Arrange
        var dto0 = new InferenceServerDTO { Hostname = "host1", Port = 8000, Status = "Offline" };
        var dto1 = new InferenceServerDTO { Hostname = "host2", Port = 8001, Status = "Offline" };
        var server0 = new InferenceServer(dto0);
        var server1 = new InferenceServer(dto1);
        _registry.AddServer(server0);
        _registry.AddServer(server1);

        // Act
        var servers = _registry.GetAllServers().ToList();

        // Assert
        Assert.Equal(2, servers.Count);
        Assert.Contains(servers, s => s.Value.Uuid == server0.Uuid);
        Assert.Contains(servers, s => s.Value.Uuid == server1.Uuid);
    }

    [Fact]
    public void GetServer_WhenServerExists_ReturnsServer()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Offline" };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);

        // Act
        var result = _registry.GetServer(server.Uuid);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(server.Uuid, result!.Uuid);
    }

    [Fact]
    public void GetServer_WhenServerDoesNotExist_ReturnsNull()
    {
        // Act
        var result = _registry.GetServer("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void AddServer_WhenServerDoesNotExist_ReturnsTrue()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Offline" };
        var server = new InferenceServer(dto);

        // Act
        var result = _registry.AddServer(server);

        // Assert
        Assert.True(result);
        Assert.NotNull(_registry.GetServer(server.Uuid));
    }

    [Fact]
    public void AddServer_WhenServerExists_ReturnsFalse()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Offline" };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);

        // Act
        var result = _registry.AddServer(server);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UpdateServer_WhenServerExists_ReturnsTrue()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Offline" };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);
        server.Status = "Updated";

        // Act
        var result = _registry.UpdateServer(server);

        // Assert
        Assert.True(result);
        var updatedServer = _registry.GetServer(server.Uuid);
        Assert.NotNull(updatedServer);
        Assert.Equal("Updated", updatedServer!.Status);
    }

    [Fact]
    public void UpdateServer_WhenServerDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Offline" };
        var server = new InferenceServer(dto);

        // Act
        var result = _registry.UpdateServer(server);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RemoveServer_WhenServerExists_ReturnsTrue()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Offline" };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);

        // Act
        var result = _registry.RemoveServer(server.Uuid);

        // Assert
        Assert.True(result);
        Assert.Null(_registry.GetServer(server.Uuid));
    }

    [Fact]
    public void RemoveServer_WhenServerDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = _registry.RemoveServer("nonexistent");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetAvailableServer_WhenNoServersAvailable_ReturnsNull()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Offline", IsAvailable = false };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);

        // Act
        var result = _registry.GetAvailableServer();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAvailableServer_WhenServerAvailable_ReturnsServer()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Online", IsAvailable = true };
        var server = new InferenceServer(dto);
        server.ActiveTasks = 0;
        server.MaxTasks = 2;
        _registry.AddServer(server);

        // Act
        var result = _registry.GetAvailableServer();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(server.Uuid, result!.Uuid);
    }

    [Fact]
    public void GetAvailableServer_WhenMultipleServersAvailable_ReturnsLeastBusy()
    {
        // Arrange
        var dto0 = new InferenceServerDTO { Hostname = "host1", Port = 8000, Status = "Online", IsAvailable = true };
        var dto1 = new InferenceServerDTO { Hostname = "host2", Port = 8001, Status = "Online", IsAvailable = true };
        var server0 = new InferenceServer(dto0);
        var server1 = new InferenceServer(dto1);
        server0.ActiveTasks = 2;
        server0.MaxTasks = 3;
        server1.ActiveTasks = 1;
        server1.MaxTasks = 3;
        _registry.AddServer(server0);
        _registry.AddServer(server1);

        // Act
        var result = _registry.GetAvailableServer();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(server1.Uuid, result!.Uuid);
    }

    [Fact]
    public void UpdateServerStatus_WhenServerExists_UpdatesStatusAndHeartbeat()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Offline" };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);
        var initialHeartbeat = server.LastHeartbeat;

        // Act
        _registry.UpdateServerStatus(server.Uuid, "Online");

        // Assert
        var updatedServer = _registry.GetServer(server.Uuid);
        Assert.NotNull(updatedServer);
        Assert.Equal("Online", updatedServer!.Status);
        Assert.True(updatedServer.LastHeartbeat > initialHeartbeat);
    }

    [Fact]
    public void UpdateServerStatus_WhenServerDoesNotExist_DoesNothing()
    {
        // Act & Assert
        _registry.UpdateServerStatus("nonexistent", "Online");
        // No exception should be thrown
    }

    [Fact]
    public void UpdateServerStatus_UpdatesLastHeartbeatToCurrentTime()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Offline" };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);
        var initialHeartbeat = server.LastHeartbeat;

        // Act
        _registry.UpdateServerStatus(server.Uuid, "Online");

        // Assert
        var updatedServer = _registry.GetServer(server.Uuid);
        Assert.NotNull(updatedServer);
        Assert.True(updatedServer!.LastHeartbeat > initialHeartbeat);
    }

    [Fact]
    public void UpdateServerStatus_MultipleUpdates_UpdatesHeartbeatEachTime()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Offline" };
        var server = new InferenceServer(dto);
        _registry.AddServer(server);
        var initialHeartbeat = server.LastHeartbeat;

        // Act
        _registry.UpdateServerStatus(server.Uuid, "Online");
        var firstUpdateHeartbeat = _registry.GetServer(server.Uuid)!.LastHeartbeat;
        Thread.Sleep(100); // Ensure some time passes
        _registry.UpdateServerStatus(server.Uuid, "Busy");
        var secondUpdateHeartbeat = _registry.GetServer(server.Uuid)!.LastHeartbeat;

        // Assert
        Assert.True(firstUpdateHeartbeat > initialHeartbeat);
        Assert.True(secondUpdateHeartbeat > firstUpdateHeartbeat);
    }

    [Fact]
    public void IncrementActiveTasks_WhenServerExists_IncrementsTasks()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Online" };
        var server = new InferenceServer(dto);
        server.ActiveTasks = 0;
        server.MaxTasks = 2;
        _registry.AddServer(server);

        // Act
        _registry.IncrementActiveTasks(server.Uuid);

        // Assert
        var updatedServer = _registry.GetServer(server.Uuid);
        Assert.NotNull(updatedServer);
        Assert.Equal(1, updatedServer!.ActiveTasks);
    }

    [Fact]
    public void DecrementActiveTasks_WhenServerExistsAndTasksGreaterThanZero_DecrementsTasks()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Online" };
        var server = new InferenceServer(dto);
        server.ActiveTasks = 1;
        server.MaxTasks = 2;
        _registry.AddServer(server);

        // Act
        _registry.DecrementActiveTasks(server.Uuid);

        // Assert
        var updatedServer = _registry.GetServer(server.Uuid);
        Assert.NotNull(updatedServer);
        Assert.Equal(0, updatedServer!.ActiveTasks);
    }

    [Fact]
    public void DecrementActiveTasks_WhenServerExistsAndTasksZero_DoesNotDecrement()
    {
        // Arrange
        var dto = new InferenceServerDTO { Hostname = "host0", Port = 8000, Status = "Online" };
        var server = new InferenceServer(dto);
        server.ActiveTasks = 0;
        server.MaxTasks = 2;
        _registry.AddServer(server);

        // Act
        _registry.DecrementActiveTasks(server.Uuid);

        // Assert
        var updatedServer = _registry.GetServer(server.Uuid);
        Assert.NotNull(updatedServer);
        Assert.Equal(0, updatedServer!.ActiveTasks);
    }
} 