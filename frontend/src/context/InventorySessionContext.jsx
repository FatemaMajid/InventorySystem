import { createContext, useContext, useState } from "react";

const InventorySessionContext = createContext(null);

export function InventorySessionProvider({ children }) {
  const [activeSession, setActiveSession] = useState(null);

  const clearSession = () => setActiveSession(null);

  return (
    <InventorySessionContext.Provider
      value={{ activeSession, setActiveSession, clearSession }}
    >
      {children}
    </InventorySessionContext.Provider>
  );
}

export function useInventorySession() {
  const context = useContext(InventorySessionContext);

  if (!context) {
    throw new Error(
      "useInventorySession must be used inside InventorySessionProvider"
    );
  }

  return context;
}