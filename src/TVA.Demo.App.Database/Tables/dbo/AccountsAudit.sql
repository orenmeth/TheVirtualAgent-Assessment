CREATE TABLE [dbo].[AccountsAudit]
(
	Id int not null identity(1,1),
	code int not null,
	person_code int not null,
	account_number varchar(50) not null,
	outstanding_balance money not null,
	account_status_id int not null,
	change_date datetime not null default GETDATE(),
	change_reason varchar(1) not null default 'U',

	constraint PK_AuditLog primary key clustered (Id)
)
GO
