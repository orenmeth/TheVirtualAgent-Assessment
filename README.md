# TVA Demo Application - Full Stack

## Overview

This project is a full-stack application consisting of a **Vue.js frontend**, a **.NET Core Web API backend**, and a **SQL Server database**. The API serves as a logic layer, managing "Person" entities and their associated "Account" information. The Vue.js application provides the user interface for interacting with this data. Development has emphasized a clean, layered architecture and adherence to SOLID principles in the backend.

**Current Date:** May 8, 2025

## Technology Stack

* **Frontend:**
    * **Framework:** Vue.js (Version X.Y, e.g., Vue 3)
    * **State Management:** (e.g., Pinia, Vuex, or Composition API-based stores)
    * **Routing:** Vue Router
    * **HTTP Client:** (e.g., Axios, Fetch API)
    * **UI Components:** (e.g., Vuetify, PrimeVue, custom components)
* **Backend:**
    * **Framework:** ASP.NET Core (using .NET 8 or higher)
    * **Language:** C#
    * **API Design:** RESTful
    * **Caching:** In-Memory Cache (`Microsoft.Extensions.Caching.Memory.IMemoryCache`)
* **Database:**
    * **Type:** SQL Server (Version X.Y)
    * **ORM (Backend):** (Likely Entity Framework Core, given .NET context)

## 1. Vue.js Frontend Application (`TVA.Demo.App.Client` - Assumed Name)

The Vue.js application provides the user interface for managing person and account data.

### Key Responsibilities:

* Displaying lists of persons with support for pagination, sorting, and filtering by interacting with the backend API.
* Providing forms for creating new persons and editing existing ones.
* Showing detailed views of individual persons, including their associated financial accounts.
* Handling user interactions and providing feedback.

### Potential Structure & Features:

* **Components:** Organized into reusable UI components (e.g., `PersonList.vue`, `PersonForm.vue`, `PersonDetail.vue`, `AccountList.vue`).
* **Views/Pages:** For different sections of the application (e.g., Dashboard, Person Management, Person Details page).
* **Routing:** `vue-router` manages navigation between different views.
* **State Management:** A solution like Pinia or Vuex might be used for managing global application state (e.g., user authentication, shared data) or complex local component state.
* **API Interaction:** Services or helper functions (e.g., using Axios) to communicate with the backend API endpoints.
* **User Experience:** Focus on responsive design and intuitive user flows.

### Getting Started (Frontend - Placeholder):

1.  **Navigate to the client app directory:**
    ```bash
    cd path/to/vue-app-directory
    ```
2.  **Install dependencies:**
    ```bash
    npm install
    # or
    yarn install
    ```
3.  **Configure API endpoint:**
    * Ensure the Vue app is configured to point to the correct backend API URL (e.g., in a `.env` file or configuration module).
4.  **Run the development server:**
    ```bash
    npm run serve
    # or
    yarn serve
    ```
    The application will typically be available at `http://localhost:8080` (or another port configured for Vue).

