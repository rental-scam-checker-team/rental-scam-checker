/*
============================================================
Rental Scam Checker - tblPriceBenchmark
Component: Price Benchmarks
Jira: RSC-36

Contents:
1. Table
2. Stored procedures
   - Insert
   - Update
   - Delete
   - SelectAll
   - FilterByBenchmarkId
   - Filter
3. Test data
============================================================
*/
-- IMPORTANT: Use the correct database
USE RentalScamChecker;


GO
-- =========================================================
-- 1. TABLE
-- =========================================================
IF OBJECT_ID('dbo.tblPriceBenchmark', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.tblPriceBenchmark (
            BenchmarkId      INT             IDENTITY (1, 1) NOT NULL,
            City             NVARCHAR (50)   NOT NULL,
            PrivateRentPerM2 DECIMAL (10, 2) NULL,
            SocialRentPerM2  DECIMAL (10, 2) NULL,
            IsActive         BIT             DEFAULT 1 NOT NULL,
            DataDate         DATE            NOT NULL,
            SourceReference  NVARCHAR (200)  NOT NULL,
            CONSTRAINT PK_tblPriceBenchmark PRIMARY KEY (BenchmarkId),
            CONSTRAINT CK_tblPriceBenchmark_City CHECK (City IN ('Copenhagen', 'Aarhus', 'Odense', 'Aalborg')),
            CONSTRAINT CK_tblPriceBenchmark_PrivateRent CHECK (PrivateRentPerM2 IS NULL
                                                               OR PrivateRentPerM2 > 0),
            CONSTRAINT CK_tblPriceBenchmark_SocialRent CHECK (SocialRentPerM2 IS NULL
                                                              OR SocialRentPerM2 > 0)
        );
    END


GO
-- =========================================================
-- 2. STORED PROCEDURES
-- =========================================================
-- INSERT
CREATE OR ALTER PROCEDURE dbo.sproc_tblPriceBenchmark_Insert
@City NVARCHAR (50), @PrivateRentPerM2 DECIMAL (10, 2)=NULL, @SocialRentPerM2 DECIMAL (10, 2)=NULL, @IsActive BIT=1, @DataDate DATE, @SourceReference NVARCHAR (200)
AS
BEGIN
    SET NOCOUNT ON;
    IF @DataDate IS NULL
        THROW 50001, 'DataDate is required.', 1;
    IF @DataDate > CAST (GETDATE() AS DATE)
        THROW 50002, 'DataDate cannot be in the future.', 1;
    IF @City IS NULL
       OR @City NOT IN ('Copenhagen', 'Aarhus', 'Odense', 'Aalborg')
        THROW 50003, 'Invalid city.', 1;
    IF @SourceReference IS NULL
       OR LTRIM(RTRIM(@SourceReference)) = ''
        THROW 50004, 'SourceReference is required.', 1;
    INSERT  INTO dbo.tblPriceBenchmark (
        City,
        PrivateRentPerM2,
        SocialRentPerM2,
        IsActive,
        DataDate,
        SourceReference
    )
    VALUES                             (@City, @PrivateRentPerM2, @SocialRentPerM2, @IsActive, @DataDate, @SourceReference);
    SELECT CAST (SCOPE_IDENTITY() AS INT) AS BenchmarkId;
END


GO
-- UPDATE
CREATE OR ALTER PROCEDURE dbo.sproc_tblPriceBenchmark_Update
@BenchmarkId INT, @City NVARCHAR (50), @PrivateRentPerM2 DECIMAL (10, 2), @SocialRentPerM2 DECIMAL (10, 2), @IsActive BIT, @DataDate DATE, @SourceReference NVARCHAR (200)
AS
BEGIN
    SET NOCOUNT ON;
    IF @DataDate IS NULL
        THROW 50005, 'DataDate is required.', 1;
    IF @DataDate > CAST (GETDATE() AS DATE)
        THROW 50006, 'DataDate cannot be in the future.', 1;
    IF @City IS NULL
       OR @City NOT IN ('Copenhagen', 'Aarhus', 'Odense', 'Aalborg')
        THROW 50007, 'Invalid city.', 1;
    IF @SourceReference IS NULL
       OR LTRIM(RTRIM(@SourceReference)) = ''
        THROW 50008, 'SourceReference is required.', 1;
    UPDATE dbo.tblPriceBenchmark
    SET    City             = @City,
           PrivateRentPerM2 = @PrivateRentPerM2,
           SocialRentPerM2  = @SocialRentPerM2,
           IsActive         = @IsActive,
           DataDate         = @DataDate,
           SourceReference  = @SourceReference
    WHERE  BenchmarkId = @BenchmarkId;
    SELECT @@ROWCOUNT AS RowsAffected;
END


GO
-- DELETE
CREATE OR ALTER PROCEDURE dbo.sproc_tblPriceBenchmark_Delete
@BenchmarkId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE dbo.tblPriceBenchmark
    WHERE  BenchmarkId = @BenchmarkId;
    SELECT @@ROWCOUNT AS RowsAffected;
END


GO
-- SELECT ALL
CREATE OR ALTER PROCEDURE dbo.sproc_tblPriceBenchmark_SelectAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT   BenchmarkId,
             City,
             PrivateRentPerM2,
             SocialRentPerM2,
             IsActive,
             DataDate,
             SourceReference
    FROM     dbo.tblPriceBenchmark
    ORDER BY DataDate DESC, BenchmarkId DESC;
END


GO
-- FIND BY ID
CREATE OR ALTER PROCEDURE dbo.sproc_tblPriceBenchmark_FilterByBenchmarkId
@BenchmarkId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT BenchmarkId,
           City,
           PrivateRentPerM2,
           SocialRentPerM2,
           IsActive,
           DataDate,
           SourceReference
    FROM   dbo.tblPriceBenchmark
    WHERE  BenchmarkId = @BenchmarkId;
END


GO
-- FILTER BY CITY AND ACTIVE STATUS
CREATE OR ALTER PROCEDURE dbo.sproc_tblPriceBenchmark_Filter
@City NVARCHAR (50)=NULL, @IsActive BIT=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT   BenchmarkId,
             City,
             PrivateRentPerM2,
             SocialRentPerM2,
             IsActive,
             DataDate,
             SourceReference
    FROM     dbo.tblPriceBenchmark
    WHERE    (@City IS NULL
              OR City = @City)
             AND (@IsActive IS NULL
                  OR IsActive = @IsActive)
    ORDER BY DataDate DESC, BenchmarkId DESC;
END


GO
-- =========================================================
-- 3. TEST DATA
-- Insert sample records only if the table is empty
-- =========================================================
IF NOT EXISTS (SELECT 1
               FROM   dbo.tblPriceBenchmark)
    BEGIN
        INSERT  INTO dbo.tblPriceBenchmark (
            City,
            PrivateRentPerM2,
            SocialRentPerM2,
            IsActive,
            DataDate,
            SourceReference
        )
        VALUES                             ('Copenhagen', 225.00, 93.42, 1, '2026-07-01', 'Bolighub (2026) July rental market data; Landsbyggefonden (2026) Huslejestatistik 2026'),
                                           ('Aarhus', 151.00, 84.75, 1, '2026-07-01', 'Bolighub (2026) July rental market data; Landsbyggefonden (2026) Huslejestatistik 2026'),
                                           ('Odense', 119.00, 71.83, 1, '2026-07-01', 'Bolighub (2026) July rental market data; Landsbyggefonden (2026) Huslejestatistik 2026'),
                                           ('Aalborg', 96.00, 76.92, 1, '2026-07-01', 'Bolighub (2026) July rental market data; Landsbyggefonden (2026) Huslejestatistik 2026'),
                                           -- Synthetic test records: inactive, not for real price checks
                                           ('Copenhagen', 200.00, 90.00, 0, '2026-06-01', 'SYNTHETIC TEST DATA - NOT FOR PRODUCTION'),
                                           ('Aarhus', 140.00, 80.00, 0, '2026-06-01', 'SYNTHETIC TEST DATA - NOT FOR PRODUCTION'),
                                           ('Odense', 110.00, 70.00, 0, '2026-06-01', 'SYNTHETIC TEST DATA - NOT FOR PRODUCTION'),
                                           ('Aalborg', 90.00, 75.00, 0, '2026-06-01', 'SYNTHETIC TEST DATA - NOT FOR PRODUCTION');
    END


GO
-- =========================================================
-- 4. VERIFY THE TABLE AND TEST DATA
-- =========================================================
SELECT   *
FROM     dbo.tblPriceBenchmark
ORDER BY BenchmarkId;

-- Verify stored procedures
SELECT   name
FROM     sys.procedures
WHERE    name LIKE 'sproc_tblPriceBenchmark%'
ORDER BY name;