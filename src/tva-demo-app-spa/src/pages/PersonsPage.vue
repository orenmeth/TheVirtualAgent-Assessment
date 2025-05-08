<template>
  <q-page class="q-pa-sm">
    <q-card>
      <q-table
        title="Persons"
        flat-bordered
        :rows="personsStore.persons"
        :columns="columns"
        row-key="code"
        :loading="personsStore.loading"
        v-model:pagination="pagination"
        @request="onRequest"
        :filter="filter"
        :grid="mode === 'grid'"
      >
        <template v-slot:top-right>
          <TableButtons
            @update:mode="mode = $event"
            @update:fullscreen="fullscreen = $event"
            :columns="columns"
            :tableData="personsStore.persons"
            @add="handleAdd"
            @update:filter="handleFilter($event)"
          />
        </template>

        <template v-slot:body-cell-actions="props">
          <div class="flex flex-center full-width full-height">
            <q-btn
              color="primary"
              flat dense round
              icon="edit"
              @click="navigateToPersonDetails(props.row)"
            >
              <q-tooltip>View/Edit</q-tooltip>
            </q-btn>
          </div>
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
import { ref, onMounted, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { usePersonsStore } from 'src/stores/personsStore'
import TableButtons from 'src/components/TableActions.vue'

const router = useRouter()
const personsStore = usePersonsStore()
const filter = ref(null)
const mode = ref('list')
const fullscreen = ref(false)

const columns = ref([
  { name: 'code', required: true, label: 'Code', align: 'left', field: 'code', sortable: true },
  { name: 'name', label: 'First Name', align: 'left', field: 'name', sortable: true },
  { name: 'surname', label: 'Last Name', align: 'left', field: 'surname', sortable: true },
  { name: 'idNumber', label: 'ID Number', align: 'left', field: 'idNumber', sortable: true },
  { name: 'actions', label: '', align: 'right' },
])

const pagination = ref({
  sortBy: 'code',
  descending: false,
  page: 1,
  rowsPerPage: 10,
  rowsNumber: 0,
})

const onRequest = (props) => {
  const { page, rowsPerPage, sortBy, descending } = props.pagination
  personsStore.getPersons(sortBy, filter.value, descending, page, rowsPerPage)
  pagination.value.page = page
  pagination.value.rowsPerPage = rowsPerPage
  pagination.value.rowsNumber = personsStore.totalItems
  pagination.value.sortBy = sortBy;
  pagination.value.descending = descending;
}

onMounted(() => {
  personsStore.getPersons(pagination.value.sortBy, filter.value, pagination.value.descending, pagination.value.page, pagination.value.rowsPerPage)
  pagination.value.rowsNumber = personsStore.totalItems
})

const totalRowsComputed = computed(() => personsStore.totalItems)
watch(totalRowsComputed, (newVal) => {
  pagination.value.rowsNumber = newVal
})

const navigateToPersonDetails = (person) => {
  if (person) {
    router.push({ name: 'person_details', params: { personId: person.code } })
  } else {
    router.push({ name: 'person_details' })
  }
}

function handleAdd() {
  router.push({ name: 'person_details' })
}

function handleFilter(newFilter) {
  if (newFilter === '' || newFilter === null) {
    filter.value = 'null'
  } else {
    filter.value = newFilter
  }
  pagination.value.page = 1;
  onRequest({ pagination: pagination.value });
}

</script>

<style scoped>
</style>
