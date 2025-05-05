<template>
  <q-page padding>
    <div class="q-mx-auto">

      <q-card class="q-mb-md">
        <q-card-section>
          <div class="text-h4 q-mb-md">About PAM</div>
          <p class="text-body1">
            Welcome to PAM! This application provides a streamlined and efficient way
            to manage your customer information, track their accounts, and record associated
            transactions. Built for simplicity and ease of use, it helps you keep your essential
            customer data organized and accessible.
          </p>
        </q-card-section>
        <q-separator spaced />
        <q-card-section>
          <p class="text-body1">
            Please visit the <a href="https://github.com/orenmeth/TheVirtualAgent-Assessment/blob/main/README.md" target="_blank" rel="noopener noreferrer">README</a>
            page to find out more about how to build, test and run this application.
          </p>
        </q-card-section>
      </q-card>

      <q-card class="q-mb-md">
        <q-card-section>
          <div class="text-h5 q-mb-sm">Key Features</div>
        </q-card-section>

        <q-list bordered separator>
          <q-item>
            <q-item-section avatar>
              <q-icon color="primary" name="people" />
            </q-item-section>
            <q-item-section>
              <q-item-label>Customer Management</q-item-label>
              <q-item-label caption>
                Easily create, view, update, and delete customer profiles. Keep all essential
                customer information in one place.
              </q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section avatar>
              <q-icon color="primary" name="account_balance_wallet" />
            </q-item-section>
            <q-item-section>
              <q-item-label>Account Tracking</q-item-label>
              <q-item-label caption>
                Manage multiple accounts associated with each customer. Track accounts,
                balances and statuses effortlessly.
              </q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section avatar>
              <q-icon color="primary" name="receipt_long" />
            </q-item-section>
            <q-item-section>
              <q-item-label>Transaction Recording</q-item-label>
              <q-item-label caption>
                Log transactions linked to specific customer accounts. Record details like date,
                amount and description.
              </q-item-label>
            </q-item-section>
          </q-item>

           <q-item>
            <q-item-section avatar>
              <q-icon color="primary" name="search" />
            </q-item-section>
            <q-item-section>
              <q-item-label>Search & Filtering</q-item-label>
              <q-item-label caption>
                Quickly find the information you need with built-in search and filtering capabilities.
              </q-item-label>
            </q-item-section>
          </q-item>
        </q-list>
      </q-card>

      <q-card class="q-mb-md">
        <q-card-section>
          <div class="text-h5 q-mb-sm">Technology Stack</div>
          <p>
            This application is built using modern web technologies:
          </p>
          <ul>
            <li>
              <a href="https://vuejs.org/" target="_blank" rel="noopener noreferrer">VueJS</a>:
              A progressive JavaScript framework for building user interfaces.
            </li>
            <li>
              <a href="https://quasar.dev/" target="_blank" rel="noopener noreferrer">Quasar Framework</a>:
              A high-performance VueJS framework for building responsive websites, PWAs, SSR apps,
              mobile apps, and desktop apps from a single codebase.
            </li>
            <li>
              <a href="https://pinia.vuejs.org/" target="_blank" rel="noopener noreferrer">Pinia</a>:
              A state management library for VueJS applications, providing a simple and intuitive API.
            </li>
            <li>
              <a href="https://dotnet.microsoft.com/en-us/platform/why-choose-dotnet" target="_blank" rel="noopener noreferrer">.NET</a>:
              .NET is a secure, reliable, and high-performance application platform.
            </li>
            <li>
              <a href="https://github.com/DapperLib/Dapper" target="_blank" rel="noopener noreferrer">Dapper</a>:
              A simple object mapper for .NET, providing a fast and efficient way to work with databases.
            </li>
            <li>
              <a href="https://www.microsoft.com/en-za/sql-server/sql-server-2022" target="_blank" rel="noopener noreferrer">Microsoft SQL Server</a>:
              A relational database management system developed by Microsoft, designed for enterprise.
            </li>
            </ul>
        </q-card-section>
      </q-card>

      <q-card class="q-mb-md">
        <q-card-section>
          <div class="text-h5 q-mb-sm">Version & Contact</div>
          <p>
            <strong>Application Version:</strong> {{ appVersion }}
          </p>
          <p>
            Need help or have suggestions? We'd love to hear from you!
          </p>
          <p>
            Contact us at: <a href="mailto:[orenmeth@gmail.com]">orenmeth@gmail.com</a>

            </p>
        </q-card-section>
      </q-card>

      </div>
  </q-page>
</template>

<script setup>
import { ref } from 'vue';
const appVersion = ref(import.meta.env.VITE_APP_VERSION || '1.0.0');
</script>

<style scoped>
a {
  color: var(--q-primary);
  text-decoration: none;
}

a:hover {
  text-decoration: underline;
}

ul {
  padding-left: 20px;
}

li {
  margin-bottom: 8px;
}
</style>
