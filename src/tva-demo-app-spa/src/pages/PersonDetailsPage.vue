<template>
  <q-page class="q-pa-md">
    <div class="row q-col-gutter-sm q-ma-xs q-mr-sm">
      <div class="col-lg-4 col-md-4 col-sm-12 col-xs-12">
        <q-card flat bordered class>
          <q-card-section>
            <div class="text-h6">Person Details</div>
          </q-card-section>
          <q-separator inset></q-separator>
          <q-card-section>
            <q-form @submit.prevent="savePersonDetails" class="q-gutter-md">
            <q-input outlined v-model="personForm.code" label="Code" readonly />
            <q-input outlined v-model="personForm.name" label="Name" :rules="[val => val.length <= 50 || 'Name must be less than 50 characters']" />
            <q-input outlined v-model="personForm.surname" label="Surname" :rules="[val => val.length <= 50 || 'Surname must be less than 50 characters']" />
            <q-input outlined v-model="personForm.idNumber" label="ID Number" :rules="[val => val.length <= 50 || 'ID number must be less than 50 characters']" />

            <div class="q-mt-md">
              <q-btn type="submit" color="primary" :label="personId ? 'Save Changes' : 'Create Person'" />
              <q-btn flat class="q-ml-sm" @click="$router.go(-1)">Cancel</q-btn>
            </div>
          </q-form>
          </q-card-section>
        </q-card>
      </div>

      <div class="col-lg-8 col-md-8 col-sm-12 col-xs-12">
        <q-card flat bordered>
          <q-card-section>
            <div class="text-h6">Accounts</div>
          </q-card-section>
          <q-separator inset></q-separator>
          <q-card-section>
            <q-table
            v-if="personId && personAccounts.length > 0"
            :rows="personAccounts"
            :columns="accountColumns"
            row-key="accountNumber"
            :pagination="{ rowsPerPage: 10, page: 1 }"
            :loading="loading"
          >
            <template v-slot:body-cell-actions="props">
              <q-btn
                class="q-mr-xs"
                color="primary"
                flat
                icon="more_vert"
                @click="navigateToAccountDetails(props.row)"
              >
                <q-tooltip>View/Edit</q-tooltip>
              </q-btn>
            </template>

            <template v-slot:no-data>
              <div class="text-center q-pa-md">No accounts found.</div>
            </template>
          </q-table>
          <div v-else-if="personId">No accounts associated with this person.</div>
          <div v-else>No accounts will be associated until the person is created.</div>
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useQuasar } from 'quasar';
import { usePersonsStore } from 'src/stores/personsStore'

const $q = useQuasar();

const route = useRoute();
const router = useRouter();
const personsStore = usePersonsStore();

const personId = ref(parseInt(route.params.personId, 10));
const loading = ref(false);
const personForm = ref({
  code: '',
  name: '',
  surname: '',
  idNumber: '',
});
const personAccounts = ref([]);
const accountColumns = ref([
  { name: 'code', required: true, label: 'Account Code', align: 'left', field: 'code', sortable: true },
  { name: 'accountNumber', label: 'Account Number', align: 'left', field: 'accountNumber', sortable: true },
  { name: 'outstandingBalance', label: 'Outstanding Balance', align: 'right', field: 'outstandingBalance', sortable: true, format: (val) => parseFloat(val).toFixed(2) },
  { name: 'actions', label: 'Actions', field: 'actions', align: 'right' },
]);

const fetchPersonDetails = async (id) => {
  loading.value = true;
  try {
    const person = await personsStore.fetchPersonByCode(id);
    if (person) {
      personForm.value = {
        code: person.code,
        name: person.name,
        surname: person.surname,
        idNumber: person.idNumber,
      };
      personAccounts.value = person.accounts || [];
    } else {
      $q.notify({
        type: 'error',
        message: 'Person not found',
      });
      router.push({ name: 'persons' });
    }
  } catch (error) {
    console.error('Error fetching person details:', error);
    $q.notify({
      type: 'error',
      message: 'Failed to load person details',
    });
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  if (personId.value) {
    fetchPersonDetails(personId.value);
  }
});

const savePersonDetails = async () => { // Make savePersonDetails async
  loading.value = true;
  try {
    if (personId.value) {
      // await personsStore.updatePerson(personId.value, personForm.value); // Adjust the method name in store
      $q.notify({ type: 'positive', message: 'Person details updated successfully!' });
    } else {
      // await personsStore.createPerson(personForm.value);  // Adjust the method name in store
      $q.notify({ type: 'positive', message: 'New person created successfully!' });
    }
    router.push({ name: 'persons' });
  } catch (error) {
    console.error('Error saving person details:', error);
    $q.notify({
      type: 'error',
      message: 'Failed to save person details',
    });
  } finally {
    loading.value = false;
  }
};

const navigateToAccountDetails = (account) => {
  if (account) {
    router.push({ name: 'account_details', params: { accountId: account.code } });
  } else {
    router.push({ name: 'account_details' });
  }
};
</script>

<style lang="scss">

</style>
