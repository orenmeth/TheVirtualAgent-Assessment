CREATE ROLE [ReadWrite_PI]
	AUTHORIZATION [dbo];
GO;

ALTER ROLE [db_datawriter] ADD MEMBER [ReadWrite_PI];
GO;

ALTER ROLE [db_datareader] ADD MEMBER [ReadWrite_PI];
GO;