CREATE TABLE Persons(
  code int not null identity(1,1),
  name varchar(50) null,
  surname varchar(50) null,
  id_number varchar(50) not null,

  constraint PK_Persons primary key clustered
  (
    code
  )
)
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_Person_id ON Persons(id_number)
GO