import { Navigate, Route, Routes } from "react-router-dom";
import Layout from "../components/Layout/Layout";
import PermissionRoute from "./PermissionRoute";
import ProtectedRoute from "./ProtectedRoute";
import Login from "../screens/Login/Login";
import Home from "../screens/Home/Home";
import Dashboard from "../screens/Dashboard/Dashboard";
import InventorySessions from "../screens/InventorySessions/InventorySessions";
import NewInventorySession from "../screens/NewInventorySession/NewInventorySession";
import Reports from "../screens/Reports/Reports";
import AttentionItems from "../screens/AttentionItems/AttentionItems";
import Branches from "../screens/Branches/Branches";
import Stores from "../screens/Stores/Stores";
import Users from "../screens/Users/Users";
import RolesPermissions from "../screens/RolesPermissions/RolesPermissions";
import AuditLog from "../screens/AuditLog/AuditLog";

function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />

      <Route element={<ProtectedRoute />}>
        <Route element={<Layout />}>
          <Route path="/" element={<Home />} />

          <Route
            path="/dashboard"
            element={<PermissionRoute permission="Dashboard.View" />}
          >
            <Route index element={<Dashboard />} />
          </Route>

          <Route
            path="/inventory-sessions"
            element={
              <PermissionRoute permission="InventorySession.View" />
            }
          >
            <Route index element={<InventorySessions />} />
          </Route>

          <Route
            path="/new-inventory-session"
            element={
              <PermissionRoute permission="InventorySession.Create" />
            }
          >
            <Route index element={<NewInventorySession />} />
          </Route>

          <Route
            path="/comparison-results"
            element={<Navigate to="/reports" replace />}
          />

          <Route
            path="/attention-items"
            element={<PermissionRoute permission="Attention.View" />}
          >
            <Route index element={<AttentionItems />} />
          </Route>

          <Route
            path="/reports"
            element={<PermissionRoute permission="Report.View" />}
          >
            <Route index element={<Reports />} />
          </Route>

          <Route
            path="/branches"
            element={<PermissionRoute permission="Branch.View" />}
          >
            <Route index element={<Branches />} />
          </Route>

          <Route
            path="/stores"
            element={<PermissionRoute permission="Store.View" />}
          >
            <Route index element={<Stores />} />
          </Route>

          <Route
            path="/users"
            element={<PermissionRoute permission="User.View" />}
          >
            <Route index element={<Users />} />
          </Route>

          <Route
            path="/roles-permissions"
            element={<PermissionRoute permission="Role.View" />}
          >
            <Route index element={<RolesPermissions />} />
          </Route>

          <Route
            path="/audit-logs"
            element={<PermissionRoute permission="AuditLog.View" />}
          >
            <Route index element={<AuditLog />} />
          </Route>
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default AppRoutes;