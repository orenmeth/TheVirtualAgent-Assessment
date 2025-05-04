import { defineStore } from 'pinia';
import { api } from 'boot/axios';
import { ref } from 'vue';

export const usePersonsStore = defineStore('personsStore', () => {
    const persons = ref([]);
    const loading = ref(false);
    const person = ref(null);
    const error = ref(null);

    async function getPersons(sortBy, descending, page = 1, pageSize = 10, filter = null) {
      loading.value = true;
      error.value = null;
      try {
        const response = await api.get(`/Person/GetPersons/page/${page}/pageSize/${pageSize}/sortBy/${sortBy}/descending/${descending}`);
        persons.value = response.data.items.filter(item => {
          if (!filter) return true;
          if (item.name.toLowerCase().includes(filter.toLowerCase())) return true;
          if (item.surname.toLowerCase().includes(filter.toLowerCase())) return true;
          if (item.idNumber.toLowerCase().includes(filter.toLowerCase())) return true;
        }).sort((a, b) => a - b);
        if (descending) {
          persons.value.reverse();
        }
      } catch (error) {
        console.error('Error fetching persons:', error);
      } finally {
        loading.value = false;
      }
    }

    async function getPersonByCode(code) {
      loading.value = true;
      error.value = null;
      try {
        const response = await api.get(`/Person/GetPerson/${code}`);
        person.value = response.data;
        return response.data;
      } catch (error) {
        console.error('Error fetching person by code:', error);
        return null;
      } finally {
        loading.value = false;
      }
    }

    async function upsertPerson(personData) {
      loading.value = true;
      error.value = null;

      const existingPersonIndex = persons.value.findIndex(p => p.code === personData.code);

      try {
          const response = await api.post(`/persons/upsert`, personData);

          if (existingPersonIndex > -1) {
              persons.value[existingPersonIndex] = response.data;
          } else {
              persons.value.push(response.data);
          }
          person.value = response.data;
          return response.data;
      }
       catch (error) {
        if (existingPersonIndex > -1) {
          console.error('Error updating person:', error);
        } else {
          console.error('Error creating person:', error);
        }          
        throw error;
      } finally {
          loading.value = false;
      }
    }

    async function deletePerson(code) {
      loading.value = true;
      error.value = null;
      try {
        await api.delete(`/persons/${code}`);
        persons.value = this.persons.filter(p => p.code !== code);
        person.value = null;
      } catch (error) {
        console.error('Error deleting person:', error);
        throw error;
      } finally {
        loading.value = false;
      }
    }

    return {
      persons,
      loading,
      person,
      error,
      getPersons,
      getPersonByCode,
      deletePerson,
      upsertPerson,
    };
});