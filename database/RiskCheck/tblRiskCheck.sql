/* =====================================================================
   Rental Scam Checker - tblRiskCheck (owner: Guru Prasad Kandel)
   Jira: RSC-14 (database setup), RSC-51 (schema)

   Run order:
     1. Aman's tblListing script must run FIRST (this table has a FK to it)
     2. Run this whole file in SQL Server Management Studio / Azure Data Studio

   Contents:
     1. Table
     2. Stored procedures: Insert, Update, Delete, SelectAll,
        FilterByRiskCheckId, Filter
     3. Test data (10 rows, needs ListingId 1-10 to exist)
   ===================================================================== */

/* ---------------------------------------------------------------------
   1. TABLE
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.tblRiskCheck', 'U') IS NOT NULL
    DROP TABLE dbo.tblRiskCheck;
GO

CREATE TABLE dbo.tblRiskCheck
(
    RiskCheckId   INT IDENTITY(1,1) NOT NULL,   -- PK
    ListingId     INT               NOT NULL,   -- FK -> tblListing
    RiskScore     INT               NOT NULL,   -- numeric, 0-100
    Flags         NVARCHAR(500)     NOT NULL,   -- string, reasons found
    ReviewNote    NVARCHAR(250)     NULL,       -- string, admin note
    CheckedDate   DATETIME          NOT NULL,   -- date
    UserNotified  BIT               NOT NULL CONSTRAINT DF_tblRiskCheck_UserNotified DEFAULT (0), -- Boolean

    CONSTRAINT PK_tblRiskCheck PRIMARY KEY (RiskCheckId),
    CONSTRAINT FK_tblRiskCheck_tblListing FOREIGN KEY (ListingId)
        REFERENCES dbo.tblListing (ListingId),  -- blocks deleting a listing that has checks (team to confirm)
    CONSTRAINT CK_tblRiskCheck_RiskScore CHECK (RiskScore BETWEEN 0 AND 100)
);
GO

/* ---------------------------------------------------------------------
   2. STORED PROCEDURES
   --------------------------------------------------------------------- */

-- ADD: inserts a risk check and returns the new RiskCheckId
CREATE OR ALTER PROCEDURE dbo.sproc_tblRiskCheck_Insert
    @ListingId     INT,
    @RiskScore     INT,
    @Flags         NVARCHAR(500),
    @ReviewNote    NVARCHAR(250) = NULL,
    @CheckedDate   DATETIME,
    @UserNotified  BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.tblRiskCheck (ListingId, RiskScore, Flags, ReviewNote, CheckedDate, UserNotified)
    VALUES (@ListingId, @RiskScore, @Flags, @ReviewNote, @CheckedDate, @UserNotified);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS RiskCheckId;
END
GO

-- EDIT: updates an existing risk check
CREATE OR ALTER PROCEDURE dbo.sproc_tblRiskCheck_Update
    @RiskCheckId   INT,
    @ListingId     INT,
    @RiskScore     INT,
    @Flags         NVARCHAR(500),
    @ReviewNote    NVARCHAR(250) = NULL,
    @CheckedDate   DATETIME,
    @UserNotified  BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tblRiskCheck
    SET ListingId    = @ListingId,
        RiskScore    = @RiskScore,
        Flags        = @Flags,
        ReviewNote   = @ReviewNote,
        CheckedDate  = @CheckedDate,
        UserNotified = @UserNotified
    WHERE RiskCheckId = @RiskCheckId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- DELETE: removes one risk check
CREATE OR ALTER PROCEDURE dbo.sproc_tblRiskCheck_Delete
    @RiskCheckId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.tblRiskCheck
    WHERE RiskCheckId = @RiskCheckId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- LIST: all risk checks, newest first, with the listing address
CREATE OR ALTER PROCEDURE dbo.sproc_tblRiskCheck_SelectAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT rc.RiskCheckId, rc.ListingId, l.Address, rc.RiskScore, rc.Flags,
           rc.ReviewNote, rc.CheckedDate, rc.UserNotified
    FROM dbo.tblRiskCheck rc
    LEFT JOIN dbo.tblListing l ON l.ListingId = rc.ListingId
    ORDER BY rc.CheckedDate DESC;
END
GO

-- FIND: one risk check by its ID
CREATE OR ALTER PROCEDURE dbo.sproc_tblRiskCheck_FilterByRiskCheckId
    @RiskCheckId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT rc.RiskCheckId, rc.ListingId, l.Address, rc.RiskScore, rc.Flags,
           rc.ReviewNote, rc.CheckedDate, rc.UserNotified
    FROM dbo.tblRiskCheck rc
    LEFT JOIN dbo.tblListing l ON l.ListingId = rc.ListingId
    WHERE rc.RiskCheckId = @RiskCheckId;
END
GO

-- FILTER: by score range, date range and notified status.
-- Any parameter left NULL is ignored, so filters can be combined or cleared.
CREATE OR ALTER PROCEDURE dbo.sproc_tblRiskCheck_Filter
    @MinScore      INT      = NULL,
    @MaxScore      INT      = NULL,
    @FromDate      DATETIME = NULL,
    @ToDate        DATETIME = NULL,
    @UserNotified  BIT      = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT rc.RiskCheckId, rc.ListingId, l.Address, rc.RiskScore, rc.Flags,
           rc.ReviewNote, rc.CheckedDate, rc.UserNotified
    FROM dbo.tblRiskCheck rc
    LEFT JOIN dbo.tblListing l ON l.ListingId = rc.ListingId
    WHERE (@MinScore     IS NULL OR rc.RiskScore    >= @MinScore)
      AND (@MaxScore     IS NULL OR rc.RiskScore    <= @MaxScore)
      AND (@FromDate     IS NULL OR rc.CheckedDate  >= @FromDate)
      AND (@ToDate       IS NULL OR rc.CheckedDate  <= @ToDate)
      AND (@UserNotified IS NULL OR rc.UserNotified  = @UserNotified)
    ORDER BY rc.CheckedDate DESC;
END
GO

/* ---------------------------------------------------------------------
   3. TEST DATA (needs ListingId 1-10 in tblListing)
   --------------------------------------------------------------------- */
INSERT INTO dbo.tblRiskCheck (ListingId, RiskScore, Flags, ReviewNote, CheckedDate, UserNotified) VALUES
(1,  85, N'Rent 58% below area average; asks to pay deposit before viewing', N'Classic advance-payment pattern', '2026-10-01 10:15', 1),
(2,  12, N'No red flags found', NULL, '2026-10-02 14:30', 1),
(3,  64, N'Landlord says they are abroad; viewing not possible', NULL, '2026-10-03 09:05', 0),
(4,  91, N'Rent 62% below average; photos found on Booking.com; pay via MoneyGram', N'Reported to team for review', '2026-10-05 18:40', 1),
(5,  35, N'Rent 20% below average', NULL, '2026-10-07 11:20', 0),
(6,  0,  N'No red flags found', N'Boundary test: minimum score', '2026-10-08 08:00', 1),
(7,  100, N'Deposit above 3 months rent; pressure to pay today; no CPR registration allowed', N'Boundary test: maximum score', '2026-10-10 16:55', 0),
(8,  47, N'Ad text asks to communicate only by WhatsApp', NULL, '2026-10-12 13:10', 1),
(9,  73, N'Rent 45% below average; landlord refuses video call', NULL, '2026-10-15 19:25', 0),
(10, 20, N'Furnished flat slightly below average rent', NULL, '2026-10-20 12:00', 1);
GO
