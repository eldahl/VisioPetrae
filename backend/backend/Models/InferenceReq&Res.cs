namespace backend.Models
{
    public class InferenceResponse {
        public string dialog_uuid { get; set; } = "";
        public string server_uuid { get; set; } = "";
        public string response { get; set; } = "";
    }

    public class InferenceRequest {
        public IFormFile image { get; set; } = null!;
        public string prompt { get; set; } = "";
    }
}