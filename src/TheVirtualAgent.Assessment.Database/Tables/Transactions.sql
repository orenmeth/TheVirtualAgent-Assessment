CREATE TABLE Transactions(
  code int not null identity(1,1),
  account_code int not null,
  transaction_date datetime not null,
  capture_date datetime not null,
  amount money not null,
  description varchar(100) not null,

  constraint FK_Transaction_Account foreign key (account_code) references Accounts(code),

  constraint PK_Transactions primary key clustered
  (
    code
  )
)
GO;