CREATE USER [TheVirtualAgent]
	FOR LOGIN [TheVirtualAgentLogin]
	WITH DEFAULT_SCHEMA = dbo
GO;

GRANT CONNECT TO [TheVirtualAgent];
GO;

GRANT EXECUTE ON SCHEMA::dbo TO [TheVirtualAgent];
GO;

GRANT SELECT ON OBJECT::dbo.Persons TO [TheVirtualAgent];
GO;

GRANT SELECT ON OBJECT::dbo.Accounts TO [TheVirtualAgent];
GO;

GRANT SELECT ON OBJECT::dbo.Transactions TO [TheVirtualAgent];
GO;