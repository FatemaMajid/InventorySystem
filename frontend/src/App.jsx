import { useEffect } from "react";
import { BrowserRouter } from "react-router-dom";
import { LanguageProvider } from "./context/LanguageContext";
import { ThemeProvider } from "./context/ThemeContext";
import AppRoutes from "./routes/AppRoutes";
import {
  loginAdmin,
  isAuthenticated,
} from "./services/authService";

function App() {
  useEffect(() => {
    const initializeAuth = async () => {
      if (!isAuthenticated()) {
        try {
          await loginAdmin();
        } catch (error) {
          console.error("Auto login failed:", error);
        }
      }
    };

    initializeAuth();
  }, []);

  return (
    <ThemeProvider>
      <LanguageProvider>
        <BrowserRouter>
          <AppRoutes />
        </BrowserRouter>
      </LanguageProvider>
    </ThemeProvider>
  );
}

export default App;