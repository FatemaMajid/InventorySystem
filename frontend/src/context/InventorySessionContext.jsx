import { useState } from "react";
import { InventorySessionContext } from "./InventorySessionContextValue";



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
