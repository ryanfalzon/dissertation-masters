CREATE TABLE [dbo].[PrivacySettings]
(
	[Id] INT NOT NULL , 
    [VisibleProfile] BIT NOT NULL DEFAULT 0, 
    [VisiblePosts] BIT NOT NULL DEFAULT 0, 
    [UserId] INT NOT NULL, 
    CONSTRAINT [PK_PrivacySettings] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_PrivacySettings_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
)