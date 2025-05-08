<template>
  <q-page padding class="flex flex-center text-center">
    <div class="apex-chart">
      <div :id="chartId"></div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, onMounted, onUnmounted, watch, toRef } from 'vue';
import ApexCharts from 'apexcharts';

export default defineComponent({
  props: {
    options: { // ApexCharts options
      type: Object,
      required: true,
    },
    series: {  // ApexCharts series data
      type: Array,
      required: true,
    },
    chartId: { // Unique ID for the chart container
      type: String,
      default: () => `apexchart-${Math.random().toString(36).substring(7)}`,
    },
  },
  setup(props) {
    let chart = null;
    const optionsRef = toRef(props, 'options');
    const seriesRef = toRef(props, 'series');

    onMounted(() => {
      // Create the chart instance when the component is mounted
      const chartContainer = document.getElementById(props.chartId);
      if (chartContainer) {
        chart = new ApexCharts(chartContainer, {
          ...props.options,
          series: props.series,
        });
        chart.render();
      }
    });

    watch(
        () => [optionsRef.value, seriesRef.value],
        ([newOptions, newSeries]) => {
            if (chart) {
                // Update the chart when options or series change
                chart.updateOptions({
                    ...newOptions,
                    series: newSeries,
                    chart: {
                        ...newOptions.chart,
                        id: props.chartId, // Keep the chart ID
                    }
                });
            }
        },
        { deep: true }
    );

    onUnmounted(() => {
      // Destroy the chart instance when the component is unmounted
      if (chart) {
        chart.destroy();
        chart = null;
      }
    });

    return {
      chartId,
    };
  },
});
</script>

<style scoped>
.apex-chart {
  width: 100%;
}
</style>
