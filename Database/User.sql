﻿CREATE TABLE [dbo].[User]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Login] NVARCHAR(32) NOT NULL, 
    [Password] BINARY(128) NOT NULL
)
