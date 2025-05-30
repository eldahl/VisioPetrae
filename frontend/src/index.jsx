/* @refresh reload */
import { render } from 'solid-js/web';
import { Router, Route, A } from '@solidjs/router';

import './index.css';
import App from './App';

// Import page components
import Home from './pages/Home';
import Features from './pages/Features';
import Pricing from './pages/Pricing';
import Contact from './pages/Contact';
import Inference from './pages/Inference';
import Login from './pages/Login';
import Register from './pages/Register';

const root = document.getElementById('root');

if (import.meta.env.DEV && !(root instanceof HTMLElement)) {
  throw new Error(
    'Root element not found. Did you forget to add it to your index.html? Or maybe the id attribute got misspelled?',
  );
}

render(() => (
<Router>
  <Route path="/vp/" component={() => <App><Home /></App>} />
  <Route path="/vp/features" component={() => <App><Features /></App>} />
  <Route path="/vp/pricing" component={() => <App><Pricing /></App>} />
  <Route path="/vp/contact" component={() => <App><Contact /></App>} />
  <Route path="/vp/inference" component={() => <App><Inference /></App>} />
  <Route path="/vp/login" component={() => <App><Login /></App>} />
  <Route path="/vp/register" component={() => <App><Register /></App>} />
</Router>), root);
