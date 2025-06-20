using inference_registry.Models;
using System.Collections.Concurrent;

namespace inference_registry.Services;

public class InferenceServerRegistry
{
    private readonly ConcurrentDictionary<string, InferenceServer> _servers = new();
    
    public IEnumerable<KeyValuePair<string, InferenceServer>> GetAllServers()
    {
        return _servers.Select(s => new KeyValuePair<string, InferenceServer>(s.Key, s.Value));
    }
    
    public InferenceServer? GetServer(string uuid)
    {
        _servers.TryGetValue(uuid, out var server);
        return server;
    }
    
    public bool AddServer(InferenceServer server)
    {
        // Check if the server is already registered
        if(_servers.Values.Any(s => s.Hostname == server.Hostname))
            return false;

        // Add the server to the registry
        if(_servers.TryAdd(server.Uuid, server)){
            // Start the server's heartbeat
            Task.Run(async () => await server.StartPeriodicHeartbeat(TimeSpan.FromSeconds(2), async () => {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"http://{server.Hostname}:{server.Port}");

                var response = await client.GetAsync("/heartbeat");
                return response.IsSuccessStatusCode;
            }));
            return true;
        }

        return false;
    }
    
    public bool UpdateServer(InferenceServer server)
    {
        if (_servers.TryGetValue(server.Uuid, out var existingServer))
        {
            return _servers.TryUpdate(server.Uuid, server, existingServer);
        }
        return false;
    }
    
    public bool RemoveServer(string uuid)
    {
        return _servers.TryRemove(uuid, out _);
    }
    
    public InferenceServer? GetAvailableServer()
    {
        var availableServer = _servers.Values
            .Where(s => s.IsAvailable && s.ActiveTasks < s.MaxTasks)
            .OrderBy(s => s.ActiveTasks)
            .FirstOrDefault();
            
        return availableServer;
    }
    
    public void UpdateServerStatus(string uuid, string status)
    {
        if (_servers.TryGetValue(uuid, out var server))
        {
            server.Status = status;
            server.LastHeartbeat = DateTime.UtcNow;
            _servers[uuid] = server;
        }
    }
    
    public void IncrementActiveTasks(string uuid)
    {
        if (_servers.TryGetValue(uuid, out var server))
        {
            server.ActiveTasks++;
            _servers[uuid] = server;
        }
    }
    
    public void DecrementActiveTasks(string uuid)
    {
        if (_servers.TryGetValue(uuid, out var server) && server.ActiveTasks > 0)
        {
            server.ActiveTasks--;
            _servers[uuid] = server;
        }
    }
} 