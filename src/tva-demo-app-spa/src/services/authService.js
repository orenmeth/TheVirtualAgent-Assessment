import apiClient from './api';

class AuthService {
  login(credentials) {
    return apiClient.post('/auth/login', credentials);
  }

  register(userData) {
    return apiClient.post('/auth/register', userData);
  }
}

export default new AuthService();