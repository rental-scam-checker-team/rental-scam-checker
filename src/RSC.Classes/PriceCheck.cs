namespace RSC.Classes;

/// <summary>
/// The result of comparing one listing's rent with the price benchmark for its city.
/// </summary>
public class PriceCheckResult
{
    public int ListingId { get; set; }
    public string City { get; set; } = "";
    public decimal ListingRentPerM2 { get; set; }
    public decimal? BenchmarkRentPerM2 { get; set; }
    public string BenchmarkSource { get; set; } = "";

    /// <summary>How far below the benchmark the listing is, in %. Negative = above the benchmark.</summary>
    public decimal? PercentBelow { get; set; }

    /// <summary>Score the price alone suggests (0-100). The reviewer can raise it for other warning signs.</summary>
    public int SuggestedScore { get; set; }

    /// <summary>Text to put in the Flags field.</summary>
    public string Flag { get; set; } = "";
}

/// <summary>
/// Price check (RSC-19 / Jira RSC-31): a rent far below the normal rent per m² for the city
/// is one of the strongest scam signs. Owner: Guru Prasad Kandel.
/// Uses tblListing (Aman) and tblPriceBenchmark (Shrayana).
/// </summary>
public static class PriceCheck
{
    // Rules: how far below the benchmark → suggested score
    public const decimal VeryLowPercent = 60m;   // 60% or more below  → 90 (High)
    public const decimal LowPercent = 40m;       // 40-59.99% below    → 70 (High)
    public const decimal SlightlyLowPercent = 30m; // 30-39.99% below  → 50 (Medium)
    public const int VeryLowScore = 90;
    public const int LowScore = 70;
    public const int SlightlyLowScore = 50;
    public const int NormalScore = 10;

    /// <summary>
    /// Works out the result from the numbers only (no database), so it can be unit tested.
    /// benchmarkPerM2 is null when the city has no active benchmark.
    /// </summary>
    public static PriceCheckResult Calculate(int listingId, string city, decimal monthlyRent, int sizeM2,
                                             decimal? benchmarkPerM2, string benchmarkSource = "")
    {
        var result = new PriceCheckResult
        {
            ListingId = listingId,
            City = city,
            BenchmarkRentPerM2 = benchmarkPerM2,
            BenchmarkSource = benchmarkSource,
        };

        if (sizeM2 <= 0 || monthlyRent <= 0)
        {
            result.SuggestedScore = NormalScore;
            result.Flag = "Price check not possible: rent or size is missing";
            return result;
        }

        result.ListingRentPerM2 = Math.Round(monthlyRent / sizeM2, 2);

        if (benchmarkPerM2 is null || benchmarkPerM2 <= 0)
        {
            result.SuggestedScore = NormalScore;
            result.Flag = $"No active price benchmark for {city}";
            return result;
        }

        decimal percentBelow = Math.Round((benchmarkPerM2.Value - result.ListingRentPerM2) / benchmarkPerM2.Value * 100m, 1);
        result.PercentBelow = percentBelow;
        result.SuggestedScore = ScoreFor(percentBelow);

        string compare = $"{result.ListingRentPerM2:0} kr/m² vs {city} benchmark {benchmarkPerM2.Value:0} kr/m²";
        result.Flag = percentBelow >= SlightlyLowPercent
            ? $"Rent {percentBelow:0}% below the {city} average ({compare})"
            : $"Rent within the normal range for {city} ({compare})";
        return result;
    }

    /// <summary>Suggested score for a given % below the benchmark.</summary>
    public static int ScoreFor(decimal percentBelow)
    {
        if (percentBelow >= VeryLowPercent) return VeryLowScore;
        if (percentBelow >= LowPercent) return LowScore;
        if (percentBelow >= SlightlyLowPercent) return SlightlyLowScore;
        return NormalScore;
    }

    /// <summary>
    /// Runs the price check for a listing in the database.
    /// Returns null if the listing does not exist.
    /// </summary>
    public static PriceCheckResult? ForListing(int listingId)
    {
        var listing = new ListingCollection(DbSettings.ConnectionString).Find(listingId);
        if (listing is null) return null;

        // The newest active benchmark for the listing's city
        var benchmark = new PriceBenchmarkCollection(DbSettings.ConnectionString)
            .Filter(listing.Area, true)
            .Where(b => b.PrivateRentPerM2 is not null)
            .OrderByDescending(b => b.DataDate)
            .FirstOrDefault();

        return Calculate(listing.ListingId, listing.Area, listing.MonthlyRent, listing.SizeM2,
                         benchmark?.PrivateRentPerM2, benchmark?.SourceReference ?? "");
    }
}
