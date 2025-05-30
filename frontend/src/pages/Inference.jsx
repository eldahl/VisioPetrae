import styles from '../App.module.css';
import { createSignal } from 'solid-js';

export default function Inference() {
  const [response, setResponse] = createSignal('');
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    
    const form = e.target;
    const formData = new FormData();
    const imageFile = form.image.files[0];
    const promptText = form.prompt.value;

    formData.append('image', imageFile);
    formData.append('prompt', promptText);

    try {
      const response = await fetch('https://vps.eldc.dk/api/Inference/request_inference_job', {
        method: 'POST',
        body: formData
      });

      const result = await response.text();
      setResponse(result);
    } catch (error) {
      setResponse('Error: ' + error.message);
    }
  };

  return (
    <>
      <div class={styles.pageContainer}>
        <h1>Upload Image with Prompt</h1>
        <div class={styles.inferenceContainer}>
          <form onSubmit={handleSubmit}>
            <label for="image">Choose an image:</label>
            <input type="file" id="image" name="image" accept="image/*" required />

            <label for="prompt">Enter prompt:</label>
            <textarea id="prompt" name="prompt" rows="4" required></textarea>

            <button type="submit">Send to Server</button>
          </form>
          <div id="response">{response()}</div>
        </div>
      </div>
    </>
  );
} 