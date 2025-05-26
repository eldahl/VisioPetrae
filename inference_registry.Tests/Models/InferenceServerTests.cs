using Xunit;
using System;
using System.Threading.Tasks;
using inference_registry.Models;

namespace inference_registry.Tests.Models;

public class InferenceServerTests
{
    [Fact]
    public async Task StartPeriodicHeartbeat_WhenHeartbeatSucceeds_UpdatesStatusToOnline()
    {
        // Arrange
        var server = new InferenceServer("test-uuid", "localhost", 8000);
        var statusUpdates = new List<string>();
        var shouldStop = false;

        var heartbeatTask = server.StartPeriodicHeartbeat(
            TimeSpan.FromMilliseconds(100),
            async () => {
                if (shouldStop) throw new TaskCanceledException();
                await Task.Delay(1);
                return true;
            },
            status => statusUpdates.Add(status)
        );

        // Act
        await Task.Delay(150); // Wait for first heartbeat

        // Assert
        Assert.Contains("Online", statusUpdates);
        Assert.Equal("Online", server.Status);
        Assert.True(server.IsAvailable);
        
        // Clean up
        shouldStop = true;
        await heartbeatTask; // Wait for the task to complete
    }

    [Fact]
    public async Task StartPeriodicHeartbeat_WhenHeartbeatFails_UpdatesStatusToOffline()
    {
        // Arrange
        var server = new InferenceServer("test-uuid", "localhost", 8000);
        var statusUpdates = new List<string>();
        var shouldStop = false;

        var heartbeatTask = server.StartPeriodicHeartbeat(
            TimeSpan.FromMilliseconds(100),
            async () => {
                if (shouldStop) throw new TaskCanceledException();
                await Task.Delay(1);
                return false;
            },
            status => statusUpdates.Add(status)
        );

        // Act
        await Task.Delay(150); // Wait for first heartbeat

        // Assert
        Assert.Contains("Offline", statusUpdates);
        Assert.Equal("Offline", server.Status);
        Assert.False(server.IsAvailable);
        
        // Clean up
        shouldStop = true;
        await heartbeatTask; // Wait for the task to complete
    }

    [Fact]
    public async Task StartPeriodicHeartbeat_WhenMaxTasksReached_SetsIsAvailableToFalse()
    {
        // Arrange
        var server = new InferenceServer("test-uuid", "localhost", 8000);
        server.ActiveTasks = server.MaxTasks;
        var statusUpdates = new List<string>();
        var shouldStop = false;

        var heartbeatTask = server.StartPeriodicHeartbeat(
            TimeSpan.FromMilliseconds(100),
            async () => {
                if (shouldStop) throw new TaskCanceledException();
                await Task.Delay(1);
                return true;
            },
            status => statusUpdates.Add(status)
        );

        // Act
        await Task.Delay(150); // Wait for first heartbeat

        // Assert
        Assert.Contains("Online", statusUpdates);
        Assert.Equal("Online", server.Status);
        Assert.False(server.IsAvailable);

        // Clean up
        shouldStop = true;
        await heartbeatTask; // Wait for the task to complete
    }

    [Fact]
    public async Task StartPeriodicHeartbeat_UpdatesLastHeartbeat()
    {
        // Arrange
        var server = new InferenceServer("test-uuid", "localhost", 8000);
        var initialHeartbeat = server.LastHeartbeat;
        var shouldStop = false;

        var heartbeatTask = server.StartPeriodicHeartbeat(
            TimeSpan.FromMilliseconds(100),
            async () => {
                if (shouldStop) throw new TaskCanceledException();
                await Task.Delay(1);
                return true;
            }
        );

        // Act
        await Task.Delay(150); // Wait for first heartbeat
        shouldStop = true;
        await heartbeatTask; // Wait for the task to complete

        // Assert
        Assert.True(server.LastHeartbeat > initialHeartbeat);
    }

    [Fact]
    public async Task StartPeriodicHeartbeat_WhenHeartbeatThrows_UpdatesStatusToOffline()
    {
        // Arrange
        var server = new InferenceServer("test-uuid", "localhost", 8000);
        var statusUpdates = new List<string>();
        var shouldStop = false;

        var heartbeatTask = server.StartPeriodicHeartbeat(
            TimeSpan.FromMilliseconds(100),
            async () => {
                if (shouldStop) throw new TaskCanceledException();
                await Task.Delay(1);
                throw new Exception("Test exception");
            },
            status => statusUpdates.Add(status)
        );

        // Act
        await Task.Delay(150); // Wait for first heartbeat
        shouldStop = true;
        await heartbeatTask; // Wait for the task to complete

        // Assert
        Assert.Contains("Offline", statusUpdates);
        Assert.Equal("Offline", server.Status);
        Assert.False(server.IsAvailable);
    }
}