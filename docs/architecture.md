```mermaid
graph TD
    %% === USER INTERACTION ===
    subgraph UserLayer["User Interface Layer"]
        User["<i class='fa fa-user'></i> User / Client Browser"]
    end

    %% === FRONTEND APPLICATION ===
    subgraph Frontend["Vue.js Frontend Application"]
        direction LR
        VueApp["<i class='fab fa-vuejs'></i> Vue.js App<br/>(UI Components, Routing, State Mgt.<br/>API Client, Axios)"]
    end

    %% === BACKEND API ===
    subgraph BackendAPI[".NET Core Web API Backend"]
        %% Top to Bottom layout for internal backend components
        direction TB

        %% API Controllers (Entry Point)
        Controllers["<i class='fas fa-network-wired'></i> API Controllers<br/>(ASP.NET Core MVC - Handle HTTP Requests)"]

        %% Application Layer (Business Logic)
        subgraph ApplicationServiceLayer["Application Layer"]
            direction TB
            Services["<i class='fas fa-cogs'></i> Application Services<br/>(Business Logic, Orchestration, Mapping)"]
            SvcCache["<i class='fas fa-memory'></i> In-Memory Cache<br/>(IMemoryCache - Used by Services)"]
        end

        %% Domain Layer (Core Business Rules & Entities)
        subgraph DomainModelLayer["Domain Layer"]
            direction TB
            Entities["<i class='fas fa-boxes'></i> Domain Entities & Models<br/>(PersonDto, AccountDto, PersonRequest, PagedResult)"]
            RepoInterfaces["<i class='fas fa-file-alt'></i> Repository Interfaces<br/>(IPersonRepository, IAccountRepository)"]
        end

        %% Infrastructure Layer (Data Access, External Services)
        subgraph InfrastructureDataLayer["Infrastructure Layer"]
            direction TB
            RepoImpl["<i class='fas fa-database'></i> Repository Implementations<br/>(PersonRepository, AccountRepository, TransactionRepository)"]
            Dapper["<i class='fas fa-layer-group'></i> Dapper<br/>(Data Access Abstraction)"]
        end
    end

    %% === DATABASE PERSISTENCE ===
    subgraph DatabasePersistenceLayer["Database Layer"]
        SQLDb["<i class='fas fa-database'></i> SQL Server Database<br/>(Tables: Persons, Accounts, Transactions)"]
        StoredProcedures["<i class='fas fa-code'></i> Stored Procedures<br/>(GetPersons, GetPerson, UpsertPerson, etc.)"]
    end

    %% === DEFINE CONNECTIONS BETWEEN COMPONENTS ===
    User --> VueApp

    VueApp -- "HTTPS Requests<br/>(REST API Calls - JSON)" --> Controllers

    Controllers --> Services

    Services -- "Uses / Returns" --> Entities
    Services -- "Calls Methods Of" --> RepoInterfaces
    Services -- "Reads from / Writes to" --> SvcCache

    %% Implementation of Domain Interfaces by Infrastructure
    RepoImpl -- "Implements" --> RepoInterfaces
    RepoImpl -- "Uses" --> Dapper
    Dapper -- "Executes" --> StoredProcedures
    StoredProcedures -- "Interacts With" --> SQLDb