*(**TODO for User:** Add specific details about your Vue app's structure, state management, key libraries, and exact commands if different.)*

## 2. SQL Server Database

The SQL Server database is the persistence layer for the application, storing information about persons and their accounts.

### Schema Overview (Inferred):

The database likely contains at least the following main tables:

* **`Persons` Table:**
    * `Code` (INT, Primary Key, Identity or sequence-generated)
    * `Name` (NVARCHAR, e.g., 100, NOT NULL)
    * `Surname` (NVARCHAR, e.g., 100, NOT NULL)
    * `Id_Number` (NVARCHAR, e.g., 13, UNIQUE, NOT NULL) - *Consider data type carefully for national ID numbers.*
    * *(Other relevant person-specific fields, e.g., `DateOfBirth`, `ContactInfo`)*
* **`Accounts` Table:**
    * `Code` (INT, Primary Key, Identity or sequence-generated)
    * `Person_Code` (INT, Foreign Key referencing `Persons.Code`, NOT NULL)
    * `Account_Number` (NVARCHAR, e.g., 50, UNIQUE, NOT NULL)
    * `Outstanding_Balance` (DECIMAL, e.g., 18,2, NOT NULL)
    * `Account_Status_Id` (INT, Foreign Key referencing an `AccountStatuses` table, NOT NULL)
    * *(Other relevant account-specific fields, e.g., `AccountType`, `DateOpened`)*
* **`AccountStatuses` Table (Lookup Table - Assumed):**
    * `Id` (INT, Primary Key)
    * `StatusName` (NVARCHAR, e.g., 50, NOT NULL) - (e.g., "Active", "Closed", "Frozen")

### Key Aspects:

* **Relationships:** Clear foreign key relationships (e.g., one-to-many from `Persons` to `Accounts`).
* **Indexing:** Appropriate indexes on foreign keys, unique constraints, and frequently queried columns (e.g., `Persons.Id_Number`, `Persons.Name`, `Persons.Surname`, `Accounts.Account_Number`) to ensure query performance.
* **Data Integrity:** Enforced through primary keys, foreign keys, NOT NULL constraints, and UNIQUE constraints.
* **Stored Procedures/Views (Optional):** May contain stored procedures for complex operations or views for simplified data retrieval, though the current design leans on the API/ORM for logic.

### Setup (Database - Placeholder):

1.  **Ensure SQL Server instance is running.**
2.  **Create the Database:** Manually create a database (e.g., `TVADemoDB`) or ensure it's created by migrations.
3.  **Connection String:** The backend API's `appsettings.json` must have the correct connection string.
4.  **Migrations (if using EF Core):**
    * If Entity Framework Core is used in the backend for data access, migrations will define and update the schema.
    * Apply migrations using the `dotnet ef database update` command from the backend project where the `DbContext` resides.

*(**TODO for User:** Add specific details about your database schema, relationships, indexing strategies, and any specific setup scripts or migration steps.)*

## 3. .NET Core Web API Backend (`TVA.Demo.App.Api`)

*(This section remains largely the same as before, as it was the primary focus of our discussion. I've slightly rephrased the intro to fit the full-stack context.)*

The .NET Core Web API serves as the central nervous system, handling business logic, data processing, and communication between the Vue.js frontend and the SQL Server database.

### Features:

* **Person Management:**
    * Retrieve a paginated, sortable, and filterable list of persons.
    * Retrieve a single person's details by their unique code.
    * Create new persons or update existing ones (Upsert).
    * Delete persons.
* **Account Information:**
    * View accounts associated with a specific person when retrieving their details.
* **Caching:**
    * Server-side caching is implemented for person lists, individual person details, and person-specific account details.
* **Layered Architecture:** API, Application (Services), Domain (Entities, Models, Repository Interfaces), Infrastructure (Repository Implementations - implied).

### SOLID Principles Adherence:
*(This subsection remains the same)*
* **Single Responsibility Principle (SRP):** ...
* **Open/Closed Principle (OCP):** ...
* **Liskov Substitution Principle (LSP):** ...
* **Interface Segregation Principle (ISP):** ...
* **Dependency Inversion Principle (DIP):** ...

### API Endpoints:
*(This subsection remains the same)*
* **Get Persons (Paginated, Filtered, Sorted)**
    * **Endpoint:** `GET /Person/GetPersons` ...
* **Get Person by Code**
    * **Endpoint:** `GET /Person/GetPerson/{code}` ...
* **Upsert Person (Create or Update)**
    * **Endpoint:** `POST /Person/UpsertPerson` ...
* **Delete Person**
    * **Endpoint:** `DELETE /Person/DeletePerson/{code}` ...

### Setup and Installation (Backend):
*(This subsection remains largely the same, just clarified it's for the backend)*
1.  **Prerequisites:** .NET SDK, SQL Server ...
2.  **Database Setup (for API connection):** Update connection string in `appsettings.json` ...
3.  **Running the Application:** Set `TVA.Demo.App.Api` as startup project ...

## General Project Setup (Full Stack - Placeholder)

1.  Clone the repository.
2.  Set up the SQL Server database (see Database section).
3.  Configure and run the Backend API (see .NET Core Web API Backend section).
4.  Configure and run the Vue.js Frontend Application (see Vue.js Frontend Application section).

## Usage (Overall Application - Placeholder)

*(This section is a placeholder.)*

* Access the Vue.js application via its local development URL (e.g., `http://localhost:8080`).
* The frontend will interact with the backend API.
* API can also be tested directly using tools like Postman or Swagger (if available at `/swagger` on the API's URL).

## Potential Future Improvements & TODOs (Full Stack)

* **Frontend:**
    * Implement comprehensive UI unit and end-to-end tests.
    * Enhance UI/UX based on user feedback.
    * Add internationalization (i18n) support.
* **Backend:**
    * Refine querying in Service/Repository for efficiency (e.g., `IQueryable` support).
    * Advanced Input Validation (e.g., FluentValidation).
    * Enhanced Error Handling (e.g., `ProblemDetails`).
    * Security: Implement robust authentication (e.g., OAuth2/OIDC) and authorization.
    * Logging: Expand structured logging.
* **Database:**
    * Performance tuning and query optimization.
    * Implement a backup and recovery strategy.
* **DevOps:**
    * Set up CI/CD pipelines for automated testing and deployment.
    * Containerize applications (Docker).

## Contribution Guidelines
*(This subsection remains the same)*

## License
*(This subsection remains the same)*

---