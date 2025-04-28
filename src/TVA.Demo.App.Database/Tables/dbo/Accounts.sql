CREATE TABLE [dbo].[Accounts](
  code int not null identity(1,1),
  person_code int not null,
  account_number varchar(50) not null,
  outstanding_balance money not null,

  constraint FK_Account_Person foreign key (person_code) references Persons(code),

  constraint PK_Accounts primary key clustered
  (
    code
  )
)
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_Account_num ON [dbo].[Accounts](account_number)
GO