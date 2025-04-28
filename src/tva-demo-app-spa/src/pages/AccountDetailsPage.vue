<template>
  <q-page class="q-pa-md">
    <div class="row q-col-gutter-sm q-ma-xs q-mr-sm">
      <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
        <q-card flat bordered>
          <q-card-section>
            <div class="text-h6">Account Details</div>
          </q-card-section>
          <q-separator inset></q-separator>
          <q-card-section>
            <q-form @submit.prevent="saveAccountDetails" class="q-gutter-md">
              <q-input outlined v-model="accountForm.code" label="Code" readonly />
              <q-input outlined v-model="accountForm.personCode" label="Person Code" readonly />
              <q-input outlined v-model="accountForm.accountNumber" label="Account Number" />
              <q-input outlined v-model="accountForm.outstandingBalance" label="Outstanding Balance" />
              <div class="q-mt-md">
                <q-btn type="submit" color="primary" :label="accountId ? 'Save Changes' : 'Create Account'" />
                <q-btn flat class="q-ml-sm" @click="$router.go(-1)">Cancel</q-btn>
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

    <TransactionDialog
      v-model:show="transactionDialog"
      :transaction="transactionForm"
      @save="handleSaveTransaction"
    />
  </q-page>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useQuasar } from 'quasar';
import { useAccountsStore } from 'src/stores/accountsStore';
import { useTransactionsStore } from 'src/stores/transactionsStore';
import TransactionDialog from 'src/components/TransactionDialog.vue';

const $q = useQuasar();

const route = useRoute();
const router = useRouter();
const accountsStore = useAccountsStore();
const transactionsStore = useTransactionsStore();
const accountId = ref(route.params.accountId);

const loading = ref(false);
const accountForm = ref({
  code: '',
  personCode: '',
  accountNumber: '',
  outstandingBalance: '',
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
});

const fetchAccountByCode = async (id) => {
  loading.value = true;
  try {
    const account = await accountsStore.fetchAccountByCode(id);
    if (account) {
      accountForm.value = {
        code: account.code,
        personCode: account.personCode,
        accountNumber: account.accountNumber,
        outstandingBalance: account.outstandingBalance,
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
  }
});

watch(() => route.params.accountId, () => {
  if (route.params.accountId) {
    fetchAccountByCode(route.params.accountId);
  }
});

const saveAccountDetails = async () => {
  loading.value = true;
  try {
    const payload = {
      ...accountForm.value,
      openingDate: accountForm.value.openingDate ? accountForm.value.openingDate.toISOString() : null,
    };
    // Call store action to update account details
    await accountsStore.updateAccountDetails(accountId.value, payload);
    $q.notify({ type: 'positive', message: 'Account details updated successfully!' });
    router.push({ name: 'accounts' });
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

function openTransactionDialog() {
  transactionForm.value = {
    code: '',
    accountCode: accountId,
    transactionDate: '',
    captureDate: null,
    amount: null,
    description: '',
  };

  this.$q.dialog({
    title: 'Add New Transaction',
    message: 'Enter the transaction details below.',
    component: TransactionDialog,
    componentProps: {
      modelValue: true,
      transaction: transactionForm.value,
    },
    persistent: true,
    onDismiss: () => {
      console.log('Dialog dismissed');
    },
    onOk: async () => {
      console.log('Dialog OK, data:');
      await handleSaveTransaction();
    },
  });
}

function editTransaction(transaction) {
  transactionForm.value = { ...transaction };

  this.$q.dialog({
    title: 'Edit Transaction',
    message: 'Edit the transaction details below.',
    component: TransactionDialog,
    componentProps: {
      modelValue: true,
      transaction: transactionForm.value,
    },
    onCancel: () => {
      console.log('Dialog dismissed');
    },
    persistent: true,
    onOk: async () => {
      console.log('Dialog OK, data:', transactionForm.value);
      await this.handleSaveTransaction();
    },
  });
}

async function handleSaveTransaction(formData) {
  alert('Saving transaction...', formData);
  try {
    if (formData.code) {
      await transactionsStore.updateTransaction(accountId.value, formData.code, formData);
      $q.notify({ type: 'positive', message: 'Transaction updated' });
    } else {
      await transactionsStore.addTransaction(accountId.value, formData);
      $q.notify({ type: 'positive', message: 'Transaction added' });
    }
    fetchAccountByCode(accountId.value);
  } catch (error) {
    console.error('Error saving transaction', error);
    $q.notify({ type: 'error', message: 'Failed to save transaction' });
  }
  finally {
    $q.dialog.hide();
    transactionDialog.value = false;
  }
}
</script>
