using inference_registry.Models;
using System.Collections.Concurrent;

namespace inference_registry.Services;

public class InferenceServerRegistry
{
    private readonly ConcurrentDictionary<int, InferenceServer> _servers = new();
    
    public IEnumerable<KeyValuePair<int, InferenceServer>> GetAllServers()
    {
        return _servers.Select(s => new KeyValuePair<int, InferenceServer>(s.Key, s.Value));
    }
    
    public InferenceServer? GetServer(int id)
    {
        _servers.TryGetValue(id, out var server);
        return server;
    }
    
    public bool AddServer(InferenceServer server)
    {
        return _servers.TryAdd(server.Id, server);
    }
    
    public bool UpdateServer(InferenceServer server)
    {
        return _servers.TryUpdate(server.Id, server, _servers[server.Id]);
    }
    
    public bool RemoveServer(int id)
    {
        return _servers.TryRemove(id, out _);
    }
    
    public int GetAvailableServerId()
    {
        var availableServer = _servers.Values
            .Where(s => s.IsAvailable && s.ActiveTasks < s.MaxTasks)
            .OrderBy(s => s.ActiveTasks)
            .FirstOrDefault();
            
        return availableServer?.Id ?? -1;
    }
    
    public void UpdateServerStatus(int id, string status)
    {
        if (_servers.TryGetValue(id, out var server))
        {
            server.Status = status;
            server.LastHeartbeat = DateTime.UtcNow;
            _servers[id] = server;
        }
    }
    
    public void IncrementActiveTasks(int id)
    {
        if (_servers.TryGetValue(id, out var server))
        {
            server.ActiveTasks++;
            _servers[id] = server;
        }
    }
    
    public void DecrementActiveTasks(int id)
    {
        if (_servers.TryGetValue(id, out var server) && server.ActiveTasks > 0)
        {
            server.ActiveTasks--;
            _servers[id] = server;
        }
    }
} 