CREATE TABLE [dbo].[Ticket]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [StartId] INT NOT NULL, 
    [DestinationId] INT NOT NULL
)
