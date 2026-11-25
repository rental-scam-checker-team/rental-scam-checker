USE RentalScamChecker;
GO

IF OBJECT_ID('tblAdmin') IS NOT NULL
    DROP TABLE tblAdmin;
GO

CREATE TABLE tblAdmin (
    AdminId       INT IDENTITY(1,1) PRIMARY KEY,
    Username      NVARCHAR(50)   NOT NULL UNIQUE,
    PasswordHash  NVARCHAR(200)  NOT NULL,
    DateCreated   DATE           NOT NULL
);
GO

INSERT INTO tblAdmin (Username, PasswordHash, DateCreated)
VALUES ('admin', '100000.UlNDYWRtaW5TYWx0MjAyNg==.AaoNQRB53yDhxAjCBnbkb1TlwYFDhSTfCRlH8JSsBuQ=', '2026-11-24');
GO

DROP PROCEDURE IF EXISTS sproc_tblAdmin_FilterByUsername;
GO
CREATE PROCEDURE sproc_tblAdmin_FilterByUsername
    @Username  NVARCHAR(50)
AS
BEGIN
    SELECT AdminId, Username, PasswordHash, DateCreated
    FROM tblAdmin
    WHERE Username = @Username;
END
GO
