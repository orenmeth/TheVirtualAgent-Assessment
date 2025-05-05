<template>
  <q-page padding class="flex flex-center text-center">

    <div style="width: 100%; max-width: 700px;">

      <div class="text-h3 q-mb-md" style="font-weight: 500; line-height: 1.2em;">
        Welcome to Person Account Manager (PAM)
      </div>

      <p class="text-subtitle1 text-grey-7 q-mb-lg">
        Your simple solution for organizing customer accounts and transactions.
      </p>

      <p class="text-body1 q-mb-xl" style="max-width: 600px; margin-left: auto; margin-right: auto;">
        PAM helps you efficiently manage customer profiles, keep track of their associated accounts,
        and log all related transactions in one central place. Stay organized and access your
        information easily.
      </p>

      <q-btn
        to="/persons"
        label="Go to Dashboard"
        color="primary"
        size="lg"
        icon-right="dashboard"
        unelevated
        rounded
        class="q-px-xl"
      />

      </div>
  </q-page>
</template>

<script setup>
</script>

<style scoped>
.text-h3 {
  letter-spacing: -0.5px;
}
.text-body1 {
  line-height: 1.7;
  opacity: 0.9;
}
.q-btn {
  margin-top: 1rem;
}
</style>
