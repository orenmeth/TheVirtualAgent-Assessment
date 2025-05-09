import { defineStore } from 'pinia';
import { api } from 'boot/axios';
import { ref } from 'vue';

export const usePersonsStore = defineStore('personsStore', () => {
    const persons = ref([]);
    const loading = ref(false);
    const person = ref(null);
    const totalItems = ref(0);
    const error = ref(null);

    async function getPersons(sortBy, filter, descending, page = 1, pageSize = 10) {
      loading.value = true;
      error.value = null;
      try {
        let url = `/Person/GetPersons?page=${page}&pageSize=${pageSize}&descending=${descending}&sortBy=${sortBy}&filter=${filter}`;
        const response = await api.get(url);
        persons.value = response.data.items
        totalItems.value = response.data.totalItems;
      } catch (error) {
        console.error('Error fetching persons:', error);
        persons.value = [];
        totalItems.value = 0;
        error.value = error;
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

    async function savePerson(personData) {
      loading.value = true;
      error.value = null;

      const existingPersonIndex = persons.value.findIndex(p => p.code === personData.code);

      try {
        if (personData.code === '' || personData.code === null || personData.code === 'null') {
          personData.code = 0;
        }
        await api.post(`/Person/UpsertPerson`, personData);
        if (existingPersonIndex > -1) {
          persons.value[existingPersonIndex] = personData;
        } else {
          persons.value.push(personData);
        }
      } catch (error) {
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
        const person = await getPersonByCode(code);
        if (person.accounts.length === 0 || person.accounts.filter((account) => account.accountStatusId === 1).length === 0) {
          await api.delete(`/Person/DeletePerson/${code}`);
          persons.value = this.persons.filter(p => p.code !== code);
          person.value = null;
        } else {
          throw new Error('Cannot delete person with active accounts.');
        }
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
      totalItems,
      getPersons,
      getPersonByCode,
      deletePerson,
      savePerson,
    };
});
