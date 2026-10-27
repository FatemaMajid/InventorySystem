import { Navigate, Route, Routes } from "react-router-dom";

import Layout from "../components/Layout/Layout";
import PermissionRoute from "./PermissionRoute";
import ProtectedRoute from "./ProtectedRoute";

import Login from "../screens/Login/Login";
import Home from "../modules/inventory/screens/Home/Home";
import SystemHome from "../screens/SystemHome/SystemHome";
import Dashboard from "../modules/inventory/screens/Dashboard/Dashboard";
import InventorySessions from "../modules/inventory/screens/InventorySessions/InventorySessions";
import NewInventorySession from "../modules/inventory/screens/NewInventorySession/NewInventorySession";
import Reports from "../modules/inventory/screens/Reports/Reports";
import AttentionItems from "../modules/inventory/screens/AttentionItems/AttentionItems";
import Branches from "../screens/Branches/Branches";
import Stores from "../screens/Stores/Stores";
import Users from "../screens/Users/Users";
import RolesPermissions from "../screens/RolesPermissions/RolesPermissions";
import AuditLog from "../screens/AuditLog/AuditLog";

import FinancialHome from "../modules/financial/screens/Home/FinancialHome";
import FinancialDashboard from "../screens/FinancialAnalysis/FinancialAnalysis";
import FinancialSection from "../modules/financial/screens/Section/FinancialSection";

function AppRoutes() {
  return (
    <Routes>
      <Route
        path="/login"
        element={<Login />}
      />

      <Route element={<ProtectedRoute />}>
        <Route element={<Layout />}>
          <Route
            path="/modules"
            element={<SystemHome />}
          />

          <Route
            path="/"
            element={<Home />}
          />

          <Route
            path="/dashboard"
            element={
              <PermissionRoute permission="Dashboard.View" />
            }
          >
            <Route
              index
              element={<Dashboard />}
            />
          </Route>

          <Route
            path="/inventory-sessions"
            element={
              <PermissionRoute permission="InventorySession.View" />
            }
          >
            <Route
              index
              element={<InventorySessions />}
            />
          </Route>

          <Route
            path="/new-inventory-session"
            element={
              <PermissionRoute permission="InventorySession.Create" />
            }
          >
            <Route
              index
              element={<NewInventorySession />}
            />
          </Route>

          <Route
            path="/comparison-results"
            element={
              <Navigate
                to="/reports"
                replace
              />
            }
          />

          <Route
            path="/attention-items"
            element={
              <PermissionRoute permission="Attention.View" />
            }
          >
            <Route
              index
              element={<AttentionItems />}
            />
          </Route>

          <Route
            path="/reports"
            element={
              <PermissionRoute permission="Report.View" />
            }
          >
            <Route
              index
              element={<Reports />}
            />
          </Route>

          <Route
            path="/financial-analysis"
            element={
              <PermissionRoute permission="FinancialAnalysis.View" />
            }
          >
            <Route
              index
              element={<FinancialHome />}
            />
          </Route>

          <Route
            path="/financial-analysis/dashboard"
            element={
              <PermissionRoute permission="FinancialDashboard.View" />
            }
          >
            <Route
              index
              element={<FinancialDashboard />}
            />
          </Route>

          <Route
            path="/financial-analysis/periods"
            element={
              <PermissionRoute permission="FinancialPeriod.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="periods"
                  tone="amber"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/uploads"
            element={
              <PermissionRoute permission="FinancialPeriod.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="uploads"
                  tone="teal"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/sales"
            element={
              <PermissionRoute permission="FinancialSales.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="sales"
                  tone="blue"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/inventory"
            element={
              <PermissionRoute permission="FinancialInventory.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="inventory"
                  tone="teal"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/expenses"
            element={
              <PermissionRoute permission="FinancialExpenses.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="expenses"
                  tone="terracotta"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/purchases"
            element={
              <PermissionRoute permission="FinancialPurchases.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="purchases"
                  tone="amber"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/profit-loss"
            element={
              <PermissionRoute permission="FinancialProfitLoss.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="profitLoss"
                  tone="green"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/customers-employees"
            element={
              <PermissionRoute permission="FinancialCustomers.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="customersEmployees"
                  tone="blue"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/debt"
            element={
              <PermissionRoute permission="FinancialDebt.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="debt"
                  tone="terracotta"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/cash"
            element={
              <PermissionRoute permission="FinancialCash.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="cash"
                  tone="teal"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/suppliers"
            element={
              <PermissionRoute permission="FinancialSuppliers.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="suppliers"
                  tone="amber"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/capital"
            element={
              <PermissionRoute permission="FinancialCapital.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="capital"
                  tone="purple"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/transfers"
            element={
              <PermissionRoute permission="FinancialTransfers.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="transfers"
                  tone="teal"
                />
              }
            />
          </Route>

          <Route
            path="/financial-analysis/settlements"
            element={
              <PermissionRoute permission="FinancialSettlements.View" />
            }
          >
            <Route
              index
              element={
                <FinancialSection
                  sectionKey="settlements"
                  tone="purple"
                />
              }
            />
          </Route>

          <Route
            path="/branches"
            element={
              <PermissionRoute permission="Branch.View" />
            }
          >
            <Route
              index
              element={<Branches />}
            />
          </Route>

          <Route
            path="/stores"
            element={
              <PermissionRoute permission="Store.View" />
            }
          >
            <Route
              index
              element={<Stores />}
            />
          </Route>

          <Route
            path="/users"
            element={
              <PermissionRoute permission="User.View" />
            }
          >
            <Route
              index
              element={<Users />}
            />
          </Route>

          <Route
            path="/roles-permissions"
            element={
              <PermissionRoute permission="Role.View" />
            }
          >
            <Route
              index
              element={<RolesPermissions />}
            />
          </Route>

          <Route
            path="/audit-logs"
            element={
              <PermissionRoute permission="AuditLog.View" />
            }
          >
            <Route
              index
              element={<AuditLog />}
            />
          </Route>
        </Route>
      </Route>

      <Route
        path="*"
        element={
          <Navigate
            to="/modules"
            replace
          />
        }
      />
    </Routes>
  );
}

export default AppRoutes;