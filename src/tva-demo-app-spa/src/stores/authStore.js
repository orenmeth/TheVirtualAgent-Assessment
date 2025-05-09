import { defineStore } from 'pinia';
import authService from 'src/services/authService';
import router from 'src/router';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || null,
    user: JSON.parse(localStorage.getItem('user')) || null,
    loginStatus: {
      isLoading: false,
      error: null,
    },
  }),
  getters: {
    isLoggedIn: (state) => !!state.token,
    currentUser: (state) => state.user,
    authToken: (state) => state.token,
    isLoading: (state) => state.loginStatus.isLoading,
    authError: (state) => state.loginStatus.error,
  },
  actions: {
    async login(credentials) {
      this.loginStatus.isLoading = true;
      this.loginStatus.error = null;
      try {
        const response = await authService.login(credentials);
        if (response.data && response.data.token) {
          const { token, username } = response.data;

          this.token = token;
          this.user = { username };

          localStorage.setItem('token', token);
          localStorage.setItem('user', JSON.stringify(this.user));
          
          return true;
        }
      } catch (error) {
        const errorMessage = error.response?.data?.message || error.message || 'Login failed. Please try again.';
        this.loginStatus.error = errorMessage;
        this.token = null;
        this.user = null;
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        console.error('Login error:', error.response || error);
        return false;
      } finally {
        this.loginStatus.isLoading = false;
      }
    },
    logout() {
      this.token = null;
      this.user = null;
      this.loginStatus.error = null;
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      router.push('/login');
    },
    async register(userData) {
      this.loginStatus.isLoading = true;
      this.loginStatus.error = null;
      try {
        await authService.register(userData);
        return true;
      } catch (error) {
        const errorMessage = error.response?.data?.message || error.message || 'Registration failed. Please try again.';
        this.loginStatus.error = errorMessage;
        console.error('Registration error:', error.response || error);
        return false;
      } finally {
        this.loginStatus.isLoading = false;
      }
    },
    initializeAuth() {
        const token = localStorage.getItem('token');
        const user = localStorage.getItem('user');
        if (token && user) {
            this.token = token;
            this.user = JSON.parse(user);
        } else {
            this.token = null;
            this.user = null;
        }
    }
  },
});