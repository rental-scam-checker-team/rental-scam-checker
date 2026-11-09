using System.Globalization;
using Microsoft.Data.SqlClient;

namespace RSC.Classes;

/// <summary>
/// One rental listing (one row in tblListing). Owner: Aman Paudel.
/// Jira: RSC-18 (Add + validation), RSC-22 (Find), RSC-24 (tests).
/// </summary>
public class Listing
{
    // The cities in the price benchmark data. Area must be one of these,
    // otherwise the price check cannot find a benchmark.
    // TODO: check the full list with Shrayana/Guru and add any missing cities.
    public static readonly string[] AllowedCities = { "Copenhagen", "Aarhus", "Odense", "Aalborg" };

    // Validation limits, kept in one place so the tests and the rules match
    public const int AddressMaxLength = 100;
    public const int AreaMaxLength = 50;
    public const decimal RentMax = 50000m;
    public const int SizeMin = 5;
    public const int SizeMax = 500;

    // Rent may have a decimal point but no thousands separator,
    // so "6,500" is not read as 6500 by mistake.
    private const NumberStyles RentStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

    private readonly string? connectionString;

    public Listing() { }

    public Listing(string connectionString)
    {
        this.connectionString = connectionString;
    }

    // One property per column in tblListing
    public int ListingId { get; set; }
    public string Address { get; set; } = "";
    public string Area { get; set; } = "";
    public decimal MonthlyRent { get; set; }
    public int SizeM2 { get; set; }
    public DateTime PostedDate { get; set; }
    public bool Furnished { get; set; }

    /// <summary>
    /// Checks the values typed into the form. Everything comes in as text,
    /// because a user can type anything (for example letters in the rent box).
    /// Returns "" when all data is fine, otherwise one error message per wrong field.
    /// </summary>
    public string Valid(string address, string area, string monthlyRent, string sizeM2,
                        string postedDate, string furnished)
    {
        var errors = new List<string>();

        // Address: required, 1-100 characters
        address = (address ?? "").Trim();
        if (address.Length == 0)
            errors.Add("Address must not be blank.");
        else if (address.Length > AddressMaxLength)
            errors.Add($"Address must be {AddressMaxLength} characters or less.");

        // Area: required, max 50 characters, must be one of the benchmark cities
        area = (area ?? "").Trim();
        if (area.Length == 0)
            errors.Add("City must not be blank.");
        else if (area.Length > AreaMaxLength)
            errors.Add($"City must be {AreaMaxLength} characters or less.");
        else if (!AllowedCities.Contains(area, StringComparer.OrdinalIgnoreCase))
            errors.Add("City must be one of: " + string.Join(", ", AllowedCities) + ".");

        // MonthlyRent: a number, greater than 0 and max 50,000 kr
        if (!decimal.TryParse(monthlyRent, RentStyle, CultureInfo.InvariantCulture, out decimal rent))
            errors.Add("Monthly rent must be a number.");
        else if (rent <= 0)
            errors.Add("Monthly rent must be greater than 0.");
        else if (rent > RentMax)
            errors.Add($"Monthly rent must be {RentMax:0} kr or less.");

        // SizeM2: a whole number from 5 to 500
        if (!int.TryParse(sizeM2, NumberStyles.Integer, CultureInfo.InvariantCulture, out int size))
            errors.Add("Size must be a whole number.");
        else if (size < SizeMin || size > SizeMax)
            errors.Add($"Size must be between {SizeMin} and {SizeMax} m².");

        // PostedDate: a real date (yyyy-MM-dd), not in the future
        if (!DateTime.TryParseExact(postedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                    DateTimeStyles.None, out DateTime posted))
            errors.Add("Posted date must be a real date (yyyy-mm-dd).");
        else if (posted.Date > DateTime.Today)
            errors.Add("Posted date cannot be in the future.");

        // Furnished: yes or no
        if (!TryParseYesNo(furnished, out _))
            errors.Add("Furnished must be yes or no.");

        return string.Join(" ", errors);
    }

    /// <summary>Accepts true/false, 1/0 and yes/no.</summary>
    public static bool TryParseYesNo(string? value, out bool result)
    {
        switch ((value ?? "").Trim().ToLowerInvariant())
        {
            case "true": case "1": case "yes": result = true; return true;
            case "false": case "0": case "no": result = false; return true;
            default: result = false; return false;
        }
    }

    /// <summary>
    /// Fills the properties from text that has already passed Valid().
    /// </summary>
    public void SetFromText(string address, string area, string monthlyRent, string sizeM2,
                            string postedDate, string furnished)
    {
        Address = address.Trim();
        // store the city with the same spelling as the benchmark list
        Area = AllowedCities.First(c => c.Equals(area.Trim(), StringComparison.OrdinalIgnoreCase));
        MonthlyRent = decimal.Parse(monthlyRent, RentStyle, CultureInfo.InvariantCulture);
        SizeM2 = int.Parse(sizeM2, CultureInfo.InvariantCulture);
        PostedDate = DateTime.ParseExact(postedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        TryParseYesNo(furnished, out bool f);
        Furnished = f;
    }

    /// <summary>
    /// Loads one listing from the database into this object.
    /// Returns true if it was found, false if not.
    /// </summary>
    public bool Find(int listingId)
    {
        if (connectionString == null)
            throw new InvalidOperationException("Create the Listing with a connection string to use Find.");

        using var conn = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("dbo.sproc_tblListing_FilterByListingId", conn)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@ListingId", listingId);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return false;

        CopyFrom(reader);
        return true;
    }

    /// <summary>Copies one row from the database into the properties.</summary>
    internal void CopyFrom(SqlDataReader reader)
    {
        ListingId = reader.GetInt32(reader.GetOrdinal("ListingId"));
        Address = reader.GetString(reader.GetOrdinal("Address"));
        Area = reader.GetString(reader.GetOrdinal("Area"));
        MonthlyRent = reader.GetDecimal(reader.GetOrdinal("MonthlyRent"));
        SizeM2 = reader.GetInt32(reader.GetOrdinal("SizeM2"));
        PostedDate = reader.GetDateTime(reader.GetOrdinal("PostedDate"));
        Furnished = reader.GetBoolean(reader.GetOrdinal("Furnished"));
    }
}
