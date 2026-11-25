import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import './index.css';
import { ProveedorAuth } from './contexto/AuthContexto';
import { ProveedorToast } from './contexto/ToastContexto';

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <ProveedorToast>
        <ProveedorAuth>
          <App />
        </ProveedorAuth>
      </ProveedorToast>
    </BrowserRouter>
  </StrictMode>
);
