import API_BASE_URL from './apiConfig';

async function parseResponse(response) {
  const contentType =
    response.headers.get('content-type') || '';

  if (contentType.includes('application/json')) {
    return response.json();
  }

  const text = await response.text();

  return text || null;
}

async function request(endpoint, options = {}) {
  const {
    method = 'GET',
    body,
    headers = {},
  } = options;

  const isFormData = body instanceof FormData;

  const requestHeaders = {
    ...headers,
  };

  if (!isFormData && body !== undefined) {
    requestHeaders['Content-Type'] =
      'application/json';
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

  const data = await parseResponse(response);

  if (!response.ok) {
    let message =
      `Request failed with status ${response.status}.`;

    if (typeof data === 'string' && data) {
      message = data;
    }

    if (typeof data === 'object' && data) {
      message =
        data.message ||
        data.title ||
        data.error ||
        message;
    }

    throw new Error(message);
  }

  return data;
}

const apiClient = {
  get(endpoint, options = {}) {
    return request(endpoint, {
      ...options,
      method: 'GET',
    });
  },

  post(endpoint, body, options = {}) {
    return request(endpoint, {
      ...options,
      method: 'POST',
      body,
    });
  },

  put(endpoint, body, options = {}) {
    return request(endpoint, {
      ...options,
      method: 'PUT',
      body,
    });
  },

  patch(endpoint, body, options = {}) {
    return request(endpoint, {
      ...options,
      method: 'PATCH',
      body,
    });
  },

  delete(endpoint, options = {}) {
    return request(endpoint, {
      ...options,
      method: 'DELETE',
    });
  },

  postForm(endpoint, formData, options = {}) {
    return request(endpoint, {
      ...options,
      method: 'POST',
      body: formData,
    });
  },

  putForm(endpoint, formData, options = {}) {
    return request(endpoint, {
      ...options,
      method: 'PUT',
      body: formData,
    });
  },
};

export default apiClient;