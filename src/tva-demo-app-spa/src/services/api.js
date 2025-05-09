import axios from 'axios';
import { useAuthStore } from 'src/stores/authStore';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:44345';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use(
  (config) => {
    const authStore = useAuthStore();
    const token = authStore.authToken;
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

apiClient.interceptors.response.use(
  response => response,
  error => {
    if (error.response && error.response.status === 401) {
      const authStore = useAuthStore();
      authStore.logout();
      console.error('Unauthorized, logging out.');
    }
    return Promise.reject(error);
  }
);

export default apiClient;