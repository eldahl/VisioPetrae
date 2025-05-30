import { createSignal, onMount } from 'solid-js';
import styles from '../App.module.css';

function Inference() {
  const [token, setToken] = createSignal('');
  const [selectedFile, setSelectedFile] = createSignal(null);
  const [previewUrl, setPreviewUrl] = createSignal('');
  const [prompt, setPrompt] = createSignal('What is shown inthis image?');
  const [isLoading, setIsLoading] = createSignal(false);
  const [error, setError] = createSignal('');
  const [result, setResult] = createSignal(null);

  onMount(() => {
    const storedToken = localStorage.getItem('token');
    if (!storedToken) {
      window.location.href = '/vp/login';
      return;
    }
    setToken(storedToken);
  });

  const handleFileChange = (event) => {
    const file = event.target.files[0];
    if (file) {
      setSelectedFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setPreviewUrl(reader.result);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    if (!selectedFile()) {
      setError('Please select a file');
      return;
    }

    setIsLoading(true);
    setError('');
    setResult(null);

    const formData = new FormData();
    formData.append('file', selectedFile());
    formData.append('prompt', prompt());

    try {
      const response = await fetch('https://vps.eldc.dk/api/Inference/request_inference_job', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token()}`,
        },
        body: formData,
      });

      if (!response.ok) {
        throw new Error('Inference request failed');
      }

      const data = await response.json();
      setResult(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div class={styles.pageContainer}>
      <div class={styles.apiInfo}>
        <p>Use the user token to send requests to the API endpoint:</p>
        <p class={styles.endpoint}>POST https://vps.eldc.dk/api/Inference/request_inference_job</p>
        <div class={styles.tokenContainer}>
          <p>Your token:</p>
          <code class={styles.token}>{token()}</code>
        </div>
      </div>

      <h1>Or.. Run Inference directly from your browser:</h1>
      <div class={styles.inferenceContainer}>
        <form onSubmit={handleSubmit}>
          {error() && <div class={styles.error}>{error()}</div>}
          
          <div class={styles.formGroup}>
            <label for="file">Select Image</label>
            <input
              type="file"
              id="file"
              accept="image/*"
              onChange={handleFileChange}
              required
            />
          </div>

          <div class={styles.formGroup}>
            <label for="prompt">Prompt</label>
            <textarea
              id="prompt"
              value={prompt()}
              onInput={(e) => setPrompt(e.target.value)}
              rows="4"
              required
              placeholder="Enter your prompt here..."
            />
          </div>

          {previewUrl() && (
            <div class={styles.previewContainer}>
              <img src={previewUrl()} alt="Preview" class={styles.preview} />
            </div>
          )}

          <button type="submit" class={styles.button} disabled={isLoading()}>
            {isLoading() ? 'Processing...' : 'Run Inference'}
          </button>
        </form>

        {result() && (
          <div class={styles.resultContainer}>
            <h2>Result</h2>
            <pre>{JSON.stringify(result(), null, 2)}</pre>
          </div>
        )}
      </div>
    </div>
  );
}

export default Inference; 