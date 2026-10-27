import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { getCurrentUser } from "../services/authService";
import { hasPermission } from "../services/permissionService";

function PermissionRoute({ permission }) {
    const { authenticated } = useAuth();
    const location = useLocation();
    const user = getCurrentUser();

    if (!authenticated) {
        return <Navigate to="/login" replace state={{ from: location }} />;
    }

    if (!permission) {
        return <Outlet />;
    }

    if (user?.role?.toLowerCase() === "manager") {
        return <Outlet />;
    }

    if (hasPermission(permission)) {
        return <Outlet />;
    }

    return <Navigate to="/" replace />;
}

export default PermissionRoute;