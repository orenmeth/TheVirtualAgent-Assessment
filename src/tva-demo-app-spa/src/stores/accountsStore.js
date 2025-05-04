import { defineStore, acceptHMRUpdate } from 'pinia';
import { ref } from 'vue';
import { api } from 'boot/axios';

export const useAccountsStore = defineStore('AccountsStore', () => {
  const accounts = ref([]);
  const totalItems = ref(0);
  const loading = ref(false);
  const error = ref(null);

  async function getAccounts(page = 1, pageSize = 10) {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.get(`/Account/GetAccounts/page/${page}/pageSize/${pageSize}`);
      if (response.status === 200) {
        accounts.value = response.data.items;
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

  async function getAccountByCode(code) {
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

  async function saveAccount(accountData) {
    loading.value = true;
    error.value = null;

    const existingAccountIndex = accounts.value.findIndex(a => a.code === accountData.code);

    try {
      if (accountData.code === '' || accountData.code === null || accountData.code === 'null') {
        accountData.code = 0;
      }
      await api.post(`/Account/UpsertAccount`, accountData);        
      if (existingAccountIndex > -1) {
        accounts.value[existingAccountIndex] = accountData;
      } else {
        accounts.value.push(accountData);
      }
    } catch (err) {
      error.value = `Failed to save Account: ${err.message}`;
    } finally {
      loading.value = false;
    }
  }

  return {
    accounts,
    totalItems,
    loading,
    error,
    getAccounts,
    getAccountByCode,
    saveAccount,
  };
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useAccountsStore, import.meta.hot))
}
