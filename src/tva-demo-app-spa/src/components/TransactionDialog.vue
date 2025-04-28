<template>
  <q-dialog v-model="visible" persistent>
    <q-card flat bordered :style="$q.screen.gt.sm ? 'width: 60%;' : 'width: 90%;'">
      <q-card-section>
        <div class="text-h6">{{ transaction.code ? 'Edit Transaction' : 'Add New Transaction' }}</div>
      </q-card-section>
      <q-separator />
      <q-card-section>
        <q-form class="q-gutter-md">
          <q-input outlined v-model="formData.code" label="Code" readonly />
          <q-input outlined v-model="formData.accountCode" label="Account Code" readonly />
          <q-input outlined v-model="formData.transactionDate" label="Transaction Date" type="date" />
          <q-input outlined v-model="formData.captureDate" label="Capture Date" type="date" readonly />
          <q-input outlined v-model="formData.amount" label="Amount" />
          <q-input outlined v-model="formData.description" label="Description" />
        </q-form>
      </q-card-section>
      <q-card-actions align="right">
        <q-btn label="Cancel" @click="closeDialog"></q-btn>
        <q-btn type="submit" color="primary" @click="saveTransaction">
          {{ transaction.code ? 'Update' : 'Save' }}
        </q-btn>
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script>
export default {
  props: {
    modelValue: Boolean,
    transaction: Object,
    title: String
  },
  data() {
    return {
      visible: this.modelValue,
      formData: { ...this.transaction }
    };
  },
  watch: {
    modelValue(val) {
      this.visible = val;
    },
    visible(val) {
      this.$emit('update:modelValue', val);
    }
  },
  methods: {
    closeDialog() {
      this.visible = false;
    },
    saveTransaction() {
      this.$emit('save', this.formData);
      this.closeDialog();
    }
  }
};
</script>
