CREATE TABLE [dbo].[AccountStatus]
(
	[Id] INT NOT NULL identity(1,1),
	[Description] NVARCHAR(50) NOT NULL,

	constraint PK_AccountStatus primary key clustered
	(
	  Id
	)
)
GO