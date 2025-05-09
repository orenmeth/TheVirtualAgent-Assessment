<template>
  <q-page class="q-pa-md">
    <div class="row q-col-gutter-sm q-ma-xs q-mr-sm">
      <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
        <q-card flat bordered>
          <q-card-section>
            <div class="text-h6">
              Account Details
              <q-chip
                v-if="accountForm.accountStatusId !== 0"
                :color="accountForm.accountStatusId === 1 ? 'green' : 'red'"
                :text-color="accountForm.accountStatusId === 1 ? 'white' : 'black'"
                dense
                rounded
                class="text-weight-medium q-px-sm"
              >
                {{ accountsStore.accountStatuses.find((status) => status.id === accountForm.accountStatusId)?.description || 'Status unavailable' }}
              </q-chip>
            </div>
          </q-card-section>
          <q-separator inset></q-separator>
          <q-card-section>
            <q-form @submit.prevent="saveAccountDetails" class="q-gutter-md">
              <q-input outlined v-model="accountForm.code" label="Code" readonly />
              <q-input outlined v-model="accountForm.personCode" label="Person Code" readonly />
              <q-input
                outlined
                v-model="accountForm.accountNumber"
                hint="Enter a valid account number."
                clearable
                label="Account Number"
                :rules="[isRequired, isNotMoreThanMaxLength(50)]"
              />
              <q-input
                v-if="accountForm.code === ''"
                outlined
                v-model="accountForm.outstandingBalance"
                hint="Enter a valid monetary value."
                clearable
                label="Outstanding Balance"
                :rules="[isRequired, isMonetaryValue]"
                :readonly="accountForm.code !== ''"
              />
              <q-input
                v-if="accountForm.code !== ''"
                outlined
                v-model="accountForm.outstandingBalance"
                hint="Enter a valid monetary value."
                clearable
                label="Outstanding Balance"
                :readonly="true"
              />
              <div class="q-mt-md items-center">
                <q-btn class="q-ml-sm" type="submit" color="primary" :label="accountId ? 'Save Account' : 'Create Account'" />
                <q-btn
                  v-if="accountId && accountForm.accountStatusId === 1 && accountForm.outstandingBalance === 0"
                  :color="accountForm.accountStatusId !== 1 ? 'positive' : 'negative'"
                  :label="'Close Account'"
                  @click="closeAccount"
                  class="q-ml-sm"
                />
                <q-btn
                  v-if="accountId && accountForm.accountStatusId === 2"
                  :color="accountForm.accountStatusId !== 1 ? 'positive' : 'negative'"
                  :label="'Re-open Account'"
                  @click="reopenAccount"
                  class="q-ml-sm"
                />
                <q-btn flat class="q-ml-sm" @click="navigateToPersonDetails(accountForm.personCode)">Back</q-btn>
              </div>
            </q-form>
          </q-card-section>
        </q-card>
      </div>

      <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
        <q-card flat bordered>
          <q-toolbar>
            <q-toolbar-title>Transactions</q-toolbar-title>
            <q-space />
            <q-btn
              v-if="accountForm.code"
              outline
              color="primary"
              label="Add New Transaction"
              @click="openTransactionDialog()"
            />
          </q-toolbar>
          <q-separator inset></q-separator>
          <q-card-section>
            <q-table
              v-if="accountId && transactions.length > 0"
              :rows="transactions"
              :columns="transactionColumns"
              row-key="code"
              :pagination="{ rowsPerPage: 10, page: 1 }"
              :loading="loading"
            >
              <template v-slot:body-cell-actions="props">
                <q-btn
                  class="q-mr-xs"
                  color="primary"
                  flat
                  icon="more_vert"
                  @click="editTransaction(props.row)"
                >
                  <q-tooltip>View/Edit</q-tooltip>
                </q-btn>
              </template>
            </q-table>
            <div v-else-if="accountId">No transactions found for this account.</div>
            <div v-else>No transactions will be displayed until the account is created.</div>
          </q-card-section>
        </q-card>
      </div>
    </div>

    <q-dialog v-model="transactionDialog">
      <TransactionDialog
        v-model:show="transactionDialog"
        :transaction="transactionForm"
        @save="handleSaveTransaction"
        @update:modelValue="transactionDialog = $event"
      />
    </q-dialog>
  </q-page>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useQuasar } from 'quasar';
import { useAccountsStore } from 'src/stores/accountsStore';
import { useTransactionsStore } from 'src/stores/transactionsStore';
import TransactionDialog from 'src/components/TransactionDialog.vue';
import { isRequired, isNotMoreThanMaxLength, isMonetaryValue } from 'src/utils/validationRules';

const $q = useQuasar();

const route = useRoute();
const router = useRouter();
const accountsStore = useAccountsStore();
const transactionsStore = useTransactionsStore();
const accountId = ref(route.params.accountId);
const personId = ref(route.params.personId);

const loading = ref(false);
const accountForm = ref({
  code: '',
  personCode: '',
  accountNumber: '',
  outstandingBalance: '',
  accountStatusId: 0,
});

