namespace inference_registry.Models;

public class InferenceServer
{
    // Connection information
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

public class InferenceServerResponse {
    public int Id { get; set; }
    public string response;
}

public class InferenceServerRequest {
    public int Id { get; set; }
    public string request;
}