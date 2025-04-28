import { defineStore, acceptHMRUpdate } from 'pinia';
import { ref } from 'vue';
import { api } from 'boot/axios';

export const useAccountsStore = defineStore('AccountsStore', () => {
  const Accounts = ref([]);
  const totalItems = ref(0);
  const loading = ref(false);
  const error = ref(null);

  async function fetchAccounts(page = 1, pageSize = 10) {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.get(`/Account/GetAccounts/page/${page}/pageSize/${pageSize}`);
      if (response.status === 200) {
        Accounts.value = response.data.items;
        totalItems.value = response.data.totalItems;
      } else {
        error.value = `Failed to fetch Accounts: HTTP status ${response.status}`;
      }
    } catch (err) {
      error.value = `Failed to fetch Accounts: ${err.message}`;
    } finally {
      loading.value = false;
    }
  }

  async function fetchAccountByCode(code) {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.get(`/Account/GetAccount/${code}`);
      if (response.status === 200) {
        return response.data;
      } else {
        error.value = `Failed to fetch Account: HTTP status ${response.status}`;
      }
    } catch (err) {
      error.value = `Failed to fetch Account: ${err.message}`;
    } finally {
      loading.value = false;
    }
  }

  return {
    Accounts,
    totalItems,
    loading,
    error,
    fetchAccounts,
    fetchAccountByCode,
  };
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useAccountsStore, import.meta.hot))
}
