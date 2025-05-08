<template>
  <q-card flat bordered :style="$q.screen.gt.sm ? 'width: 60%;' : 'width: 90%;'">
    <q-card-section>
      <div class="text-h6">{{ transaction.code ? 'Edit Transaction' : 'Add New Transaction' }}</div>
    </q-card-section>
    <q-separator />
    <q-form @submit.prevent="saveTransaction" class="q-gutter-md" ref="transactionForm">
      <q-card-section>
        <q-input
          outlined
          v-model="formData.code"
          label="Code"
          readonly
          hint=""
        />
        <q-input
          outlined
          v-model="formData.accountCode"
          label="Account Code"
          readonly
          hint=""
        />
        <q-input
          outlined
          v-model="formData.captureDate"
          label="Capture Date"
          type="text"
          hint=""
          readonly
        />
        <q-input
          outlined
          v-model="formData.transactionDate"
          label="Transaction Date"
          type="text"
          :rules="[isRequired, isNotFutureDate]"
          clearable
        >
          <template v-slot:append>
            <q-icon name="event" class="cursor-pointer">
              <q-popup-proxy cover transition-show="scale" transition-hide="scale">
                <q-date
                  v-model="formData.transactionDate"
                  :options="disableFutureDates"
                  mask="MM/DD/YYYY"
                >
                  <div class="row items-center justify-end">
                    <q-btn v-close-popup label="Close" color="primary" flat />
                  </div>
                </q-date>
              </q-popup-proxy>
            </q-icon>
          </template>
        </q-input>
        <q-input
          outlined
          v-model="formData.amount"
          label="Amount"
          :rules="[isRequired, isMonetaryValue]"
          clearable
        />
        <q-input
          outlined
          v-model="formData.description"
          label="Description"
          :rules="[isRequired, isNotMoreThanMaxLength(100)]"
          clearable
        />
      </q-card-section>
      <q-card-actions align="right">
        <q-btn label="Cancel" @click="closeDialog"></q-btn>
        <q-btn type="submit" color="primary">
          {{ transaction.code ? 'Update' : 'Save' }}
        </q-btn>
      </q-card-actions>
    </q-form>
  </q-card>
</template>

<script>
import { isRequired, isNotFutureDate, isMonetaryValue, isNotMoreThanMaxLength } from 'src/utils/validationRules';

export default {
  props: {
    modelValue: Boolean,
    transaction: Object,
    title: String
  },
  data() {
    return {
      visible: this.modelValue,
      formData: { ...this.transaction },
      isRequired: isRequired,
      isMonetaryValue: isMonetaryValue,
      isNotFutureDate: isNotFutureDate,
      isNotMoreThanMaxLength: isNotMoreThanMaxLength,
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
      this.$emit("update:modelValue", false);
    },
    saveTransaction() {
      this.$emit('save', this.formData);
      this.closeDialog();
    },
    disableFutureDates(dateString) {
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      const currentDateInPicker = new Date(dateString);
      return currentDateInPicker <= today;
    },
  }
};
</script>

<style scoped>
</style>
