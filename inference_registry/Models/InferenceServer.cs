namespace inference_registry.Models;
using System.Net.Http.Headers;
using System.Threading;

public class InferenceServerDTO {
    public string Hostname { get; set; } = string.Empty;
    public int Port { get; set; }
    public int MaxTasks { get; set; }

    public bool IsAvailable { get; set; }
    public string Status { get; set; } = "Offline";
}

public class InferenceServer
{
    public InferenceServer(InferenceServerDTO dto) : this(Guid.NewGuid().ToString(), dto.Hostname, dto.Port, dto.MaxTasks, dto.IsAvailable, dto.Status) {}
    public InferenceServer(string uuid, InferenceServerDTO dto) : this(uuid, dto.Hostname, dto.Port, dto.MaxTasks, dto.IsAvailable, dto.Status) {}
    public InferenceServer(string uuid, string hostname = "localhost", int port = 8000, int maxTasks = 3, bool isAvailable = false, string status = "Offline") {
        this.Uuid = uuid;
        this.Hostname = hostname;
        this.Port = port;
        this.ActiveTasks = 0;
        this.MaxTasks = maxTasks;
        this.Status = status;
        this.IsAvailable = isAvailable;
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

    public virtual async Task<InferenceResponse> RequestInference(InferenceRequest request) {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri($"http://{this.Hostname}:{this.Port}");
        
        var formData = new MultipartFormDataContent();
        // image
        var imageContent = new StreamContent(request.image.OpenReadStream());
        imageContent.Headers.ContentType = new MediaTypeHeaderValue(request.image.ContentType);
        formData.Add(imageContent, "image", request.image.FileName);
        // prompt
        formData.Add(new StringContent(request.prompt), "prompt");

        var response = await client.PostAsync("/infer", formData);
        
        response.EnsureSuccessStatusCode();

        var resp = new InferenceResponse {
            server_uuid = this.Uuid,
            dialog_uuid = Guid.NewGuid().ToString(),
            response = await response.Content.ReadAsStringAsync()
        };
        return resp;
    }

    public virtual async Task StartPeriodicHeartbeat(TimeSpan interval, Func<Task<bool>> heartbeat, Action<string>? onStatusUpdate = null)
    {
        while (true)
        {
            try
            {
                var isAlive = await heartbeat();
                if (isAlive)
                {
                    this.Status = "Online";
                    this.IsAvailable = this.ActiveTasks < this.MaxTasks;
                }
                else
                {
                    this.Status = "Offline";
                    this.IsAvailable = false;
                }
                this.LastHeartbeat = DateTime.UtcNow;
                onStatusUpdate?.Invoke(this.Status);
            }
            catch (Exception)
            {
                this.Status = "Offline";
                this.IsAvailable = false;
                this.LastHeartbeat = DateTime.UtcNow;
                onStatusUpdate?.Invoke(this.Status);
                break;
            }

            await Task.Delay(interval);
        }
    }
} 

public class InferenceResponse {
    public string dialog_uuid { get; set; } = "";
    public string server_uuid { get; set; } = "";
    public string response { get; set; } = "";
}

public class InferenceRequest {
    public IFormFile image { get; set; } = null!;
    public string prompt { get; set; } = "";
}