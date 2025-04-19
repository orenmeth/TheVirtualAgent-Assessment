CREATE LOGIN [TheVirtualAgentLogin]
	WITH PASSWORD = '$(TheVirtualAgentUserPassword)'
	HASHED,
	DEFAULT_LANGUAGE = [us_english],
	CHECK_EXPIRATION = OFF;