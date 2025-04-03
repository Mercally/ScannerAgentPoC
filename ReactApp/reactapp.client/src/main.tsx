import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
//import App from './App.tsx'
import ScannerList from './components/scanner-list/ScannerList.tsx';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
        <ScannerList />
  </StrictMode>,
)
