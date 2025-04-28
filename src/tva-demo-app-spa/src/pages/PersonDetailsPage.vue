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
            <q-input outlined v-model="personForm.name" label="Name" :rules="[val => val && val.length > 0 || 'Please enter a name']" />
            <q-input outlined v-model="personForm.surname" label="Surname" />
            <q-input outlined v-model="personForm.idNumber" label="ID Number" />

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
            title="Person Accounts"
            :rows="personAccounts"
            :columns="accountColumns"
            row-key="account_number"
            :pagination="{ rowsPerPage: 10, page: 1 }"
          >
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

const $q = useQuasar();

const route = useRoute();
const router = useRouter();

const personId = ref(route.params.personId);
const personDetails = ref(null);
const personForm = ref({
  code: '',
  name: '',
  surname: '',
  idNumber: '',
});
const personAccounts = ref([]);
const accountColumns = ref([
  { name: 'code', required: true, label: 'Account Code', align: 'left', field: 'code', sortable: true },
  { name: 'account_number', label: 'Account Number', align: 'left', field: 'account_number', sortable: true },
  { name: 'outstanding_balance', label: 'Outstanding Balance', align: 'right', field: 'outstanding_balance', sortable: true, format: (val) => parseFloat(val).toFixed(2) },
]);

onMounted(() => {
  if (personId.value) {
    fetchPersonDetails(personId.value);
    fetchPersonAccounts(personId.value);
  }
});

const fetchPersonDetails = (id) => {
  // Simulate fetching person details (replace with your actual API call)
  console.log(id);
  setTimeout(() => {
    const foundPerson = { code: 'P001', name: 'John', surname: 'Doe', idNumber: '1234567890' }; // Replace with actual data
    personDetails.value = { ...foundPerson };
    personForm.value = { ...foundPerson };
  }, 300);
};

const fetchPersonAccounts = (personId) => {
  // Simulate fetching person accounts (replace with your actual API call)
  console.log(personId);
  setTimeout(() => {
    const accountsData = [
      { code: 'A123', person_code: 'P001', account_number: 'ACC001', outstanding_balance: 1250.50 },
      { code: 'B456', person_code: 'P001', account_number: 'ACC002', outstanding_balance: 580.25 },
      // ... more accounts
    ];
    personAccounts.value = accountsData;
  }, 500);
};

const savePersonDetails = () => {
  if (personId.value) {
    // Logic to update the existing person (e.g., API call)
    $q.notify({ type: 'positive', message: 'Person details updated successfully!' });
    router.push({ name: 'persons' });
  } else {
    // Logic to create a new person (e.g., API call)
    $q.notify({ type: 'positive', message: 'New person created successfully!' });
    router.push({ name: 'persons' });
  }
};
</script>
