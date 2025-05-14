import styles from '../App.module.css';
import { createSignal } from 'solid-js';

export default function Contact() {
  const [name, setName] = createSignal('');
  const [email, setEmail] = createSignal('');
  const [message, setMessage] = createSignal('');
  const [submitted, setSubmitted] = createSignal(false);

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log({ name: name(), email: email(), message: message() });
    setSubmitted(true);
    // In a real application, you would submit this data to your backend
  };

  return (
    <>
    <div class={styles.pageContainer}>
      <h1>Contact Us</h1>
      
      {!submitted() ? (
        <form class={styles.contactForm} onSubmit={handleSubmit}>
          <div class={styles.formGroup}>
            <label for="name">Name</label>
            <input 
              type="text" 
              id="name" 
              value={name()} 
              onInput={(e) => setName(e.target.value)} 
              required 
            />
          </div>
          
          <div class={styles.formGroup}>
            <label for="email">Email</label>
            <input 
              type="email" 
              id="email" 
              value={email()} 
              onInput={(e) => setEmail(e.target.value)} 
              required 
            />
          </div>
          
          <div class={styles.formGroup}>
            <label for="message">Message</label>
            <textarea 
              id="message" 
              rows="5" 
              value={message()} 
              onInput={(e) => setMessage(e.target.value)} 
              required
            />
          </div>
          
          <button type="submit" class={styles.button}>Send Message</button>
        </form>
      ) : (
        <div class={styles.thankYouMessage}>
          <h2>Thank you for your message!</h2>
          <p>We will get back to you as soon as possible.</p>
          <button class={styles.button} onClick={() => setSubmitted(false)}>Send Another Message</button>
        </div>
      )}
      
      <div class={styles.contactInfo}>
        <div>
          <h3>Email</h3>
          <p>contact@visiopetrae.com</p>
        </div>
        <div>
          <h3>Phone</h3>
          <p>+1 (555) 123-4567</p>
        </div>
        <div>
          <h3>Address</h3>
          <p>123 AI Plaza, Tech District<br />San Francisco, CA 94103</p>
        </div>
      </div>
    </div>
    </>
  );
} 