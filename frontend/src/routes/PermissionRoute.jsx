import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { getToken } from "../services/authService";

function PermissionRoute({ permission }) {
    const { authenticated } = useAuth();
    const location = useLocation();

    if (!authenticated) {
        return <Navigate to="/login" replace state={{ from: location }} />;
    }

    if (!permission) {
        return <Outlet />;
    }

    const token = getToken();

    if (!token) {
        return <Navigate to="/login" replace state={{ from: location }} />;
    }

    try {
        const tokenPayload = token.split(".")[1];
        const normalizedPayload = tokenPayload
            .replace(/-/g, "+")
            .replace(/_/g, "/");

        const paddedPayload =
            normalizedPayload +
            "=".repeat((4 - (normalizedPayload.length % 4)) % 4);

        const payload = JSON.parse(atob(paddedPayload));
        const permissions = payload.permission || [];

        const userPermissions = Array.isArray(permissions)
            ? permissions
            : [permissions];

        const hasPermission = userPermissions.some(
            (userPermission) =>
                userPermission.toLowerCase() === permission.toLowerCase()
        );

        if (!hasPermission) {
            return <Navigate to="/" replace />;
        }
    } catch {
        return <Navigate to="/login" replace state={{ from: location }} />;
    }

    return <Outlet />;
}

export default PermissionRoute;