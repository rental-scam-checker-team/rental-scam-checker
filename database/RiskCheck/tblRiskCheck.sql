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
-- Scores match Aman's listings (tblListing) compared with Shrayana's benchmarks (tblPriceBenchmark, July 2026).
(1,  15,  N'Rent within the normal range for Copenhagen (173 kr/m2 vs benchmark 225 kr/m2); no red flags', NULL, '2026-10-01 10:15', 1),
(2,  92,  N'Rent 80% below the Copenhagen average (46 kr/m2 vs benchmark 225 kr/m2); asks for deposit before viewing', N'Classic advance-payment pattern', '2026-10-02 14:30', 1),
(3,  40,  N'Landlord says they are abroad; viewing not possible', N'Boundary test: Medium starts at 40', '2026-10-03 09:05', 0),
(4,  10,  N'Rent within the normal range for Aarhus (120 kr/m2 vs benchmark 151 kr/m2); landlord met at viewing', NULL, '2026-10-05 18:40', 1),
(5,  100, N'Rent 81% below the Aarhus average (29 kr/m2 vs benchmark 151 kr/m2); photos found on another site; pay via MoneyGram', N'Boundary test: maximum score', '2026-10-07 11:20', 0),
(6,  0,   N'No red flags found', N'Boundary test: minimum score', '2026-10-08 08:00', 1),
(7,  69,  N'Ad asks to communicate only by WhatsApp; pressure to decide today', N'Boundary test: Medium ends at 69', '2026-10-10 16:55', 0),
(8,  25,  N'Rent within the normal range for Aalborg (94 kr/m2 vs benchmark 96 kr/m2); asks for 3 months deposit (allowed by law)', NULL, '2026-10-12 13:10', 1),
(9,  88,  N'Rent 74% below the Aalborg average (25 kr/m2 vs benchmark 96 kr/m2); landlord refuses video call', NULL, '2026-10-15 19:25', 0),
(10, 39,  N'Rent within the normal range for Copenhagen (169 kr/m2 vs benchmark 225 kr/m2); landlord slow to answer', N'Boundary test: Low ends at 39', '2026-10-20 12:00', 1);
GO
