```mermaid
classDiagram
    class PersonController {
        - ILogger<PersonController> _logger
        - IPersonService _personService
        + GetPersonsAsync(GetPersonsRequest, CancellationToken) : Task<IActionResult>
        + GetPersonByCodeAsync(int, CancellationToken) : Task<IActionResult>
        + DeletePersonAsync(int, CancellationToken) : Task<IActionResult>
        + UpsertPersonAsync(PersonRequest, CancellationToken) : Task<IActionResult>
    }

    class IPersonService {
        <<interface>>
        + GetPersonsAsync(string?, string?, bool, int, int, CancellationToken) : Task<PagedResponse<PersonResponse>>
        + GetPersonAsync(int, CancellationToken) : Task<PersonResponse>
        + DeletePersonAsync(int, CancellationToken) : Task
        + UpsertPersonAsync(PersonRequest, CancellationToken) : Task<PersonResponse>
    }

    class IPersonRepository {
        <<interface>>
        + GetPersonsAsync(CancellationToken) : Task<IEnumerable<PersonDto>>
        + GetPersonAsync(int, CancellationToken) : Task<PersonDto?>
        + UpsertPersonAsync(PersonDto, CancellationToken) : Task<int>
        + DeletePersonAsync(int, bool, CancellationToken) : Task
    }

    class PersonRepository {
        - string _connectionString
        + GetPersonsAsync() : Task<IEnumerable<PersonDto>>
        + GetPersonAsync(int) : Task<PersonDto?>
        + UpsertPersonAsync(int?, string, string, string) : Task
        + DeletePersonAsync(int, bool) : Task
    }

    class GetPersonsRequest {
        + string? SortBy
        + string? Filter
        + int Page
        + int PageSize
        + bool Descending
    }

    class PersonRequest {
        + int Code
        + string? Name
        + string? Surname
        + string IdNumber
    }

    class PersonResponse {
        + int Code
        + string? Name
        + string? Surname
        + string IdNumber
        + List<AccountResponse> Accounts
    }

    class AccountController {
        - ILogger<AccountController> _logger
        - IAccountService _accountService
        + GetAccountByCodeAsync(int, CancellationToken) : Task<IActionResult>
        + DeleteAccountAsync(int, CancellationToken) : Task<IActionResult>
        + UpsertAccountAsync(AccountRequest, CancellationToken) : Task<IActionResult>
    }

    class IAccountService {
        <<interface>>
        + GetAccountAsync(int, CancellationToken) : Task<AccountResponse>
        + DeleteAccountAsync(int, CancellationToken) : Task
        + UpsertAccountAsync(AccountRequest, CancellationToken) : Task<AccountResponse>
        + GetAccountsByPersonCodeAsync(int, CancellationToken) : Task<List<AccountResponse>>
    }

    class IAccountRepository {
        <<interface>>
        + GetAccountAsync(int, CancellationToken) : Task<AccountDto?>
        + GetAccountsByPersonCodeAsync(int, CancellationToken) : Task<IEnumerable<AccountDto>>
        + UpsertAccountAsync(AccountDto, CancellationToken) : Task<int>
        + DeleteAccountAsync(int, CancellationToken) : Task
    }

    class AccountRepository {
        - string _connectionString
        + GetAccountAsync(int) : Task<AccountDto?>
        + GetAccountsAsync() : Task<IEnumerable<AccountDto>>
        + UpsertAccountAsync(int?, int, string, string, decimal) : Task
        + DeleteAccountAsync(int) : Task
    }

    class AccountRequest {
        + int Code
        + int PersonCode
        + string AccountNumber
        + decimal OutstandingBalance
        + int AccountStatusId
    }

    class AccountResponse {
        + int Code
        + int PersonCode
        + string AccountNumber
        + decimal OutstandingBalance
        + int AccountStatusId
        + List<TransactionResponse> Transactions
    }

    class TransactionController {
        - ILogger<TransactionController> _logger
        - ITransactionService _transactionService
        + GetTransactionByCodeAsync(int, CancellationToken) : Task<IActionResult>
        + DeleteTransactionAsync(int, CancellationToken) : Task<IActionResult>
        + UpsertTransactionAsync(TransactionRequest, CancellationToken) : Task<IActionResult>
    }

    class ITransactionService {
        <<interface>>
        + GetTransactionAsync(int, CancellationToken) : Task<TransactionResponse>
        + DeleteTransactionAsync(int, CancellationToken) : Task
        + UpsertTransactionAsync(TransactionRequest, CancellationToken) : Task<TransactionResponse>
        + GetTransactionsByAccountCodeAsync(int, CancellationToken) : Task<List<TransactionResponse>>
    }

    class ITransactionRepository {
        <<interface>>
        + GetTransactionAsync(int, CancellationToken) : Task<TransactionDto?>
        + GetTransactionsByAccountCodeAsync(int, CancellationToken) : Task<IEnumerable<TransactionDto>>
        + UpsertTransactionAsync(TransactionDto, CancellationToken) : Task<int>
        + DeleteTransactionAsync(int, CancellationToken) : Task
    }

    class TransactionRepository {
        - string _connectionString
        + GetTransactionAsync(int) : Task<TransactionDto?>
        + GetTransactionsByAccountCodeAsync(int) : Task<IEnumerable<TransactionDto>>
        + UpsertTransactionAsync(int?, int, DateTime, DateTime, decimal, string) : Task
        + DeleteTransactionAsync(int) : Task
    }

    class TransactionRequest {
        + int Code
        + int AccountCode
        + string TransactionDate
        + decimal Amount
        + string Description
    }

    class TransactionResponse {
        + int Code
        + int AccountCode
        + string TransactionDate
        + string CaptureDate
        + decimal Amount
        + string Description
    }

    PersonController --> IPersonService
    IPersonService --> IPersonRepository
    IPersonRepository <|-- PersonRepository
    PersonController --> GetPersonsRequest
    PersonController --> PersonRequest
    PersonController --> PersonResponse
    PersonResponse --> AccountResponse
    AccountController --> IAccountService
    IAccountService --> IAccountRepository
    IAccountRepository <|-- AccountRepository
    AccountController --> AccountRequest
    AccountController --> AccountResponse
    AccountResponse --> TransactionResponse
    TransactionController --> ITransactionService
    ITransactionService --> ITransactionRepository
    ITransactionRepository <|-- TransactionRepository
    TransactionController --> TransactionRequest
    TransactionController --> TransactionResponse
