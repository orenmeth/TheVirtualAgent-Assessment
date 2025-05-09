```mermaid
erDiagram
    Persons {
        int Code PK "Person's unique identifier (Primary Key)"
        nvarchar Name "Person's first name (NOT NULL)"
        nvarchar Surname "Person's last name (NOT NULL)"
        %% Added UK keyword for Id_Number to explicitly mark it as unique
        nvarchar Id_Number UK "Person's national ID (UNIQUE, NOT NULL)"
    }

    Accounts {
        int Code PK "Account's unique identifier (Primary Key)"
        %% Added UK keyword for Account_Number to explicitly mark it as unique
        nvarchar Account_Number UK "Unique account number (UNIQUE, NOT NULL)"
        decimal Outstanding_Balance "Current outstanding balance (NOT NULL)"
        int Person_Code FK "References Persons.Code (FK, NOT NULL)"
        int Account_Status_Id FK "References AccountStatuses.Id (FK, NOT NULL)"
    }

    AccountStatuses {
        int Id PK "Account status unique identifier (Primary Key)"
        %% Added UK keyword for StatusName to explicitly mark it as unique
        nvarchar StatusName UK "Name of the account status (UNIQUE, NOT NULL)"
    }

    Transactions {
        int code PK "Transaction unique identifier (Primary Key, IDENTITY)"
        int account_code FK "References Accounts.Code (FK, NOT NULL)"
        datetime transaction_date "Date transaction occurred (NOT NULL)"
        datetime capture_date "Date transaction captured (NOT NULL, DEFAULT GETDATE())"
        money amount "Transaction amount (NOT NULL)"
        %% Type 'varchar' with length specified in comment, as Mermaid supports basic types
        varchar description "Desc. of transaction (VARCHAR(100), NOT NULL)"
    }

    %% Relationships
    Persons        ||--o{ Accounts         : "has"
    AccountStatuses ||--o{ Accounts         : "defines_status_for"
    Accounts       ||--o{ Transactions     : "records"