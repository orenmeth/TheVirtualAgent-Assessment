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
import { ref, onMounted, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { usePersonsStore } from 'src/stores/personsStore'
import TableButtons from 'src/components/TableActions.vue'

const router = useRouter()
const personsStore = usePersonsStore()
const filter = ref('')
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
  personsStore.fetchPersons(sortBy, descending, page, rowsPerPage, filter.value)
  pagination.value.page = page
  pagination.value.rowsPerPage = rowsPerPage
  pagination.value.rowsNumber = personsStore.totalItems
  pagination.value.sortBy = sortBy;
  pagination.value.descending = descending;
}

onMounted(() => {
  personsStore.fetchPersons(pagination.value.sortBy, pagination.value.descending, pagination.value.page, pagination.value.rowsPerPage, filter.value)
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
  filter.value = newFilter
  personsStore.fetchPersons(pagination.value.sortBy, pagination.value.descending, pagination.value.page, pagination.value.rowsPerPage, filter.value)
  console.log('Filter applied:', newFilter)
}
</script>

<style scoped>
.q-table__middle > tbody > tr > td:last-child {
  white-space: nowrap;
  padding-right: 12px;
}
</style>
