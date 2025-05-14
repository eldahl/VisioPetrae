import styles from '../App.module.css';

export default function Pricing() {
  return (
    <>
    <div class={styles.pageContainer}>
      <h1>Pricing</h1>
      <div class={styles.pricingContainer}>
        <div class={styles.pricingCard}>
          <h2>Basic</h2>
          <p class={styles.price}>$99<span>/month</span></p>
          <ul>
            <li>500 images per month</li>
            <li>Basic image analysis</li>
            <li>Standard descriptions</li>
            <li>Email support</li>
          </ul>
          <button class={styles.button}>Get Started</button>
        </div>
        
        <div class={`${styles.pricingCard} ${styles.featured}`}>
          <h2>Professional</h2>
          <p class={styles.price}>$299<span>/month</span></p>
          <ul>
            <li>2,000 images per month</li>
            <li>Advanced image analysis</li>
            <li>Detailed descriptions</li>
            <li>Custom tagging</li>
            <li>Priority support</li>
          </ul>
          <button class={styles.button}>Get Started</button>
        </div>
        
        <div class={styles.pricingCard}>
          <h2>Enterprise</h2>
          <p class={styles.price}>Custom</p>
          <ul>
            <li>Unlimited images</li>
            <li>Full feature access</li>
            <li>Custom integration</li>
            <li>Dedicated support</li>
            <li>SLA agreement</li>
          </ul>
          <button class={styles.button}>Contact Us</button>
        </div>
      </div>
    </div>
    </>
  );
} 