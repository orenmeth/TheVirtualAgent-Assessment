# TVA Demo Application - Full Stack

## Overview

This project is a full-stack application consisting of a **Vue.js frontend**, a **.NET Core Web API backend**, and a **SQL Server database**. The API serves as a logic layer, managing "Person" entities and their associated "Account" information. The Vue.js application provides the user interface for interacting with this data. Development has emphasized a clean, layered architecture and adherence to SOLID principles in the backend.

**Current Date:** May 8, 2025

## Technology Stack

* **Frontend:**
    * **Framework:** Vue.js 3 & Quasar 2
    * **State Management:** Pinia, Vuex, or Composition API-based stores
    * **Routing:** Vue Router
    * **HTTP Client:** Axios
    * **UI Components:** Quasar Library
* **Backend:**
    * **Framework:** ASP.NET Core (.NET 8)
    * **Language:** C#
    * **API Design:** RESTful
    * **Caching:** In-Memory Cache (`Microsoft.Extensions.Caching.Memory.IMemoryCache`)
* **Database:**
    * **Type:** SQL Server (2022)
    * **ORM (Backend):** (Dapper)

## 1. Vue.js Frontend Application (`TVA.Demo.App.Client` - Assumed Name)

The Vue.js application provides the user interface for managing person and account data.

### Key Responsibilities:

* Displaying lists of persons with support for pagination, sorting, and filtering by interacting with the backend API.
* Providing forms for creating new persons and editing existing ones.
* Showing detailed views of individual persons, including their associated financial accounts.
* Handling user interactions and providing feedback.

### Potential Structure & Features:

* **Components:** Organized into reusable UI components.
* **Views/Pages:** For different sections of the application.
* **Routing:** `vue-router` manages navigation between different views.
* **State Management:** A solution like Pinia or Vuex might be used for managing global application state or complex local component state.
* **API Interaction:** Services or helper functions to communicate with the backend API endpoints.
* **User Experience:** Focus on responsive design and intuitive user flows.

### Getting Started (Frontend - Placeholder):

1.  **Navigate to the client app directory:**
    ```bash
    cd path/to/thevirtualagent/src/tva-demo-app-spa
    ```
2.  **Install dependencies:**
    ```bash
    npm install
    npm i -g @quasar/cli
    ```
3.  **Configure API endpoint:**
    * Ensure the Vue app is configured to point to the correct backend API URL (e.g., in a `.env` file or configuration module).
4.  **Run the development server:**
    ```bash
    quasar dev
    ```
    The application will typically be available at `http://localhost:9000`.

## 2. SQL Server Database

The SQL Server database is the persistence layer for the application, storing information about persons and their accounts.

### Schema Overview (Inferred):

The database likely contains at least the following main tables:

* **`Persons` Table:**
    * `code` (INT, Primary Key, Identity)
    * `name` (VARCHAR, 50, NULL)
    * `surname` (VARCHAR, 50, NULL)
    * `id_number` (VARCHAR, 50, UNIQUE, NOT NULL)
* **`Accounts` Table:**
    * `code` (INT, Primary Key, Identity)
    * `person_code` (INT, Foreign Key referencing `Persons.Code`, NOT NULL)
    * `account_number` (VARCHAR, 50, UNIQUE, NOT NULL)
    * `outstanding_balance` (MONEY, NOT NULL)
    * `account_status_id` (INT, Foreign Key referencing an `AccountStatuses` table, NOT NULL)
* **`AccountStatuses` Table (Lookup Table - Assumed):**
    * `Id` (INT, Primary Key)
    * `Description` (NVARCHAR,  50, NOT NULL)
* **`Transactions` Table:**
    * `Code` (INT, Primary Key, Identity)
    * `Account_Code` (INT, Foreign Key referencing `Accounts.Code`, NOT NULL)
    * `transaction_date` (DATETIME, NOT NULL)
    * `capture_date` (DATETIME, NOT NULL)
    * `amount` (MONEY, NOT NULL)
    * `description` (VARCHAR, 100, NOT NULL)

### Key Aspects:

* **Relationships:** Clear foreign key relationships.
* **Indexing:** Appropriate indexes on foreign keys, unique constraints, and frequently queried columns.
* **Data Integrity:** Enforced through primary keys, foreign keys, NOT NULL constraints, and UNIQUE constraints.
* **Stored Procedures:** May contain stored procedures for complex operations.

### Setup:

#### Database

1.  **Ensure SQL Server instance is running.**
2.  **Create the Database:** Publish the database project.
3.  **Connection String:** The backend API's `appsettings.json` must have the correct connection string.

## 3. .NET Core Web API Backend (`TVA.Demo.App.Api`)

The .NET Core Web API serves as the central nervous system, handling business logic, data processing, and communication between the Vue.js frontend and the SQL Server database.

### Features:

* **Person Management:**
    * Retrieve a paginated, sortable, and filterable list of persons.
    * Retrieve a single person's details by their unique code.
    * Create new persons or update existing ones (Upsert).
    * Delete persons.
* **Account Information:**
    * View, edit, close and re-open accounts associated with a specific person when retrieving their details.
* **Transaction Information:**
    * Add and edit transactions on an account.
* **Caching:**
    * Server-side caching is implemented.
* **Layered Architecture:** API, Application (Services), Domain (Entities, Models Interfaces), Infrastructure (Repository).

### SOLID Principles Adherence:
* **Single Responsibility Principle (SRP):** ...
* **Open/Closed Principle (OCP):** ...
* **Liskov Substitution Principle (LSP):** ...
* **Interface Segregation Principle (ISP):** ...
* **Dependency Inversion Principle (DIP):** ...

### API Endpoints:
* **Get Persons (Paginated, Filtered, Sorted)**
    * **Endpoint:** `GET /Person/GetPersons` ...
* **Get Person by Code**
    * **Endpoint:** `GET /Person/GetPerson/{code}` ...
* **Upsert Person (Create or Update)**
    * **Endpoint:** `POST /Person/UpsertPerson` ...
* **Delete Person**
    * **Endpoint:** `DELETE /Person/DeletePerson/{code}` ...

### Setup and Installation (Backend):
1.  **Prerequisites:** .NET SDK, SQL Server ...
2.  **Database Setup (for API connection):** Update connection string in `appsettings.json` ...
3.  **Running the Application:** Set `TVA.Demo.App.Api` as startup project ...

## General Project Setup (Full Stack - Placeholder)

1.  Clone the repository.
2.  Set up the SQL Server database (see Database section).
3.  Configure and run the Backend API (see .NET Core Web API Backend section).
4.  Configure and run the Vue.js Frontend Application (see Vue.js Frontend Application section).

## Usage (Overall Application - Placeholder)

* Access the Vue.js application via its local development URL (e.g., `http://localhost:9000`).
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

## License

---