const transactions = ref([]);
const transactionColumns = ref([
  { name: 'code', label: 'Code', field: 'code', sortable: true },
  { name: 'accountCode', label: 'Account Code', field: 'accountCode', sortable: true },
  { name: 'transactionDate', label: 'Transaction Date', field: 'transactionDate', sortable: true },
  { name: 'captureDate', label: 'Capture Date', field: 'captureDate', sortable: true },
  { name: 'amount', label: 'Amount', field: 'amount', sortable: true, format: (val) => parseFloat(val).toFixed(2) },
  { name: 'description', label: 'Description', field: 'description', sortable: true },
  { name: 'actions', label: 'Actions', field: 'actions', align: 'right' },
]);

const transactionDialog = ref(false);
const transactionForm = ref({
  code: '',
  accountCode: '',
  transactionDate: '',
  captureDate: '',
  amount: null,
  description: '',
  accountStatus: null,
});

const fetchAccountByCode = async (id) => {
  loading.value = true;
  try {
    const account = await accountsStore.getAccountByCode(id);
    if (account) {
      accountForm.value = {
        code: account.code,
        personCode: account.personCode,
        accountNumber: account.accountNumber,
        outstandingBalance: account.outstandingBalance,
        accountStatusId: account.accountStatusId,
      };
      transactions.value = account.transactions.map((transaction) => {
        return {
          ...transaction,
          transactionDate: formatDate(new Date(transaction.transactionDate)),
          captureDate: formatDate(new Date(transaction.captureDate)),
        };
      }) || [];
    } else {
      $q.notify({
        type: 'error',
        message: 'Account not found',
      });
      router.push({ name: 'accounts' });
    }
  } catch (error) {
    console.error('Error fetching account details:', error);
    $q.notify({
      type: 'error',
      message: 'Failed to load account details',
    });
  } finally {
    loading.value = false;
  }
};

const formatDate = (date) => {
  return date.toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  }).replace(/\//g, '-');
};

onMounted(() => {
  if (accountId.value) {
    fetchAccountByCode(accountId.value);
    accountsStore.getAccountStatuses().then((statuses) => {
      accountsStore.accountStatuses = statuses
    })
  }
  if (personId.value) {
    accountForm.value = {
      ...accountForm.value,
      personCode: personId.value,
    }
  }
});

const saveAccountDetails = async () => {
  loading.value = true;
  try {
    // if the account status is 0, it's a new account, so we create it with an 'Open' status.
    if (accountForm.value.accountStatusId === 0) {
      accountForm.value.accountStatusId = 1;
    }
    await accountsStore.saveAccount(accountForm.value);
    $q.notify({ type: 'positive', message: 'Account details updated successfully!' });
    router.push({ name: 'person_details', params: { personId: accountForm.value.personCode } })
  } catch (error) {
    console.error('Error saving account details:', error);
    $q.notify({
      type: 'error',
      message: 'Failed to save account details',
    });
  } finally {
    loading.value = false;
  }
};

const closeAccount = async () => {
  $q.dialog({
    title: 'Close Account',
    message: `Are you sure you want to close account number ${accountForm.value.accountNumber}?`,
    cancel: true,
    persistent: true,
  }).onOk(() => {
    loading.value = true
    try {
      accountForm.value.accountStatusId = 2;
      $q.notify({ type: 'positive', message: 'Account closed successfully!' });
      saveAccountDetails()
    } catch (error) {
      console.error('Error closing account:', error);
      $q.notify({
        type: 'error',
        message: error,
      });
    } finally {
      loading.value = false;
    }
  })
};

const reopenAccount = async () => {
  $q.dialog({
    title: 'Re-open Account',
    message: `Are you sure you want to re-open account number ${accountForm.value.accountNumber}?`,
    cancel: true,
    persistent: true,
  }).onOk(() => {
    loading.value = true
    try {
      accountForm.value.accountStatusId = 1;
      $q.notify({ type: 'positive', message: 'Account re-opened successfully!' });
      saveAccountDetails()
    } catch (error) {
      console.error('Error re-opening account:', error);
      $q.notify({
        type: 'error',
        message: error,
      });
    } finally {
      loading.value = false;
    }
  })
};

function openTransactionDialog() {
  transactionForm.value = {
    code: '',
    accountCode: accountId,
    transactionDate: '',
    captureDate: null,
    amount: null,
    description: '',
  };
  transactionDialog.value = true
}

function editTransaction(transaction) {
  transactionForm.value = { ...transaction };
  transactionDialog.value = true
}

async function handleSaveTransaction(transaction) {
  loading.value = true;
  try {
    let transactionModel = transaction;
    delete transactionModel.captureDate;
    await transactionsStore.saveTransaction(transactionModel);
    fetchAccountByCode(accountId.value);
    $q.notify({ type: 'positive', message: 'Transaction updated' });
  } catch (error) {
    console.error('Error saving transaction', error);
    $q.notify({ type: 'error', message: 'Failed to save transaction' });
  }
  finally {
    transactionDialog.value = false;
    loading.value = false;
  }
}

function navigateToPersonDetails(personCode) {
  router.push({ name: 'person_details', params: { personId: personCode } })
}
</script>

<style scoped>
</style>
