using backend.Models;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace backend.Services
{
    public class ProfileService
    {
        private readonly IMongoDBContext _db;
        private readonly ConcurrentDictionary<string, Profile> _profiles = new();

        public ProfileService(IMongoDBContext db)
        {
            _db = db;
        }

        public async virtual Task<IEnumerable<Profile>> GetAllProfiles()
        {
            // Get all profiles from the database
            return await _db.Collection<Profile>().Find(_ => true).ToListAsync();
        }

        public async virtual Task<bool> ProfileExists(string uuid)
        {
            // Check if the profile exists in the database
            return await _db.Collection<Profile>().CountDocumentsAsync(p => p.Uuid == uuid) > 0;
        }

        public async virtual Task<Profile?> GetProfileByUuid(string uuid)
        {
            // Get the profile from the database
            return await _db.Collection<Profile>().Find(p => p.Uuid == uuid).FirstOrDefaultAsync();
        }

        public async virtual Task<Profile?> GetProfileByUsername(string username)
        {
            // Get the profile from the database
            return await _db.Collection<Profile>().Find(p => p.Username == username).FirstOrDefaultAsync();
        }

        public async virtual Task<Profile> CreateProfile(ProfileDTO dto)
        {
            // Convert the profileDTO to a Profile
            var profile = new Profile(dto);
            profile.Uuid = Guid.NewGuid().ToString();
            profile.CreatedAt = DateTime.UtcNow;
            profile.UpdatedAt = DateTime.UtcNow;
            
            // Insert the profile into the database
            await _db.Collection<Profile>().InsertOneAsync(profile);
            return profile;
        }

        public async virtual Task<bool> UpdateProfile(string uuid, ProfileDTO dto)
        {
            // Get the profile from the database
            var profile = await GetProfileByUuid(uuid);
            
            if (profile is null)
                return false;

            // Update the profile from the DTO
            profile.UpdatedFromDTO(dto);
                
            // Replace the profile in the database
            var result = await _db.Collection<Profile>().ReplaceOneAsync(p => p.Uuid == uuid, profile);
            return result.ModifiedCount > 0;
        }

        public async virtual Task<bool> DeleteProfile(string uuid)
        {
            // Delete the profile from the database
            var result = await _db.Collection<Profile>().DeleteOneAsync(p => p.Uuid == uuid);
            return result is null ? false : result.DeletedCount > 0;
        }
    }
} 