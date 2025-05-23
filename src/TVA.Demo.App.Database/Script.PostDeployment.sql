﻿/*
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
:r .\Security\Roles\ReadWrite_PI.sql

:r .\Presets\dbo\PersonPresets.sql
:r .\Presets\dbo\AccountStatusPresets.sql
:r .\Presets\dbo\AccountPresets.sql
:r .\Presets\dbo\TransactionPresets.sql