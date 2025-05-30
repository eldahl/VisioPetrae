import { createSignal } from 'solid-js';
import styles from '../App.module.css';

function Login() {
  const [username, setUsername] = createSignal('');
  const [password, setPassword] = createSignal('');
  const [error, setError] = createSignal('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    try {
      const response = await fetch('https://vps.eldc.dk/api/Profile/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          username: username(),
          password: password(),
        }),
      });

      if (!response.ok) {
        throw new Error('Login failed');
      }

      const data = await response.json();
      localStorage.setItem('token', data.token);
      window.location.href = '/vp/profile';
    } catch (err) {
      setError('Invalid username or password');
    }
  };

  return (
    <div class={styles.pageContainer}>
      <h1>Login</h1>
      <div class={styles.formContainer}>
        <form onSubmit={handleSubmit} class={styles.form}>
          {error() && <div class={styles.error}>{error()}</div>}
          
          <div class={styles.formGroup}>
            <label for="username">Username</label>
            <input
              type="text"
              id="username"
              value={username()}
              onInput={(e) => setUsername(e.target.value)}
              required
            />
          </div>

          <div class={styles.formGroup}>
            <label for="password">Password</label>
            <input
              type="password"
              id="password"
              value={password()}
              onInput={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          <button type="submit" class={styles.button}>Login</button>
        </form>
        
        <div class={styles.formFooter}>
          <p>Don't have an account? <a href="/vp/register">Register</a></p>
          <p><a href="/vp/forgot-password">Forgot Password?</a></p>
        </div>
      </div>
    </div>
  );
}

export default Login; 