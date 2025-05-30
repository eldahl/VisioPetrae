import { createSignal, onMount, Show } from 'solid-js';
import styles from '../App.module.css';

function Profile() {
  const [profile, setProfile] = createSignal(null);
  const [loading, setLoading] = createSignal(true);
  const [error, setError] = createSignal('');
  const [editMode, setEditMode] = createSignal(false);
  const [formData, setFormData] = createSignal({
    username: '',
    email: '',
    firstName: '',
    lastName: '',
    bio: '',
    avatarUrl: ''
  });

  onMount(async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        window.location.href = '/vp/login';
        return;
      }

      const response = await fetch('https://vps.eldc.dk/api/Profile/me', {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        if (response.status === 401) {
          localStorage.removeItem('token');
          window.location.href = '/vp/login';
          return;
        }
        throw new Error('Failed to fetch profile');
      }

      const data = await response.json();
      setProfile(data);
      setFormData({
        username: data.username,
        email: data.email,
        firstName: data.firstName,
        lastName: data.lastName,
        bio: data.bio,
        avatarUrl: data.avatarUrl
      });
      setLoading(false);
    } catch (err) {
      setError('Failed to load profile');
    } finally {
      setLoading(false);
    }
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    try {
      const token = localStorage.getItem('token');
      const response = await fetch('https://vps.eldc.dk/api/Profile', {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData()),
      });

      if (!response.ok) {
        if (response.status === 401) {
          localStorage.removeItem('token');
          window.location.href = '/vp/login';
          return;
        }
        throw new Error('Failed to update profile');
      }

      const data = await response.json();
      setProfile(data);
      setEditMode(false);
    } catch (err) {
      setError('Failed to update profile');
    }
  };

  return (
    <>
    <Show when={loading()}>
        <div class={styles.pageContainer}>Loading...</div>
    </Show>
    <Show when={!loading()}>
        <div class={styles.pageContainer}>
        <h1>Profile</h1>
        
        {error() && <div class={styles.error}>{error()}</div>}

        <div class={styles.profileContainer}>
            <div class={styles.profileHeader}>
            <div class={styles.profileAvatar}>
                {profile()?.avatarUrl ? (
                <img src={profile().avatarUrl} alt="Profile" />
                ) : (
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
                    <circle cx="12" cy="8" r="4" />
                    <path d="M4 20c0-2.21 3.58-4 8-4s8 1.79 8 4" />
                </svg>
                )}
            </div>
            <h2>{profile()?.firstName} {profile()?.lastName}</h2>
            <button 
                class={styles.button} 
                onClick={() => setEditMode(!editMode())}
            >
                {editMode() ? 'Cancel' : 'Edit Profile'}
            </button>
            </div>

            {editMode() ? (
            <form onSubmit={handleSubmit} class={styles.form}>
                <div class={styles.formGroup}>
                <label for="username">Username</label>
                <input
                    type="text"
                    id="username"
                    value={formData().username}
                    onInput={(e) => setFormData({ ...formData(), username: e.target.value })}
                    required
                />
                </div>

                <div class={styles.formGroup}>
                <label for="email">Email</label>
                <input
                    type="email"
                    id="email"
                    value={formData().email}
                    onInput={(e) => setFormData({ ...formData(), email: e.target.value })}
                    required
                />
                </div>

                <div class={styles.formGroup}>
                <label for="firstName">First Name</label>
                <input
                    type="text"
                    id="firstName"
                    value={formData().firstName}
                    onInput={(e) => setFormData({ ...formData(), firstName: e.target.value })}
                    required
                />
                </div>

                <div class={styles.formGroup}>
                <label for="lastName">Last Name</label>
                <input
                    type="text"
                    id="lastName"
                    value={formData().lastName}
                    onInput={(e) => setFormData({ ...formData(), lastName: e.target.value })}
                    required
                />
                </div>

                <div class={styles.formGroup}>
                <label for="bio">Bio</label>
                <textarea
                    id="bio"
                    value={formData().bio}
                    onInput={(e) => setFormData({ ...formData(), bio: e.target.value })}
                    rows="3"
                />
                </div>

                <div class={styles.formGroup}>
                <label for="avatarUrl">Avatar URL</label>
                <input
                    type="url"
                    id="avatarUrl"
                    value={formData().avatarUrl}
                    onInput={(e) => setFormData({ ...formData(), avatarUrl: e.target.value })}
                    placeholder="https://example.com/avatar.jpg"
                />
                </div>

                <button type="submit" class={styles.button}>Save Changes</button>
            </form>
            ) : (
            <div class={styles.profileInfo}>
                <div class={styles.infoGroup}>
                <h3>Username</h3>
                <p>{profile()?.username}</p>
                </div>
                <div class={styles.infoGroup}>
                <h3>Email</h3>
                <p>{profile()?.email}</p>
                </div>
                <div class={styles.infoGroup}>
                <h3>Bio</h3>
                <p>{profile()?.bio || 'No bio provided'}</p>
                </div>
            </div>
            )}
        </div>
        </div>
    </Show>
    </>
  );
}

export default Profile; 