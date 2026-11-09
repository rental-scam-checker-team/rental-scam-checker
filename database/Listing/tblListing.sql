/* =====================================================================
   Rental Scam Checker - tblListing (owner: Aman Paudel)
   Jira: RSC-18 to RSC-24 (Listings), RSC-50 (spec)

   Run order:
     1. Run this file FIRST, while connected to the RentalScamChecker database.
        tblRiskCheck has a foreign key to this table.
     2. Then run database/RiskCheck, database/PriceBenchmark, database/SafetyTip.

   This script is safe to run again: it only creates the table if it is
   missing (so it never breaks the link from tblRiskCheck), it replaces
   the stored procedures, and it only adds test data to an empty table.

   Contents:
     1. Table
     2. Stored procedures: Insert, Update, Delete, SelectAll,
        FilterByListingId, Filter
     3. Test data (10 rows, ListingId 1-10)
   ===================================================================== */

/* ---------------------------------------------------------------------
   1. TABLE
   The CHECK rules are a second safety net. The main validation is done
   by Listing.Valid() in the C# class before anything is saved.
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.tblListing', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblListing
    (
        ListingId    INT IDENTITY(1,1) NOT NULL,   -- PK, counts up by itself
        Address      NVARCHAR(100)     NOT NULL,   -- street address from the ad
        Area         NVARCHAR(50)      NOT NULL,   -- the city, matches the price benchmarks
        MonthlyRent  DECIMAL(8,2)      NOT NULL,   -- kr per month
        SizeM2       INT               NOT NULL,   -- size in m2
        PostedDate   DATE              NOT NULL,   -- when the ad was posted
        Furnished    BIT               NOT NULL,   -- 1 = furnished, 0 = not
        CONSTRAINT PK_tblListing PRIMARY KEY (ListingId),
        CONSTRAINT CK_tblListing_MonthlyRent CHECK (MonthlyRent > 0 AND MonthlyRent <= 50000),
        CONSTRAINT CK_tblListing_SizeM2 CHECK (SizeM2 BETWEEN 5 AND 500)
    );
END
GO

/* ---------------------------------------------------------------------
   2. STORED PROCEDURES
   --------------------------------------------------------------------- */

-- ADD: saves a new listing and returns its new ListingId
CREATE OR ALTER PROCEDURE dbo.sproc_tblListing_Insert
    @Address      NVARCHAR(100),
    @Area         NVARCHAR(50),
    @MonthlyRent  DECIMAL(8,2),
    @SizeM2       INT,
    @PostedDate   DATE,
    @Furnished    BIT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.tblListing (Address, Area, MonthlyRent, SizeM2, PostedDate, Furnished)
    VALUES (@Address, @Area, @MonthlyRent, @SizeM2, @PostedDate, @Furnished);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS ListingId;
END
GO

-- EDIT: changes a saved listing, returns how many rows changed (1 = done, 0 = not found)
CREATE OR ALTER PROCEDURE dbo.sproc_tblListing_Update
    @ListingId    INT,
    @Address      NVARCHAR(100),
    @Area         NVARCHAR(50),
    @MonthlyRent  DECIMAL(8,2),
    @SizeM2       INT,
    @PostedDate   DATE,
    @Furnished    BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.tblListing
    SET Address     = @Address,
        Area        = @Area,
        MonthlyRent = @MonthlyRent,
        SizeM2      = @SizeM2,
        PostedDate  = @PostedDate,
        Furnished   = @Furnished
    WHERE ListingId = @ListingId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- DELETE: business rule - a listing that has risk checks cannot be deleted.
-- Returns -1 if it has risk checks, 1 if it was deleted, 0 if it was not found.
CREATE OR ALTER PROCEDURE dbo.sproc_tblListing_Delete
    @ListingId INT
