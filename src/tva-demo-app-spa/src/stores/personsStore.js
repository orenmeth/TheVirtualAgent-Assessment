import { defineStore, acceptHMRUpdate } from 'pinia';
import { ref } from 'vue';
import { api } from 'boot/axios';

export const usePersonsStore = defineStore('personsStore', () => {
  const persons = ref([]);
  const totalItems = ref(0);
  const loading = ref(false);
  const error = ref(null);

  async function fetchPersons(page = 1, pageSize = 10) {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.get(`/Person/GetPersons/page/${page}/pageSize/${pageSize}`);
      if (response.status === 200) {
        persons.value = response.data.items;
        totalItems.value = response.data.totalItems;
      } else {
        error.value = `Failed to fetch persons: HTTP status ${response.status}`;
      }
    } catch (err) {
      error.value = `Failed to fetch persons: ${err.message}`;
    } finally {
      loading.value = false;
    }
  }

  return {
    persons,
    totalItems,
    loading,
    error,
    fetchPersons,
  };
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(usePersonsStore, import.meta.hot))
}
