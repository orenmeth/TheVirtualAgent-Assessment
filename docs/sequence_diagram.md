```mermaid
sequenceDiagram
    participant Client
    participant PersonController
    participant IPersonService
    participant IPersonRepository
    participant Database

    Client->>PersonController: HTTP Request (e.g., GetPersonsAsync)
    PersonController->>IPersonService: Call GetPersonsAsync(filter, sortBy, ...)
    IPersonService->>IPersonRepository: Call GetPersonsAsync(CancellationToken)
    IPersonRepository->>Database: Execute Stored Procedure (GetPersons)
    Database-->>IPersonRepository: Return List of Persons
    IPersonRepository-->>IPersonService: Return List of Persons
    IPersonService-->>PersonController: Return PagedResponse<PersonResponse>
    PersonController-->>Client: HTTP Response (200 OK with PagedResponse)

    Client->>PersonController: HTTP Request (e.g., GetPersonByCodeAsync)
    PersonController->>IPersonService: Call GetPersonAsync(code, CancellationToken)
    IPersonService->>IPersonRepository: Call GetPersonAsync(code, CancellationToken)
    IPersonRepository->>Database: Execute Stored Procedure (GetPerson)
    Database-->>IPersonRepository: Return Person
    IPersonRepository-->>IPersonService: Return PersonDto
    IPersonService-->>PersonController: Return PersonResponse
    PersonController-->>Client: HTTP Response (200 OK with PersonResponse)

    Client->>PersonController: HTTP Request (e.g., DeletePersonAsync)
    PersonController->>IPersonService: Call DeletePersonAsync(code, CancellationToken)
    IPersonService->>IPersonRepository: Call DeletePersonAsync(code, deleteRelatedAccounts, CancellationToken)
    IPersonRepository->>Database: Execute Stored Procedure (DeletePerson)
    Database-->>IPersonRepository: Acknowledge Deletion
    IPersonRepository-->>IPersonService: Acknowledge Deletion
    IPersonService-->>PersonController: Acknowledge Deletion
    PersonController-->>Client: HTTP Response (200 OK)

    Client->>PersonController: HTTP Request (e.g., UpsertPersonAsync)
    PersonController->>IPersonService: Call UpsertPersonAsync(person, CancellationToken)
    IPersonService->>IPersonRepository: Call UpsertPersonAsync(PersonDto, CancellationToken)
    IPersonRepository->>Database: Execute Stored Procedure (UpsertPerson)
    Database-->>IPersonRepository: Return Upserted Person Code
    IPersonRepository-->>IPersonService: Return Upserted Person Code
    IPersonService-->>PersonController: Return Upserted PersonResponse
    PersonController-->>Client: HTTP Response (200 OK with PersonResponse)
