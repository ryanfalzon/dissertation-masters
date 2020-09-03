CREATE TABLE [dbo].[Posts]
(
	[Id] INT NOT NULL  IDENTITY, 
    [UserId] INT NOT NULL, 
    [Hash] NVARCHAR(MAX) NOT NULL, 
    [Content] NVARCHAR(MAX) NOT NULL, 
    [Timestamp] BIGINT NOT NULL, 
    [Likes] BIGINT NOT NULL, 
    CONSTRAINT [PK_Posts] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Posts_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
)