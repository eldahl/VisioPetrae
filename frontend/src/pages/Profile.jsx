import { createSignal, onMount } from 'solid-js';
import styles from '../App.module.css';

function Profile() {
  const [profile, setProfile] = createSignal(null);
  const [loading, setLoading] = createSignal(true);
  const [error, setError] = createSignal('');
  const [editMode, setEditMode] = createSignal(false);
  const [formData, setFormData] = createSignal({
    name: '',
    email: '',
    organization: '',
  });

  onMount(async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        window.location.href = '/vp/login';
        return;
      }

      const response = await fetch('/api/profile', {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error('Failed to fetch profile');
      }

      const data = await response.json();
      setProfile(data);
      setFormData({
        name: data.name,
        email: data.email,
        organization: data.organization,
      });
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
      const response = await fetch('/api/profile', {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData()),
      });

      if (!response.ok) {
        throw new Error('Failed to update profile');
      }

      const data = await response.json();
      setProfile(data);
      setEditMode(false);
    } catch (err) {
      setError('Failed to update profile');
    }
  };

  if (loading()) {
    return <div class={styles.pageContainer}>Loading...</div>;
  }

  return (
    <div class={styles.pageContainer}>
      <h1>Profile</h1>
      
      {error() && <div class={styles.error}>{error()}</div>}

      <div class={styles.profileContainer}>
        <div class={styles.profileHeader}>
          <div class={styles.profileAvatar}>
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
              <circle cx="12" cy="8" r="4" />
              <path d="M4 20c0-2.21 3.58-4 8-4s8 1.79 8 4" />
            </svg>
          </div>
          <h2>{profile()?.name}</h2>
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
              <label for="name">Name</label>
              <input
                type="text"
                id="name"
                value={formData().name}
                onInput={(e) => setFormData({ ...formData(), name: e.target.value })}
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
              <label for="organization">Organization</label>
              <input
                type="text"
                id="organization"
                value={formData().organization}
                onInput={(e) => setFormData({ ...formData(), organization: e.target.value })}
              />
            </div>

            <button type="submit" class={styles.button}>Save Changes</button>
          </form>
        ) : (
          <div class={styles.profileInfo}>
            <div class={styles.infoGroup}>
              <h3>Email</h3>
              <p>{profile()?.email}</p>
            </div>
            <div class={styles.infoGroup}>
              <h3>Organization</h3>
              <p>{profile()?.organization || 'Not specified'}</p>
            </div>
            <div class={styles.infoGroup}>
              <h3>Credits</h3>
              <p>{profile()?.credits || 0} credits remaining</p>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default Profile; 