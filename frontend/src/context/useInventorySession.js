import { useContext } from "react";
import { InventorySessionContext } from "./InventorySessionContextValue";
export function useInventorySession() {
  const context = useContext(InventorySessionContext);

  if (!context) {
    throw new Error(
      "useInventorySession must be used inside InventorySessionProvider"
    );
  }

  return context;
}
