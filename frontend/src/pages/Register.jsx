import { createSignal } from 'solid-js';
import styles from '../App.module.css';

import defaultAvatar from '../assets/vp-rock-2.png';

function Register() {
  const [formData, setFormData] = createSignal({
    username: '',
    password: '',
    email: '',
    firstName: '',
    lastName: '',
    bio: '',
    avatarUrl: ''
  });
  const [error, setError] = createSignal('');
  const [success, setSuccess] = createSignal(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess(false);

    try {
        // check if avatar url is empty
        if(formData().avatarUrl === '') {
            formData().avatarUrl = defaultAvatar;
        }

        const response = await fetch('https://vps.eldc.dk/api/Profile', {
            method: 'POST',
            headers: {
            'Content-Type': 'application/json',
            },
            body: JSON.stringify(formData()),
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Registration failed');
        }

        setSuccess(true);
        // Redirect to login after 2 seconds
        setTimeout(() => {
            window.location.href = '/vp/login';
        }, 2000);
        } catch (err) {
            setError(err.message || 'Registration failed. Please try again.');
        }
  };

  const handleInput = (field) => (e) => {
    setFormData({ ...formData(), [field]: e.target.value });
  };

  return (
    <div class={styles.pageContainer}>
      <h1>Register</h1>
      <div class={styles.formContainer}>
        <form onSubmit={handleSubmit} class={styles.form}>
          {error() && <div class={styles.error}>{error()}</div>}
          {success() && <div class={styles.success}>Registration successful! Redirecting to login...</div>}
          
          <div class={styles.formGroup}>
            <label for="username">Username</label>
            <input
              type="text"
              id="username"
              value={formData().username}
              onInput={handleInput('username')}
              required
            />
          </div>

          <div class={styles.formGroup}>
            <label for="password">Password</label>
            <input
              type="password"
              id="password"
              value={formData().password}
              onInput={handleInput('password')}
              required
            />
          </div>

          <div class={styles.formGroup}>
            <label for="email">Email</label>
            <input
              type="email"
              id="email"
              value={formData().email}
              onInput={handleInput('email')}
              required
            />
          </div>

          <div class={styles.formGroup}>
            <label for="firstName">First Name</label>
            <input
              type="text"
              id="firstName"
              value={formData().firstName}
              onInput={handleInput('firstName')}
              required
            />
          </div>

          <div class={styles.formGroup}>
            <label for="lastName">Last Name</label>
            <input
              type="text"
              id="lastName"
              value={formData().lastName}
              onInput={handleInput('lastName')}
              required
            />
          </div>

          <div class={styles.formGroup}>
            <label for="bio">Bio</label>
            <textarea
              id="bio"
              value={formData().bio}
              onInput={handleInput('bio')}
              rows="3"
            />
          </div>

          <div class={styles.formGroup}>
            <label for="avatarUrl">Avatar URL</label>
            <input
              type="url"
              id="avatarUrl"
              value={formData().avatarUrl}
              onInput={handleInput('avatarUrl')}
              placeholder="https://example.com/avatar.jpg"
            />
          </div>

          <button type="submit" class={styles.button}>Register</button>
        </form>
        
        <div class={styles.formFooter}>
          <p>Already have an account? <a href="/vp/login">Login</a></p>
        </div>
      </div>
    </div>
  );
}

export default Register; 