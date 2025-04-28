import { defineStore, acceptHMRUpdate } from 'pinia';
import { ref } from 'vue';
import { api } from 'boot/axios';

export const useTransactionsStore = defineStore('TransactionsStore', () => {
  const Transactions = ref([]);
  const totalItems = ref(0);
  const loading = ref(false);
  const error = ref(null);

  async function fetchTransactions(page = 1, pageSize = 10) {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.get(`/Transaction/GetTransactions/page/${page}/pageSize/${pageSize}`);
      if (response.status === 200) {
        Transactions.value = response.data.items;
        totalItems.value = response.data.totalItems;
      } else {
        error.value = `Failed to fetch Transactions: HTTP status ${response.status}`;
      }
    } catch (err) {
      error.value = `Failed to fetch Transactions: ${err.message}`;
    } finally {
      loading.value = false;
    }
  }

  async function fetchTransactionByCode(code) {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.get(`/Transaction/GetTransaction/${code}`);
      if (response.status === 200) {
        return response.data;
      } else {
        error.value = `Failed to fetch Transaction: HTTP status ${response.status}`;
      }
    } catch (err) {
      error.value = `Failed to fetch Transaction: ${err.message}`;
    } finally {
      loading.value = false;
    }
  }

  async function updateTransaction() { }

  async function addTransaction() { }

  return {
    Transactions,
    totalItems,
    loading,
    error,
    fetchTransactions,
    fetchTransactionByCode,
    updateTransaction,
    addTransaction,
  };
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useTransactionsStore, import.meta.hot))
}
