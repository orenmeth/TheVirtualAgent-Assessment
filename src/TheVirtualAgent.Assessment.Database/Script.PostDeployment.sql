/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
--:r ./Security/Logins/TheVirtualAgentLogin.sql
:r ./Security/Roles/ReadWrite_PI.sql
:r ./Security/Users/TheVirtualAgent.sql

:r ./Presets/Persons.sql
:r ./Presets/Accounts.sql
:r ./Presets/Transactions.sql