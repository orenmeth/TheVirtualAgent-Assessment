import { defineStore, acceptHMRUpdate } from 'pinia';
import { ref } from 'vue';
import { api } from 'boot/axios';

export const usePersonsStore = defineStore('personsStore', () => {
  const persons = ref([]);
  const totalItems = ref(0);
  const loading = ref(false);
  const error = ref(null);

  async function fetchPersons(sortBy, descending, page = 1, pageSize = 10, filter = null) {
    loading.value = true;
    error.value = null;
    try {
      const response = await api.get(`/Person/GetPersons/page/${page}/pageSize/${pageSize}`);
      if (response.status === 200) {
        // Move to server-side filtering
        persons.value = response.data.items.filter(item => {
          if (!filter) return true;
          if (item.name.toLowerCase().includes(filter.toLowerCase())) return true;
          if (item.surname.toLowerCase().includes(filter.toLowerCase())) return true;
          if (item.idNumber.toLowerCase().includes(filter.toLowerCase())) return true;
        }).sort((a, b) => a - b);
        if (descending) {
          persons.value.reverse();
        }
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

  async function fetchPersonByCode(code) {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.get(`/Person/GetPerson/${code}`);
      if (response.status === 200) {
        return response.data;
      } else {
        error.value = `Failed to fetch person: HTTP status ${response.status}`;
      }
    } catch (err) {
      error.value = `Failed to fetch person: ${err.message}`;
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
    fetchPersonByCode,
  };
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(usePersonsStore, import.meta.hot))
}
