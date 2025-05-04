import { defineStore } from 'pinia';
import { ref } from 'vue';
import { api } from 'boot/axios';

export const useTransactionsStore = defineStore('TransactionsStore', () => {
  const transactions = ref([]);
  const transaction = ref(null);
  const totalItems = ref(0);
  const loading = ref(false);
  const error = ref(null);

  async function getTransactions(page = 1, pageSize = 10) {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.get(`/Transaction/GetTransactions/page/${page}/pageSize/${pageSize}`);
      if (response.status === 200) {
        transactions.value = response.data.items;
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

  async function getTransactionByCode(code) {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.get(`/Transaction/GetTransaction/${code}`);
      if (response.status === 200) {
        transaction.value = response.data;
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

  async function saveTransaction(transactionData) {
    loading.value = true;
    error.value = null;
    try {
      if (transactionData.code === '' || transactionData.code === null || transactionData.code === 'null') {
        transactionData.code = 0;
      }
      await api.post(`/Transaction/UpsertTransaction`, transactionData, {
        headers: { 'Content-Type': 'application/json' },
      });
    } catch (err) {
      error.value = `Failed to save Transaction: ${err.message}`;
    } finally {
      loading.value = false;
    }
  }

  return {
    transactions,
    transaction,
    totalItems,
    loading,
    error,
    getTransactions,
    getTransactionByCode,
    saveTransaction,
  };
});