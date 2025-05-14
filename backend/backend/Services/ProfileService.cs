using backend.Models;
using System.Collections.Concurrent;

namespace backend.Services
{
    public class ProfileService
    {
        private readonly ConcurrentDictionary<int, Profile> _profiles = new();
        private int _nextId = 1;

        public IEnumerable<Profile> GetAllProfiles()
        {
            return _profiles.Values;
        }

        public Profile? GetProfile(int id)
        {
            _profiles.TryGetValue(id, out var profile);
            return profile;
        }

        public Profile? GetProfileByUsername(string username)
        {
            return _profiles.Values.FirstOrDefault(p => p.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public Profile CreateProfile(Profile profile)
        {
            profile.Id = Interlocked.Increment(ref _nextId) - 1;
            profile.CreatedAt = DateTime.UtcNow;
            profile.UpdatedAt = DateTime.UtcNow;
            
            _profiles[profile.Id] = profile;
            return profile;
        }

        public bool UpdateProfile(Profile profile)
        {
            if (!_profiles.ContainsKey(profile.Id))
            {
                return false;
            }

            profile.UpdatedAt = DateTime.UtcNow;
            return _profiles.TryUpdate(profile.Id, profile, _profiles[profile.Id]);
        }

        public bool DeleteProfile(int id)
        {
            return _profiles.TryRemove(id, out _);
        }
    }
} 