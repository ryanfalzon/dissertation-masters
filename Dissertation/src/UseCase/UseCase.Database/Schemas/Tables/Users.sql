CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL  IDENTITY, 
    [PublicKey] NVARCHAR(68) NOT NULL, 
    [FirstName] NVARCHAR(100) NOT NULL, 
    [LastName] NVARCHAR(100) NOT NULL, 
    [Email] NVARCHAR(100) NOT NULL, 
    [Mobile] BIGINT NOT NULL, 
    [Description] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
)