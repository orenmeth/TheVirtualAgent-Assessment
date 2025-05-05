<template>
  <q-page padding class="flex flex-center">

    <div style="width: 100%;">

      <q-card class="q-pa-md">
        <q-card-section class="text-center">
          <q-icon name="mail_outline" size="3em" color="primary" class="q-mb-md" />
          <div class="text-h4 q-mb-sm">Get In Touch</div>
          <p class="text-body1">
            Have questions, feedback, or need support? We're here to help.
            Reach out to us using the method below.
          </p>
        </q-card-section>

        <q-separator spaced />

        <q-card-section>
          <q-list separator>

            <q-item clickable v-ripple :href="`mailto:${supportEmail}`" tag="a" target="_blank" class="q-py-md">
              <q-item-section avatar>
                <q-icon color="primary" name="email" />
              </q-item-section>
              <q-item-section>
                <q-item-label class="text-subtitle1">Email Us</q-item-label>
                <q-item-label caption>
                  {{ supportEmail }}
                </q-item-label>
              </q-item-section>
              <q-item-section side>
                 <q-icon name="launch" color="grey-7"/>
              </q-item-section>
            </q-item>

            <q-item clickable v-ripple :href="`tel:${supportPhone}`" tag="a" target="_blank" class="q-py-md">
              <q-item-section avatar>
                <q-icon color="primary" name="phone" />
              </q-item-section>
              <q-item-section>
                <q-item-label class="text-subtitle1">Call Us</q-item-label>
                <q-item-label caption>
                  {{ supportPhone }}
                </q-item-label>
              </q-item-section>
              <q-item-section side>
                 <q-icon name="launch" color="grey-7"/>
              </q-item-section>
            </q-item>

            <q-item clickable v-ripple :href="whatsappLink" tag="a" target="_blank" class="q-py-md">
              <q-item-section avatar>
                <q-icon color="primary" name="fa-brands fa-whatsapp" />
              </q-item-section>
              <q-item-section>
                <q-item-label class="text-subtitle1">Chat on WhatsApp</q-item-label>
                <q-item-label caption>
                   Click to chat with us directly on WhatsApp.
                   </q-item-label>
              </q-item-section>
              <q-item-section side>
                <q-icon name="launch" color="grey-7"/>
              </q-item-section>
            </q-item>

            <q-item clickable v-ripple :href="websiteUrl" tag="a" target="_blank" class="q-py-md">
              <q-item-section avatar>
                <q-icon color="primary" name="language" />
              </q-item-section>
              <q-item-section>
                <q-item-label class="text-subtitle1">Visit Our Website</q-item-label>
                <q-item-label caption>
                  {{ websiteUrl }}
                </q-item-label>
              </q-item-section>
              <q-item-section side>
                <q-icon name="launch" color="grey-7"/>
              </q-item-section>
            </q-item>

            </q-list>
        </q-card-section>

        <q-card-section class="text-center text-caption q-pt-lg">
          We typically respond within 24-48 business hours.
        </q-card-section>

      </q-card>
    </div>

  </q-page>
</template>

<script setup>
import { ref, computed } from 'vue';

const supportEmail = ref('orenmeth@gmail.com');
const supportPhone = ref('+27 82 946 9558');
const websiteUrl = ref('https://www.thevirtualagent.co.za/');
const whatsappNumber = ref('27829469558');

const whatsappLink = computed(() => {
  if (!whatsappNumber.value || whatsappNumber.value.startsWith('[')) {
    return '#';
  }
  const cleanedNumber = whatsappNumber.value.replace(/\D/g, '');
  return `https://wa.me/${cleanedNumber}`;
});

</script>

<style scoped lang="scss">
  .q-item--clickable {
    transition: background-color 0.3s ease;
  }
</style>
