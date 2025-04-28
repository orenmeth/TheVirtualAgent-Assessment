<template>
  <q-page class="q-pa-sm">
    <q-card>
      <q-toolbar>
        <q-toolbar-title>People</q-toolbar-title>
        <q-space />
        <q-btn
          outline
          color="primary"
          label="Add New Person"
          @click="navigateToPersonDetails(null)"
        />
      </q-toolbar>

      <q-table
        :rows="personsStore.persons"
        :columns="columns"
        row-key="code"
        :loading="personsStore.loading"
        v-model:pagination="pagination"
        @request="onRequest"
      >
        <template v-slot:body-cell-actions="props">
          <q-btn
            class="q-mr-xs"
            color="primary"
            flat
            icon="more_vert"
            @click="navigateToPersonDetails(props.row)"
          >
            <q-tooltip>View/Edit</q-tooltip>
          </q-btn>
        </template>

        <template v-slot:loading>
          <q-inner-loading showing color="primary" />
        </template>
      </q-table>

      <div v-if="personsStore.error" class="text-negative q-mt-md">
        {{ personsStore.error }}
      </div>
    </q-card>
  </q-page>
</template>

<script setup>
import { ref, onMounted, onActivated, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { usePersonsStore } from 'src/stores/personsStore'

const router = useRouter();
const personsStore = usePersonsStore();

const columns = ref([
  { name: 'code', required: true, label: 'Code', align: 'left', field: 'code', sortable: true },
  { name: 'name', label: 'First Name', align: 'left', field: 'name', sortable: true },
  { name: 'surname', label: 'Last Name', align: 'left', field: 'surname', sortable: true },
  { name: 'id_number', label: 'ID Number', align: 'left', field: 'id_number', sortable: true },
  { name: 'actions', label: '', align: 'right' },
]);

const pagination = ref({
  page: 1,
  rowsPerPage: 10,
  rowsNumber: 0,
});

const onRequest = (props) => {
  const { page, rowsPerPage } = props.pagination;
  personsStore.fetchPersons(page, rowsPerPage);
  pagination.value.page = page;
  pagination.value.rowsPerPage = rowsPerPage;
  pagination.value.rowsNumber = personsStore.totalItems;
};

onMounted(() => {
  personsStore.fetchPersons(pagination.value.page, pagination.value.rowsPerPage);
});

onActivated(() => {
  personsStore.fetchPersons(pagination.value.page, pagination.value.rowsPerPage);
});

const totalRowsComputed = computed(() => personsStore.totalItems);
watch(totalRowsComputed, (newVal) => {
  pagination.value.rowsNumber = newVal;
});

const navigateToPersonDetails = (person) => {
  if (person) {
    router.push({ name: 'person_details', params: { personId: person.code } })
  } else {
    router.push({ name: 'person_details' })
  }
};
</script>

<style scoped>
.q-table__middle > tbody > tr > td:last-child {
  white-space: nowrap;
  padding-right: 12px;
}
</style>
