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

INSERT INTO tblSafetyTip (Title, Description, Category, Priority, IsPublished, DateAdded)
VALUES
    ('Never pay before viewing', 'Do not pay a deposit or rent before you have seen the flat in person or through a live video call.', 'Payment', 5, 1, '2026-10-01'),
    ('Be careful with Western Union or crypto', 'Scammers often ask for payment by Western Union, gift cards or crypto because the money cannot be traced or sent back. Pay by bank transfer to a Danish account instead.', 'Payment', 4, 1, '2026-10-02'),
    ('Check the landlord''s identity', 'Ask for the landlord''s full name and check it against the owner listed on the property. A real landlord will not mind showing who they are.', 'Contact', 4, 1, '2026-10-03'),
    ('Landlord is abroad and cannot meet', 'Be careful if the landlord says they live abroad and will send the keys by post. This is one of the most common rental scam stories.', 'Contact', 5, 1, '2026-10-05'),
    ('Too cheap for the area', 'If the rent is much lower than similar flats nearby, ask yourself why. A deal that looks too good to be true often is.', 'Price', 3, 1, '2026-10-06'),
    ('Pressure to decide fast', 'Scammers try to rush you by saying many people want the flat. Take your time and do not pay just because you feel pressured.', 'Contact', 2, 0, '2026-10-08'),
    ('Always get a written contract', 'Make sure you get a written rental contract before you pay anything. Read it carefully and keep a copy for yourself.', 'Contract', 3, 1, '2026-10-10'),
    ('Check the address really exists', 'Look up the address on a map and check that the building and flat match the photos in the ad.', 'Viewing', 2, 1, '2026-10-12'),
    ('Ask for the keys at handover', 'Only pay the first rent when you can collect the keys and see that they open the door of the flat.', 'Viewing', 1, 0, '2026-10-14'),
    ('Deposit max 3 months'' rent', 'In Denmark a landlord can ask for a deposit of at most three months of rent. If they ask for more, be careful.', 'Contract', 1, 0, '2026-10-15');
GO