/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
DROP USER IF EXISTS [TheVirtualAgent];

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'TheVirtualAgentLogin')
BEGIN
    DROP LOGIN [TheVirtualAgentLogin];
END;

DROP ROLE IF EXISTS [ReadWrite_PI];
