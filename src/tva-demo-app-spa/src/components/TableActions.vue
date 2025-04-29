<template>
  <q-btn outline color="primary" label="Add New" @click="onAdd" />

  <q-input
    outlined
    dense
    debounce="300"
    v-model="filter"
    placeholder="Search"
    class="q-ml-sm"
  >
    <template v-slot:append>
      <q-icon name="search" />
    </template>
  </q-input>

  <q-btn
    flat
    round
    dense
    :icon="isFullscreen ? 'fullscreen_exit' : 'fullscreen'"
    @click="toggleFullscreen"
    v-if="showFullscreen"
  >
    <q-tooltip :disable="$q.platform.is.mobile">
      {{ isFullscreen ? 'Exit Fullscreen' : 'Toggle Fullscreen' }}
    </q-tooltip>
  </q-btn>

  <q-btn
    flat
    round
    dense
    :icon="mode === 'grid' ? 'list' : 'grid_on'"
    @click="toggleMode"
    v-if="showLayout"
  >
    <q-tooltip :disable="$q.platform.is.mobile">
      {{ mode === 'grid' ? 'List' : 'Grid' }}
    </q-tooltip>
  </q-btn>

  <q-btn
    color="primary"
    icon-right="archive"
    label="Export to csv"
    no-caps
    @click="exportTable"
    v-if="showExport"
  />
</template>

<script setup>
import { ref, getCurrentInstance, watch } from 'vue'
import { exportFile } from 'quasar'

const props = defineProps({
  showAdd: {
    type: Boolean,
    default: true,
  },
  showSearch: {
    type: Boolean,
    default: true,
  },
  showFullscreen: {
    type: Boolean,
    default: true,
  },
  showLayout: {
    type: Boolean,
    default: true,
  },
  showExport: {
    type: Boolean,
    default: true,
  },
  onAdd: {
    type: Function,
  },
  onSearch: {
    type: Function,
  },
  onFullscreenToggle: {
    type: Function,
  },
  onLayoutToggle: {
    type: Function,
  },
  onExport: {
    type: Function,
  },
  columns: {
    type: Array,
    required: true,
  },
  tableData: {
    type: Array,
    required: true,
  },
})

const emit = defineEmits([
  'update:mode',
  'update:isFullscreen',
  'update:filter',
  'add',
  'export',
  'layout-toggle',
])

const filter = ref('')
const mode = ref('list')
const isFullscreen = ref(false)

const onAdd = () => {
  emit('add')
  if (props.onAdd) {
    props.onAdd()
  }
}

const toggleFullscreen = () => {
  const newIsFullscreen = !isFullscreen.value
  isFullscreen.value = newIsFullscreen
  emit('update:isFullscreen', newIsFullscreen)
  emit('fullscreen-toggle', newIsFullscreen)
  if (props.onFullscreenToggle) {
    props.onFullscreenToggle(newIsFullscreen)
  }
}

const toggleMode = () => {
  const newMode = mode.value === 'grid' ? 'list' : 'grid'
  mode.value = newMode
  emit('update:mode', newMode)
  emit('layout-toggle', newMode);
  if (props.onLayoutToggle) {
    props.onLayoutToggle(newMode)
  }
}

watch(filter, (newFilter) => {
  emit('update:filter', newFilter);
  if (props.onSearch) {
    props.onSearch(newFilter)
  }
})

const exportTable = () => {
  if (props.onExport) {
    props.onExport()
  } else {
    const content = [props.columns.map((col) => wrapCsvValue(col.label))]
      .concat(
        props.tableData.map((row) =>
          props.columns
            .map((col) =>
              wrapCsvValue(
                typeof col.field === 'function'
                  ? col.field(row)
                  : (() => {
                      const fieldName = col.field === void 0 ? col.name : col.field
                      return row[fieldName]
                    })(),
                col.format,
              ),
            )
            .join(','),
        ),
      )
      .join('\r\n')

    const status = exportFile('data.csv', content, 'text/csv')

    if (status !== true) {
      getCurrentInstance().notify({
        message: 'Browser denied file download...',
        color: 'negative',
        icon: 'warning',
      })
    }
  }
}

function wrapCsvValue(val, formatFn) {
  let formatted = formatFn !== void 0 ? formatFn(val) : val
  formatted = formatted === void 0 || formatted === null ? '' : String(formatted)
  formatted = formatted.split('"').join('""')
  return `"${formatted}"`
}

defineExpose({
  mode,
  isFullscreen,
})
</script>
