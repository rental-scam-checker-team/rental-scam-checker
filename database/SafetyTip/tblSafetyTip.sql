USE RentalScamChecker;
GO

IF OBJECT_ID('tblSafetyTip') IS NOT NULL
    DROP TABLE tblSafetyTip;
GO

CREATE TABLE tblSafetyTip (
    TipId        INT IDENTITY(1,1) PRIMARY KEY,
    Title        NVARCHAR(100)  NOT NULL,
    Description  NVARCHAR(500)  NOT NULL,
    Category     NVARCHAR(50)   NOT NULL,
    Priority     INT            NOT NULL,
    IsPublished  BIT            NOT NULL DEFAULT 0,
    DateAdded    DATE           NOT NULL
);
GO