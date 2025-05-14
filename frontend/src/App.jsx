import logo from './logo.svg';
import styles from './App.module.css';
import { createSignal, Show, onCleanup, onMount } from 'solid-js';




function App(props) {
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

  // Close menu when a link is clicked
  const handleNavLinkClick = () => {
    setIsNavMenuOpen(false);
  };

  return (
      <div class={styles.App}>
        <header class={styles.header}>
          <a href="/vp/"><img src={logo} class={styles.logo} alt="logo" /></a>
          <nav class={styles.nav}>
            <button class={styles.navMenuToggle} onClick={toggleNavMenu}>☰</button>
            <Show when={windowWidth() > 600 || isNavMenuOpen()}>
              <ul class={windowWidth() < 600 && isNavMenuOpen() ? styles.navDropdownMenu : styles.navList}>
                <li><a href="/vp/" onClick={handleNavLinkClick}>Home</a></li>
                <li><a href="/vp/features" onClick={handleNavLinkClick}>Features</a></li>
                <li><a href="/vp/pricing" onClick={handleNavLinkClick}>Pricing</a></li>
                <li><a href="/vp/contact" onClick={handleNavLinkClick}>Contact</a></li>
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
                  <li><a href="/vp/profile">Profile</a></li>
                  <li><a href="/vp/settings">Settings</a></li>
                  <li><a href="/vp/logout">Logout</a></li>
                </ul>
              )}
            </div>
          </nav>  
        </header>
        <props.children />
      </div>
  );
}

export default App;
