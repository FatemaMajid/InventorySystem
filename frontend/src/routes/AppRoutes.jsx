import { Routes, Route, Navigate } from 'react-router-dom';

import Layout from '../components/Layout/Layout';

import Home from '../screens/Home/Home';
import Dashboard from '../screens/Dashboard/Dashboard';
import InventorySessions from '../screens/InventorySessions/InventorySessions';
import NewInventorySession from '../screens/NewInventorySession/NewInventorySession';
import Reports from '../screens/Reports/Reports';
import AttentionItems from '../screens/AttentionItems/AttentionItems';

function AppRoutes() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<Home />} />

        <Route
          path="/dashboard"
          element={<Dashboard />}
        />

        <Route
          path="/inventory-sessions"
          element={<InventorySessions />}
        />

        <Route
          path="/new-inventory-session"
          element={<NewInventorySession />}
        />

        <Route
          path="/comparison-results"
          element={<Navigate to="/reports" replace />}
        />

        <Route
          path="/attention-items"
          element={<AttentionItems />}
        />

        <Route
          path="/reports"
          element={<Reports />}
        />
      </Route>

      <Route
        path="*"
        element={<Navigate to="/" replace />}
      />
    </Routes>
  );
}

export default AppRoutes;