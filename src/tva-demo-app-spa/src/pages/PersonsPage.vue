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
      >
        <template v-slot:top-right="props">
          <q-btn
            outline
            color="primary"
            label="Add New Person"
            @click="navigateToPersonDetails(null)"
          />
          <q-input outlined dense debounce="300" v-model="filter" placeholder="Search">
            <template v-slot:append>
              <q-icon name="search" />
            </template>
          </q-input>

          <q-btn
            flat
            round
            dense
            :icon="props.inFullscreen ? 'fullscreen_exit' : 'fullscreen'"
            @click="props.toggleFullscreen"
            v-if="mode === 'list'"
          >
            <q-tooltip :disable="$q.platform.is.mobile" v-close-popup
              >{{ props.inFullscreen ? 'Exit Fullscreen' : 'Toggle Fullscreen' }}
            </q-tooltip>
          </q-btn>

          <q-btn
            flat
            round
            dense
            :icon="mode === 'grid' ? 'list' : 'grid_on'"
            @click="mode = mode === 'grid' ? 'list' : 'grid'; separator= mode === 'grid' ? 'none' : 'horizontal'"
            v-if="!props.inFullscreen"
          >
            <q-tooltip :disable="$q.platform.is.mobile" v-close-popup
              >{{ mode === 'grid' ? 'List' : 'Grid' }}
            </q-tooltip>
          </q-btn>

          <q-btn
            color="primary"
            icon-right="archive"
            label="Export to csv"
            no-caps
            @click="exportTable"
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
import { ref, onMounted, onActivated, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { usePersonsStore } from 'src/stores/personsStore'
import { exportFile } from 'quasar'

const router = useRouter()
const personsStore = usePersonsStore()

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
  const { page, rowsPerPage } = props.pagination
  personsStore.fetchPersons(page, rowsPerPage)
  pagination.value.page = page
  pagination.value.rowsPerPage = rowsPerPage
  pagination.value.rowsNumber = personsStore.totalItems
}

onMounted(() => {
  personsStore.fetchPersons(pagination.value.page, pagination.value.rowsPerPage)
  pagination.value.rowsNumber = personsStore.totalItems
})

onActivated(() => {
  personsStore.fetchPersons(pagination.value.page, pagination.value.rowsPerPage)
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

function wrapCsvValue(val, formatFn) {
  let formatted = formatFn !== void 0 ? formatFn(val) : val
  formatted = formatted === void 0 || formatted === null ? '' : String(formatted)
  formatted = formatted.split('"').join('""')
  return `"${formatted}"`
}

function exportTable() {
  const content = [this.columns.map((col) => wrapCsvValue(col.label))]
    .concat(
      this.data.map((row) =>
        this.columns
          .map((col) =>
            wrapCsvValue(
              typeof col.field === 'function'
                ? col.field(row)
                : (() => {
                    const fieldName = col.field === void 0 ? col.name : col.field;
                    return row[fieldName];
                  })(),
              col.format,
            ),
          )
          .join(','),
      ),
    )
    .join('\r\n')

  const status = exportFile('customer-management.csv', content, 'text/csv')

  if (status !== true) {
    this.$q.notify({
      message: 'Browser denied file download...',
      color: 'negative',
      icon: 'warning',
    })
  }
}
</script>

<style scoped>
.q-table__middle > tbody > tr > td:last-child {
  white-space: nowrap;
  padding-right: 12px;
}
</style>
