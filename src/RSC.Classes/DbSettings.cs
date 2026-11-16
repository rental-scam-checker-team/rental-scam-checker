namespace RSC.Classes;

/// <summary>
/// Where the database is. The API sets this at startup from appsettings.json (ConnectionStrings:RSC).
/// The default is the local SQL Server Express used for development and the database tests.
/// </summary>
public static class DbSettings
{
    public static string ConnectionString { get; set; } =
        @"Server=localhost\SQLEXPRESS;Database=RentalScamChecker;Trusted_Connection=True;TrustServerCertificate=True";
}
