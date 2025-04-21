CREATE LOGIN [TheVirtualAgentLogin]
WITH PASSWORD = '$(TheVirtualAgentUserPassword)',
DEFAULT_LANGUAGE = [us_english],
CHECK_EXPIRATION = OFF,
CHECK_POLICY = OFF;
GO;