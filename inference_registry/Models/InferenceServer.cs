namespace inference_registry.Models;

public class InferenceServer
{
    public InferenceServer(string hostname, int port) {
        this.Uuid = Guid.NewGuid().ToString();
        this.Hostname = hostname;
        this.Port = port;
        this.ActiveTasks = 0;
        this.MaxTasks = 3;
        this.Status = "Offline";
        this.IsAvailable = false;
    }

    // Server information
    public string Uuid { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public int Port { get; set; }

    // Task information
    public int ActiveTasks { get; set; }
    public int MaxTasks { get; set; }

    // Status information
    public string Status { get; set; } = "Offline";
    public bool IsAvailable { get; set; }
    public DateTime LastHeartbeat { get; set; }
} 

public class InferenceResponse {
    public string uuid { get; set; }
    public string response { get; set; } = "";
}

public class InferenceRequest {
    public string uuid { get; set; }
    public string request { get; set; } = "";
}