AS
BEGIN
    SET NOCOUNT ON;
    IF OBJECT_ID('dbo.tblRiskCheck', 'U') IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.tblRiskCheck WHERE ListingId = @ListingId)
        BEGIN
            SELECT -1 AS Result;
            RETURN;
        END
    END

    DELETE FROM dbo.tblListing WHERE ListingId = @ListingId;
    SELECT @@ROWCOUNT AS Result;
END
GO

-- LIST: all listings, newest first
CREATE OR ALTER PROCEDURE dbo.sproc_tblListing_SelectAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ListingId, Address, Area, MonthlyRent, SizeM2, PostedDate, Furnished
    FROM dbo.tblListing
    ORDER BY PostedDate DESC, ListingId DESC;
END
GO

-- FIND: one listing by its ListingId
CREATE OR ALTER PROCEDURE dbo.sproc_tblListing_FilterByListingId
    @ListingId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ListingId, Address, Area, MonthlyRent, SizeM2, PostedDate, Furnished
    FROM dbo.tblListing
    WHERE ListingId = @ListingId;
END
GO

-- FILTER: by city, rent range and furnished. Any parameter left NULL is ignored.
CREATE OR ALTER PROCEDURE dbo.sproc_tblListing_Filter
    @Area       NVARCHAR(50)  = NULL,
    @MinRent    DECIMAL(8,2)  = NULL,
    @MaxRent    DECIMAL(8,2)  = NULL,
    @Furnished  BIT           = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ListingId, Address, Area, MonthlyRent, SizeM2, PostedDate, Furnished
    FROM dbo.tblListing
    WHERE (@Area      IS NULL OR Area = @Area)
      AND (@MinRent   IS NULL OR MonthlyRent >= @MinRent)
      AND (@MaxRent   IS NULL OR MonthlyRent <= @MaxRent)
      AND (@Furnished IS NULL OR Furnished = @Furnished)
    ORDER BY PostedDate DESC, ListingId DESC;
END
GO

/* ---------------------------------------------------------------------
   3. TEST DATA
   10 listings with ListingId 1-10 (the risk check test data uses these IDs).
   Some are normal, some look like scams (very cheap for the size or city).
   Only added when the table is empty, so running the script again does
   not make duplicates.
   --------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.tblListing)
BEGIN
    SET IDENTITY_INSERT dbo.tblListing ON;

    INSERT INTO dbo.tblListing (ListingId, Address, Area, MonthlyRent, SizeM2, PostedDate, Furnished) VALUES
    (1,  N'Nørrebrogade 45, 2. th.',     N'Copenhagen', 9500.00,  55, '2026-10-02', 0),
    (2,  N'Amagerbrogade 120, 3. tv.',   N'Copenhagen', 3200.00,  70, '2026-10-20', 1),  -- very cheap: scam sign
    (3,  N'Jægersborggade 8, st.',       N'Copenhagen', 7800.00,  42, '2026-09-15', 1),
    (4,  N'Frederiks Allé 30, 1.',       N'Aarhus',     7200.00,  60, '2026-10-05', 0),
    (5,  N'Vestergade 12, 4.',           N'Aarhus',     2500.00,  85, '2026-10-25', 1),  -- very cheap: scam sign
    (6,  N'Kongensgade 66, 2.',          N'Odense',     5600.00,  48, '2026-09-28', 0),
    (7,  N'Vesterbro 101, 1. th.',       N'Odense',     6100.00,  65, '2026-10-12', 1),
    (8,  N'Boulevarden 14, 3.',          N'Aalborg',    4900.00,  52, '2026-10-01', 0),
    (9,  N'Jomfru Ane Gade 5, 2.',       N'Aalborg',    1900.00,  75, '2026-10-28', 1),  -- very cheap: scam sign
    (10, N'Istedgade 70, 4. tv.',        N'Copenhagen', 11500.00, 68, '2026-08-30', 0);

    SET IDENTITY_INSERT dbo.tblListing OFF;
END
GO

-- Check: you should see 10 rows, newest first
EXEC dbo.sproc_tblListing_SelectAll;
GO
