import API_BASE_URL from "./apiConfig";

async function parseResponse(response, responseType = "auto") {
  if (responseType === "blob") {
    return response.blob();
  }

  const contentType =
    response.headers.get("content-type") || "";

  if (contentType.includes("application/json")) {
    return response.json();
  }

  const text = await response.text();

  return text || null;
}

async function request(endpoint, options = {}) {
  const {
    method = "GET",
    body,
    headers = {},
    responseType = "auto",
  } = options;

  const isFormData = body instanceof FormData;
  const token = localStorage.getItem("inventory_token");

  const requestHeaders = {
    ...headers,
  };

  if (token) {
    requestHeaders.Authorization = `Bearer ${token}`;
  }

  if (!isFormData && body !== undefined) {
    requestHeaders["Content-Type"] = "application/json";
  }

  const response = await fetch(
    `${API_BASE_URL}${endpoint}`,
    {
      method,
      headers: requestHeaders,
      body: isFormData
        ? body
        : body !== undefined
          ? JSON.stringify(body)
          : undefined,
    }
  );

  const data = await parseResponse(response, responseType);

  if (response.status === 401) {
    localStorage.removeItem("inventory_token");
    localStorage.removeItem("inventory_user");
  }

  if (!response.ok) {
    let message =
      `Request failed with status ${response.status}.`;

    if (typeof data === "string" && data) {
      message = data;
    }

    if (typeof data === "object" && data) {
      message =
        data.message ||
        data.title ||
        data.error ||
        message;
    }

    const error = new Error(message);
    error.status = response.status;
    error.data = data;

    throw error;
  }

  return data;
}

const apiClient = {
  get(endpoint, options = {}) {
    return request(endpoint, {
      ...options,
      method: "GET",
    });
  },

  post(endpoint, body, options = {}) {
    return request(endpoint, {
      ...options,
      method: "POST",
      body,
    });
  },

  put(endpoint, body, options = {}) {
    return request(endpoint, {
      ...options,
      method: "PUT",
      body,
    });
  },

  patch(endpoint, body, options = {}) {
    return request(endpoint, {
      ...options,
      method: "PATCH",
      body,
    });
  },

  delete(endpoint, options = {}) {
    return request(endpoint, {
      ...options,
      method: "DELETE",
    });
  },

  postForm(endpoint, formData, options = {}) {
    return request(endpoint, {
      ...options,
      method: "POST",
      body: formData,
    });
  },

  putForm(endpoint, formData, options = {}) {
    return request(endpoint, {
      ...options,
      method: "PUT",
      body: formData,
    });
  },
};

export default apiClient;