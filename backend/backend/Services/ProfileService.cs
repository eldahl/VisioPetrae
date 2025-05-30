using backend.Models;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using backend.Persistence;
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
            return await _db.Collection<Profile>().CountDocumentsAsync(p => p.Uuid == Guid.Parse(uuid)) > 0;
        }

        public async virtual Task<Profile?> GetProfileByUuid(string uuid)
        {
            // Get the profile from the database
            return await _db.Collection<Profile>().Find(p => p.Uuid == Guid.Parse(uuid)).FirstOrDefaultAsync();
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
            return await UpdateProfile(uuid, profile);
        }

        public async virtual Task<bool> UpdateProfile(string uuid, Profile profile)
        {
            // Update the profile in the database
            var result = await _db.Collection<Profile>().ReplaceOneAsync(p => p.Uuid == Guid.Parse(uuid), profile);
            return result.ModifiedCount > 0;
        }

        public async virtual Task<bool> DeleteProfile(string uuid)
        {
            // Check if the profile exists
            var exists = await ProfileExists(uuid);
            if (!exists)
                return false;

            // Delete the profile from the database
            var result = await _db.Collection<Profile>().DeleteOneAsync(p => p.Uuid == Guid.Parse(uuid));
            return result is null ? false : result.DeletedCount > 0;
        }

        public async virtual Task<bool> IsPasswordCorrect(string uuid, string password)
        {
            // Get the profile from the database
            var profile = await GetProfileByUuid(uuid);
            if (profile is null)
                return false;

            // Check if the password is correct
            return CryptographyService.VerifyPassword(password, profile.Password);
        }
    }
} 