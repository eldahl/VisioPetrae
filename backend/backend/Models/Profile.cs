using System;

namespace backend.Models
{
    public class Profile
    {
        public Profile(ProfileDTO dto)
        {
            this.Uuid = Guid.NewGuid().ToString();
            this.Username = dto.Username;
            this.Email = dto.Email;
            this.FirstName = dto.FirstName;
            this.LastName = dto.LastName;
            this.Bio = dto.Bio;
            this.AvatarUrl = dto.AvatarUrl;
            this.CreatedAt = DateTime.UtcNow;
            this.UpdatedAt = DateTime.UtcNow;
        }

        public string Uuid { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public void UpdatedFromDTO(ProfileDTO dto)
        {
            this.Username = dto.Username;
            this.Email = dto.Email;
            this.FirstName = dto.FirstName;
            this.LastName = dto.LastName;
            this.Bio = dto.Bio;
            this.AvatarUrl = dto.AvatarUrl;
            this.UpdatedAt = DateTime.UtcNow;
        }
    }
} 