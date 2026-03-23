USE OlympusIdentityPOC;
GO

DROP TABLE IF EXISTS Users;
GO

CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Email NVARCHAR(200) NOT NULL,
    Name NVARCHAR(200),
    EntraObjectId NVARCHAR(200),
    TenantId NVARCHAR(200),
    Role NVARCHAR(50),
    ApplicationId NVARCHAR(200),
    CreatedDate DATETIME
);
GO

DROP TABLE IF EXISTS Invitations;
GO

CREATE TABLE Invitations (
    InvitationId UNIQUEIDENTIFIER PRIMARY KEY,
    Email NVARCHAR(200) NOT NULL,
    Token NVARCHAR(200) NOT NULL,
    Application NVARCHAR(200),
    Role NVARCHAR(50),
    Status NVARCHAR(50),
    EntraObjectId NVARCHAR(200),
    CreatedDate DATETIME
);
GO
