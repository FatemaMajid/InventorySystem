import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./styles/reset.css";
import "./styles/variables.css";
import "./styles/globals.css";
import App from "./App.jsx";
import { InventorySessionProvider } from "./context/InventorySessionContext.jsx";

createRoot(document.getElementById("root")).render(
  <StrictMode>
    <InventorySessionProvider>
      <App />
    </InventorySessionProvider>
  </StrictMode>
);