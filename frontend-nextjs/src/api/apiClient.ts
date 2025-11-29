import { getToken, clearToken } from '@/api/utils';

const API_BASE_URL = '/api';

async function fetchApi(url: string, options: RequestInit = {}) {
    const headers = new Headers(options.headers);

    // Set 'Content-Type' default, allowing override from options.
    // This is safer than the original object spread with HeadersInit type.
    if (!headers.has('Content-Type')) {
        headers.set('Content-Type', 'application/json');
    }

    // Add JWT token to headers for all requests except login
    if (url !== '/authentication/Auth/Login') {
        const token = getToken();
        if (token) {
            headers.set('Authorization', `Bearer ${token}`);
        }
    }

    const response = await fetch(`${API_BASE_URL}${url}`, {
        ...options,
        headers,
    });

    // Handle 204 No Content immediately. It's a success state with no body.
    if (response.status === 204) {
        return null;
    }

    // Handle unauthorized access by redirecting to login, but NOT for the login page itself.
    if (response.status === 401 && url !== '/authentication/Auth/Login') {
        clearToken();
        // Redirect to login page
        window.location.href = '/login';
        // Return a promise that never resolves to prevent further processing
        return new Promise(() => {});
    }

    if (!response.ok) {
        let errorMessage;
        try {
            // Try to parse as JSON first
            const errorBody = await response.json();
            // Look for common error message keys, handling various structures
            errorMessage = errorBody.message ||
                           errorBody.title ||
                           errorBody.error ||
                           (Array.isArray(errorBody.errors) && errorBody.errors[0]) ||
                           JSON.stringify(errorBody);
        } catch (e) {
            // If JSON parsing fails, read as text as a fallback
            try {
                errorMessage = await response.text();
                 if (!errorMessage) { // Handle cases where the text body is empty
                    errorMessage = `Request failed with status: ${response.status} ${response.statusText}`;
                }
            } catch (textError) {
                 errorMessage = `Request failed with status: ${response.status} ${response.statusText}, and the error body could not be read.`;
            }
        }
        throw new Error(errorMessage);
    }

    // Now that we've handled 204 and other errors, we can safely parse JSON.
    return response.json();
}

export const apiClient = {
    get: <T>(url: string, options?: RequestInit): Promise<T> => fetchApi(url, { method: 'GET', ...options }),
    post: <T>(url: string, body: any): Promise<T> => fetchApi(url, { method: 'POST', body: JSON.stringify(body) }),
    put: <T>(url: string, body: any): Promise<T> => fetchApi(url, { method: 'PUT', body: JSON.stringify(body) }),
    patch: <T>(url: string, body: any): Promise<T> => fetchApi(url, { method: 'PATCH', body: JSON.stringify(body) }),
    delete: <T>(url: string, body?: any): Promise<T> => fetchApi(url, { method: 'DELETE', body: body ? JSON.stringify(body) : undefined }),
};