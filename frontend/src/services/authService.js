import apiClient from "./api/apiClient";

const ADMIN_USERNAME = "admin";
const ADMIN_PASSWORD = "Admin@12345";

export async function loginAdmin() {
  const response = await apiClient.post(
    "/api/Auth/login",
    {
      username: ADMIN_USERNAME,
      password: ADMIN_PASSWORD,
    }
  );

  const token =
    response?.token ||
    response?.accessToken ||
    response?.access_token;

  if (!token) {
    console.error("Login response:", response);

    throw new Error(
      "Login succeeded but JWT token was not found."
    );
  }

  localStorage.setItem(
    "inventory_token",
    token
  );

  localStorage.setItem(
    "inventory_user",
    ADMIN_USERNAME
  );

  return response;
}

export function logout() {
  localStorage.removeItem("inventory_token");
  localStorage.removeItem("inventory_user");
}

export function isAuthenticated() {
  return Boolean(
    localStorage.getItem("inventory_token")
  );
}