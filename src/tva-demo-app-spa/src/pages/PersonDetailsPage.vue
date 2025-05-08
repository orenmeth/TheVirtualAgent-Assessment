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
              <q-input
                outlined
                v-model="personForm.name"
                hint="Enter a valid first name."
                clearable
                label="Name"
                :rules="[isNotMoreThanMaxLength(50)]"
              />
              <q-input
                outlined
                v-model="personForm.surname"
                hint="Enter a valid surname."
                clearable
                label="Surname"
                :rules="[isNotMoreThanMaxLength(50)]"
              />
              <q-input
                outlined
                v-model="personForm.idNumber"
                hint="Enter a valid ID number."
                clearable
                label="ID Number"
                :rules="[isRequired, isNotMoreThanMaxLength(50), isNotExistingIdNumber(personForm.code)]"
              />

              <div class="q-mt-md">
                <q-btn
                  type="submit"
                  color="primary"
                  :label="personId ? 'Save Changes' : 'Create Person'"
                />
                <q-btn
                  v-if="personForm.code && (personAccounts.length === 0 || personAccounts.filter(account => account.accountStatusId === 1).length === 0)"
                  type="button"
                  color="negative"
                  label="Delete"
                  class="q-ml-sm"
                  @click="deletePerson"
                />
                <q-btn flat class="q-ml-sm" @click="$router.push({ name: 'persons' })">Close</q-btn>
              </div>
            </q-form>
          </q-card-section>
        </q-card>
      </div>

      <div class="col-lg-8 col-md-8 col-sm-12 col-xs-12">
        <q-card flat bordered>
          <q-toolbar>
            <q-toolbar-title>Accounts</q-toolbar-title>
            <q-space />
            <q-btn
              v-if="personForm.code"
              outline
              color="primary"
              label="Add New Account"
              @click="navigateToAccountDetails()"
            />
          </q-toolbar>
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

              <template v-slot:body-cell-accountStatusId="props">
                <q-td :props="props">
                  <q-chip
                    :color="props.row.accountStatusId === 1 ? 'green' : 'red'"
                    :text-color="props.row.accountStatusId === 1 ? 'white' : 'black'"
                    dense
                    rounded
                    class="text-weight-medium q-px-sm"
                  >
                  {{ accountsStore.accountStatuses.find((status) => status.id === props.row.accountStatusId)?.description || 'Unknown' }}
                  </q-chip>
                </q-td>
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
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useQuasar } from 'quasar'
import { usePersonsStore } from 'src/stores/personsStore'
import { useAccountsStore } from 'src/stores/accountsStore'
import { isRequired, isNotMoreThanMaxLength, isNotExistingIdNumber } from 'src/utils/validationRules'

const $q = useQuasar()

const route = useRoute()
const router = useRouter()
const personsStore = usePersonsStore()
const accountsStore = useAccountsStore()
const personId = ref(parseInt(route.params.personId, 10))
const loading = ref(false)
const personForm = ref({
  code: '',
  name: '',
  surname: '',
  idNumber: '',
})
const personAccounts = ref([])
const accountColumns = ref([
  {
    name: 'code',
    required: true,
    label: 'Account Code',
    align: 'left',
    field: 'code',
    sortable: true,
  },
  {
    name: 'accountNumber',
    label: 'Account Number',
    align: 'left',
    field: 'accountNumber',
    sortable: true,
  },
  {
    name: 'outstandingBalance',
    label: 'Outstanding Balance',
    align: 'right',
    field: 'outstandingBalance',
    sortable: true,
    format: (val) => parseFloat(val).toFixed(2),
  },
  {
    name: 'accountStatusId',
    label: 'Account Status',
    align: 'center',
    field: 'accountStatusId',
    sortable: true,
  },
  { name: 'actions', label: 'Actions', field: 'actions', align: 'right' },
])

const fetchPersonDetails = async (id) => {
  loading.value = true
  try {
    const person = await personsStore.getPersonByCode(id)
    if (person) {
      personForm.value = {
        code: person.code,
        name: person.name,
        surname: person.surname,
        idNumber: person.idNumber,
      }
      personAccounts.value = person.accounts || []
    } else {
      $q.notify({
        type: 'error',
        message: 'Person not found',
      })
      router.push({ name: 'persons' })
    }
  } catch (error) {
    console.error('Error fetching person details:', error)
    $q.notify({
      type: 'error',
      message: 'Failed to load person details',
    })
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  if (personId.value) {
    accountsStore.getAccountStatuses().then((statuses) => {
      accountsStore.accountStatuses = statuses
    })
    fetchPersonDetails(personId.value)
  }
})

const savePersonDetails = async () => {
  loading.value = true
  try {
    await personsStore.savePerson(personForm.value)
    $q.notify({ type: 'positive', message: 'Person details saved successfully!' })
    await personsStore.getPersons('code', null, false, 1, 10)
    router.push({ name: 'persons' })
  } catch (error) {
    console.error('Error saving person details:', error)
    $q.notify({
      type: 'error',
      message: 'Failed to save person details',
    })
  } finally {
    loading.value = false
  }
}

const deletePerson = async () => {
  loading.value = true
  try {
    await personsStore.deletePerson(personForm.value.code)
    $q.notify({ type: 'positive', message: 'Person deleted successfully!' })
    router.push({ name: 'persons' })
  } catch (error) {
    console.error('Error deleting person:', error)
    $q.notify({
      type: 'error',
      message: error,
    })
  } finally {
    loading.value = false
  }
}

const navigateToAccountDetails = (account) => {
  if (account) {
    router.push({ name: 'account_details', params: { accountId: account.code } })
  } else {
    router.push({ name: 'account_details', params: { personId: personForm.value.code } })
  }
}
</script>

<style scoped>
</style>
