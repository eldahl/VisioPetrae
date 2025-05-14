import styles from '../App.module.css';

export default function Features() {
  return (
    <>
    <div class={styles.pageContainer}>
      <h1>Features</h1>
      <div class={styles.featuresGrid}>
        <div class={styles.featureCard}>
          <h2>Advanced Image Analysis</h2>
          <p>Our AI can analyze images pixel by pixel to extract information about objects, colors, positions, and more.</p>
        </div>
        <div class={styles.featureCard}>
          <h2>Detailed Image Description</h2>
          <p>Get comprehensive descriptions of images including subject matter, context, mood, and setting.</p>
        </div>
        <div class={styles.featureCard}>
          <h2>Automated Image Tagging</h2>
          <p>Automatically generate tags for your images to improve searchability and organization.</p>
        </div>
        <div class={styles.featureCard}>
          <h2>Batch Processing</h2>
          <p>Process thousands of images at once with our powerful batch processing capabilities.</p>
        </div>
        <div class={styles.featureCard}>
          <h2>API Integration</h2>
          <p>Easily integrate our image analysis capabilities into your existing systems through our comprehensive API.</p>
        </div>
        <div class={styles.featureCard}>
          <h2>Custom Models</h2>
          <p>Train custom models to recognize specific elements relevant to your business needs.</p>
        </div>
      </div>
    </div>
    </>
  );
} 