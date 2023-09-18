import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import { GoogleOAuthProvider } from '@react-oauth/google';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(<GoogleOAuthProvider clientId="1096475234419-442tf3u29nsd8qi50qh6rmfe5i1sdqjk.apps.googleusercontent.com">
<React.StrictMode>
    <App />
</React.StrictMode>
</GoogleOAuthProvider>);

