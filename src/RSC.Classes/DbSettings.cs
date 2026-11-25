namespace RSC.Classes;

/// <summary>
/// Where the database is. The API sets this at startup from appsettings.json (ConnectionStrings:RSC).
/// The default is the shared SQL Server Express instance used for development and database tests.
/// </summary>
public static class DbSettings
{
    public static string ConnectionString { get; set; } =
        @"Server=localhost\SQLEXPRESS;Database=RentalScamChecker;Trusted_Connection=True;TrustServerCertificate=True";
}
