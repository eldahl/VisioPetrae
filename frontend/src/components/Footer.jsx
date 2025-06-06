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
            <li><a href="/vp/">Home</a></li>
            <li><a href="/vp/features">Features</a></li>
            <li><a href="/vp/contact">Contact</a></li>
          </ul>
        </div>

        <div className="footer-section">
          <h4>Resources</h4>
          <ul>
            <li><a href="#"><s>Documentation</s></a></li>
            <li><a href="#"><s>API</s></a></li>
            <li><a href="#"><s>Privacy Policy</s></a></li>
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