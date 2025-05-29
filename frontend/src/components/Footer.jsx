import './Footer.css';
import img from '../assets/vp-rock-2.png';
const Footer = () => {
  return (
    <footer className="footer">
      <div className="footer-content">
        <div className="footer-section">
          <img src={img} width={100}/>
          <h3>The seeing-rock company</h3>
          <p>Silicon & AI-powered image analysis</p>
        </div>
        
        <div className="footer-section">
          <h4>Quick Links</h4>
          <ul>
            <li><a href="/">Home</a></li>
            <li><a href="/about">About</a></li>
            <li><a href="/contact">Contact</a></li>
          </ul>
        </div>

        <div className="footer-section">
          <h4>Resources</h4>
          <ul>
            <li><a href="/docs">Documentation</a></li>
            <li><a href="/api">API</a></li>
            <li><a href="/privacy">Privacy Policy</a></li>
          </ul>
        </div>
      </div>
      
      <div className="footer-bottom">
        <p>&copy; {new Date().getFullYear()} The seeing rock company. All rights reserved.</p>
      </div>
    </footer>
  );
};
export default Footer;