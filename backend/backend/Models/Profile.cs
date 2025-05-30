using System;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models
{
    [BsonIgnoreExtraElements]
    public class Profile
    {
        public Profile(ProfileDTO dto)
        {
            this.Uuid = Guid.NewGuid().ToString();
            this.Username = dto.Username;
            this.Password = CryptographyService.HashPassword(dto.Password);
            this.Email = dto.Email;
            this.FirstName = dto.FirstName;
            this.LastName = dto.LastName;
            this.Bio = dto.Bio;
            this.AvatarUrl = dto.AvatarUrl;
            this.Credits = 100;
            this.CreatedAt = DateTime.UtcNow;
            this.UpdatedAt = DateTime.UtcNow;
        }

        public string Uuid { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Credits { get; set; } = 100;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public void UpdatedFromDTO(ProfileDTO dto)
        {
            this.Username = dto.Username;
            this.Password = CryptographyService.HashPassword(dto.Password);
            this.Email = dto.Email;
            this.FirstName = dto.FirstName;
            this.LastName = dto.LastName;
            this.Bio = dto.Bio;
            this.AvatarUrl = dto.AvatarUrl;
            this.UpdatedAt = DateTime.UtcNow;
        }

        public bool HasEnoughCredits(int requiredCredits)
        {
            return Credits >= requiredCredits;
        }

        public void DeductCredits(int amount)
        {
            if (!HasEnoughCredits(amount))
            {
                throw new InvalidOperationException("Not enough credits");
            }
            Credits -= amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddCredits(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount must be positive", nameof(amount));
            }
            Credits += amount;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 