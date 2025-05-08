import logo from './logo.svg';
import styles from './App.module.css';
import { createSignal, Show, onCleanup, onMount } from 'solid-js';

function App() {
  const [isNavMenuOpen, setIsNavMenuOpen] = createSignal(false);
  const [isProfileMenuOpen, setIsProfileMenuOpen] = createSignal(false);
  const [windowWidth, setWindowWidth] = createSignal(window.innerWidth);

  const toggleNavMenu = () => {
    setIsNavMenuOpen(!isNavMenuOpen());
  };
  const toggleProfileMenu = () => {
    setIsProfileMenuOpen(!isProfileMenuOpen());
  };

  const handleClickOutside = (event) => {
    if (!event.target.closest(`.${styles.nav}`)) {
      setIsNavMenuOpen(false);
    }
  };

  const handleResize = () => {
    setWindowWidth(window.innerWidth);
  };

  onMount(() => {
    document.addEventListener('click', handleClickOutside);
    window.addEventListener('resize', handleResize);
  });

  onCleanup(() => {
    document.removeEventListener('click', handleClickOutside);
    window.removeEventListener('resize', handleResize);
  });

  return (
    <div class={styles.App}>
      <header class={styles.header}>
        <img src={logo} class={styles.logo} alt="logo" />
        <nav class={styles.nav}>
          <button class={styles.navMenuToggle} onClick={toggleNavMenu}>☰</button>
          <Show when={windowWidth() > 600 || isNavMenuOpen()}>
            <ul class={windowWidth() < 600 && isNavMenuOpen() ? styles.navDropdownMenu : styles.navList}>
              <li><a href="#">Home</a></li>
              <li><a href="#">Features</a></li>
              <li><a href="#">Pricing</a></li>
              <li><a href="#">Contact</a></li>
            </ul>
          </Show>
          <div class={styles.profileMenu} onClick={toggleProfileMenu}>
            <svg class={styles.profilePic} xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <circle cx="12" cy="8" r="4" />
              <path d="M4 20c0-2.21 3.58-4 8-4s8 1.79 8 4" fill="currentColor" />
              <rect x="4" y="20" width="16" height="8" fill="currentColor" />
            </svg>
            <span class={styles.profileMenuIcon}>▼</span>
            {isProfileMenuOpen() && (
              <ul class={styles.profileDropdownMenu}>
                <li><a href="#">Profile</a></li>
                <li><a href="#">Settings</a></li>
                <li><a href="#">Logout</a></li>
              </ul>
            )}
          </div>
        </nav>  
      </header>
      <div class={styles.welcomeContainer}>
        <h1>What can Visio Petrae <br />do for you?</h1>
        <svg class={styles.clouds} xmlns="http://www.w3.org/2000/svg" viewBox="0 -8 200 25" fill="none" stroke="currentColor" strokeWidth="0.5">
          <g fill="white">
            <circle cx="3" cy="0" r="2" />
            <circle cx="6" cy="0" r="3" />
            <circle cx="9" cy="0" r="3" />
            <circle cx="12" cy="0" r="2" />
          </g>
          <g fill="white">
            <circle cx="23" cy="2" r="1" />
            <circle cx="26" cy="2" r="2" />
            <circle cx="29" cy="2" r="2" />
            <circle cx="32" cy="2" r="1" />
          </g>
          <g fill="white">
            <circle cx="39" cy="-2" r="1.5" />
            <circle cx="42" cy="-2" r="2.5" />
            <circle cx="45" cy="-2" r="2.5" />
            <circle cx="48" cy="-2" r="1.5" />
          </g>
          <g fill="white">
            <circle cx="60" cy="0" r="1.5" />
            <circle cx="63" cy="0" r="2.5" />
            <circle cx="66" cy="0" r="2.5" />
            <circle cx="69" cy="0" r="1.5" />
          </g>
          <g fill="white">
            <circle cx="83.5" cy="-2" r="0.5" />
            <circle cx="85.5" cy="-2" r="1.5" />
            <circle cx="88" cy="-2" r="1.5" />
            <circle cx="90" cy="-2" r="0.5" />
          </g>
          <g fill="white">
            <circle cx="103" cy="0" r="2" />
            <circle cx="106" cy="0" r="3" />
            <circle cx="109" cy="0" r="3" />
            <circle cx="112" cy="0" r="2" />
          </g>
          <g fill="white">
            <circle cx="123" cy="2" r="1" />
            <circle cx="126" cy="2" r="2" />
            <circle cx="129" cy="2" r="2" />
            <circle cx="132" cy="2" r="1" />
          </g>
          <g fill="white">
            <circle cx="139" cy="-2" r="1.5" />
            <circle cx="142" cy="-2" r="2.5" />
            <circle cx="145" cy="-2" r="2.5" />
            <circle cx="148" cy="-2" r="1.5" />
          </g>
          <g fill="white">
            <circle cx="160" cy="0" r="1.5" />
            <circle cx="163" cy="0" r="2.5" />
            <circle cx="166" cy="0" r="2.5" />
            <circle cx="169" cy="0" r="1.5" />
          </g>
          <g fill="white">
            <circle cx="183.5" cy="-2" r="0.5" />
            <circle cx="185.5" cy="-2" r="1.5" />
            <circle cx="188" cy="-2" r="1.5" />
            <circle cx="190" cy="-2" r="0.5" />
          </g>
        </svg>
        <svg class={styles.cloudsInfScroll} xmlns="http://www.w3.org/2000/svg" viewBox="0 -8 200 25" fill="none" stroke="currentColor" strokeWidth="0.5">
          <g fill="white">
            <circle cx="3" cy="0" r="2" />
            <circle cx="6" cy="0" r="3" />
            <circle cx="9" cy="0" r="3" />
            <circle cx="12" cy="0" r="2" />
          </g>
          <g fill="white">
            <circle cx="23" cy="2" r="1" />
            <circle cx="26" cy="2" r="2" />
            <circle cx="29" cy="2" r="2" />
            <circle cx="32" cy="2" r="1" />
          </g>
          <g fill="white">
            <circle cx="39" cy="-2" r="1.5" />
            <circle cx="42" cy="-2" r="2.5" />
            <circle cx="45" cy="-2" r="2.5" />
            <circle cx="48" cy="-2" r="1.5" />
          </g>
          <g fill="white">
            <circle cx="60" cy="0" r="1.5" />
            <circle cx="63" cy="0" r="2.5" />
            <circle cx="66" cy="0" r="2.5" />
            <circle cx="69" cy="0" r="1.5" />
          </g>
          <g fill="white">
            <circle cx="83.5" cy="-2" r="0.5" />
            <circle cx="85.5" cy="-2" r="1.5" />
            <circle cx="88" cy="-2" r="1.5" />
            <circle cx="90" cy="-2" r="0.5" />
          </g>
          <g fill="white">
            <circle cx="103" cy="0" r="2" />
            <circle cx="106" cy="0" r="3" />
            <circle cx="109" cy="0" r="3" />
            <circle cx="112" cy="0" r="2" />
          </g>
          <g fill="white">
            <circle cx="123" cy="2" r="1" />
            <circle cx="126" cy="2" r="2" />
            <circle cx="129" cy="2" r="2" />
            <circle cx="132" cy="2" r="1" />
          </g>
          <g fill="white">
            <circle cx="139" cy="-2" r="1.5" />
            <circle cx="142" cy="-2" r="2.5" />
            <circle cx="145" cy="-2" r="2.5" />
            <circle cx="148" cy="-2" r="1.5" />
          </g>
          <g fill="white">
            <circle cx="160" cy="0" r="1.5" />
            <circle cx="163" cy="0" r="2.5" />
            <circle cx="166" cy="0" r="2.5" />
            <circle cx="169" cy="0" r="1.5" />
          </g>
          <g fill="white">
            <circle cx="183.5" cy="-2" r="0.5" />
            <circle cx="185.5" cy="-2" r="1.5" />
            <circle cx="188" cy="-2" r="1.5" />
            <circle cx="190" cy="-2" r="0.5" />
          </g>
        </svg>
      </div>
      <div class={styles.cardContainer}>
        <div class={styles.card}>
          <h2>Image Analysis</h2>
          <p>Using AI to analyze images and extract detailed information about the image for intelligence gathering and analysis.</p>
        </div>
        <div class={styles.card}>
          <h2>Image Description</h2>
          <p>Using AI to describe images in detail your company can communicate detailed information about your images.</p>
        </div>
        <div class={styles.card}>
          <h2>Image Tagging</h2>
          <p>Using AI to tag images with relevant keywords your company can index your images for search.</p>
        </div>
      </div>
    </div>
  );
}

export default App;
