import { createContext, useCallback, useContext, useState } from "react";
import {
    getCurrentUser,
    isAuthenticated,
    login,
    logout as logoutService,
} from "../services/authService";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
    const [user, setUser] = useState(() => getCurrentUser());
    const [authenticated, setAuthenticated] = useState(() => isAuthenticated());
    const [loading, setLoading] = useState(false);

    const signIn = useCallback(async (username, password) => {
        setLoading(true);

        try {
            const response = await login(username, password);
            setUser(response.user);
            setAuthenticated(true);
            return response;
        } finally {
            setLoading(false);
        }
    }, []);

    const signOut = useCallback(() => {
        logoutService();
        setUser(null);
        setAuthenticated(false);
    }, []);

    const value = {
        user,
        authenticated,
        loading,
        signIn,
        signOut,
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);

    if (!context) {
        throw new Error("useAuth must be used within an AuthProvider.");
    }

    return context;
}