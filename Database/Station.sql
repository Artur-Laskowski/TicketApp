﻿CREATE TABLE [dbo].[Station]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(64) NOT NULL, 
    [Lat] FLOAT NOT NULL, 
    [Lng] FLOAT NOT NULL
)
