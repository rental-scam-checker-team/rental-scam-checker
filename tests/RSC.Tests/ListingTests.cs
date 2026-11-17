using RSC.Classes;

namespace RSC.Tests;

/// <summary>
/// Tests for the Listing class (owner: Aman Paudel, Jira RSC-24).
/// Boundary tests for every rule: min-1, min, min+1, mid, max-1, max, max+1,
/// an extreme value and an invalid type.
/// </summary>
public class ListingTests
{
    // A set of valid values. Each test changes ONE field, so we know exactly
    // which rule made Valid() fail.
    private const string GoodAddress = "Nørrebrogade 45, 2. th.";
    private const string GoodArea = "Copenhagen";
    private const string GoodRent = "6500";
    private const string GoodSize = "45";
    private static string GoodDate => DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
    private const string GoodFurnished = "false";

    private static string Check(string? address = null, string? area = null, string? rent = null,
                                string? size = null, string? date = null, string? furnished = null)
    {
        return new Listing().Valid(address ?? GoodAddress, area ?? GoodArea, rent ?? GoodRent,
                                   size ?? GoodSize, date ?? GoodDate, furnished ?? GoodFurnished);
    }

    // ---------- the class itself ----------

    [Fact]
    public void InstanceOk()
    {
        Assert.NotNull(new Listing());
    }

    [Fact]
    public void PropertiesOk()
    {
        var l = new Listing
        {
            ListingId = 3,
            Address = GoodAddress,
            Area = "Aarhus",
            MonthlyRent = 6500.50m,
            SizeM2 = 45,
            PostedDate = new DateTime(2026, 10, 1),
            Furnished = true
        };
        Assert.Equal(3, l.ListingId);
        Assert.Equal(GoodAddress, l.Address);
        Assert.Equal("Aarhus", l.Area);
        Assert.Equal(6500.50m, l.MonthlyRent);
        Assert.Equal(45, l.SizeM2);
        Assert.Equal(new DateTime(2026, 10, 1), l.PostedDate);
        Assert.True(l.Furnished);
    }

    [Fact]
    public void ValidOk_AllGood()
    {
        Assert.Equal("", Check());
    }

    [Fact]
    public void SetFromText_UsesBenchmarkSpelling()
    {
        var l = new Listing();
        l.SetFromText(" Main Street 1 ", "aarhus", "6500.5", "45", "2026-10-01", "yes");
        Assert.Equal("Main Street 1", l.Address);
        Assert.Equal("Aarhus", l.Area);
        Assert.Equal(6500.5m, l.MonthlyRent);
        Assert.Equal(45, l.SizeM2);
        Assert.Equal(new DateTime(2026, 10, 1), l.PostedDate);
        Assert.True(l.Furnished);
    }

    // ---------- Address: required, 1-100 characters ----------

    [Theory]
    [InlineData(0, false)]    // min-1 (blank)
    [InlineData(1, true)]     // min
    [InlineData(2, true)]     // min+1
    [InlineData(50, true)]    // mid
    [InlineData(99, true)]    // max-1
    [InlineData(100, true)]   // max
    [InlineData(101, false)]  // max+1
    [InlineData(500, false)]  // extreme
    public void Address_Boundaries(int length, bool shouldPass)
    {
        string result = Check(address: new string('a', length));
        Assert.Equal(shouldPass, result == "");
    }

    [Fact]
    public void Address_OnlySpaces_Fails()
    {
        Assert.NotEqual("", Check(address: "     "));
    }

    // ---------- Area: must be one of the benchmark cities ----------

    [Theory]
    [InlineData("Copenhagen", true)]
    [InlineData("Aarhus", true)]
    [InlineData("Odense", true)]
    [InlineData("Aalborg", true)]
    [InlineData("aarhus", true)]       // capital letters do not matter
    [InlineData(" Odense ", true)]     // spaces are trimmed
    [InlineData("", false)]            // blank
    [InlineData("Nørrebro", false)]    // a neighbourhood, not a city
    [InlineData("London", false)]      // not a benchmark city
    public void Area_Rules(string area, bool shouldPass)
    {
        Assert.Equal(shouldPass, Check(area: area) == "");
    }

    [Fact]
    public void Area_TooLong_Fails()
    {
        Assert.NotEqual("", Check(area: new string('a', 51)));
    }

    // ---------- MonthlyRent: greater than 0, max 50,000 ----------

    [Theory]
    [InlineData("0", false)]          // min-1 (0 is not allowed)
    [InlineData("0.01", true)]        // min
    [InlineData("1", true)]           // min+1
    [InlineData("25000", true)]       // mid
    [InlineData("49999.99", true)]    // max-1
    [InlineData("50000", true)]       // max
    [InlineData("50000.01", false)]   // max+1
    [InlineData("-500", false)]       // negative
    [InlineData("10000000", false)]   // extreme
    [InlineData("abc", false)]        // invalid type
    [InlineData("", false)]           // blank
    [InlineData("6,500", false)]      // comma is not allowed, use 6500 or 6500.50
    public void MonthlyRent_Boundaries(string rent, bool shouldPass)
    {
        Assert.Equal(shouldPass, Check(rent: rent) == "");
    }

    // ---------- SizeM2: whole number 5-500 ----------

    [Theory]
    [InlineData("4", false)]        // min-1
    [InlineData("5", true)]         // min
    [InlineData("6", true)]         // min+1
    [InlineData("250", true)]       // mid
    [InlineData("499", true)]       // max-1
    [InlineData("500", true)]       // max
    [InlineData("501", false)]      // max+1
    [InlineData("-5", false)]       // negative
    [InlineData("100000", false)]   // extreme
    [InlineData("abc", false)]      // invalid type
    [InlineData("45.5", false)]     // not a whole number
    public void SizeM2_Boundaries(string size, bool shouldPass)
    {
        Assert.Equal(shouldPass, Check(size: size) == "");
    }

    // ---------- PostedDate: real date, not in the future ----------

    [Theory]
    [InlineData(-3650, true)]   // extreme past (10 years ago)
    [InlineData(-1, true)]      // yesterday
    [InlineData(0, true)]       // today (max)
    [InlineData(1, false)]      // tomorrow (max+1)
    [InlineData(3650, false)]   // extreme future
    public void PostedDate_Boundaries(int daysFromToday, bool shouldPass)
    {
        string date = DateTime.Today.AddDays(daysFromToday).ToString("yyyy-MM-dd");
        Assert.Equal(shouldPass, Check(date: date) == "");
    }

    [Theory]
    [InlineData("abc")]          // invalid type
    [InlineData("2026-13-01")]   // no month 13
    [InlineData("2026-02-30")]   // no 30 February
    [InlineData("")]             // blank
    public void PostedDate_NotADate_Fails(string date)
    {
        Assert.NotEqual("", Check(date: date));
    }

    // ---------- Furnished: yes or no ----------

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", true)]
    [InlineData("yes", true)]
    [InlineData("no", true)]
    [InlineData("1", true)]
    [InlineData("0", true)]
    [InlineData("maybe", false)]
    [InlineData("", false)]
    public void Furnished_Rules(string furnished, bool shouldPass)
    {
        Assert.Equal(shouldPass, Check(furnished: furnished) == "");
    }

    // ---------- error messages ----------

    [Fact]
    public void Valid_GivesOneMessagePerWrongField()
    {
        string result = Check(rent: "abc", size: "4");
        Assert.Contains("Monthly rent must be a number.", result);
        Assert.Contains("Size must be between 5 and 500", result);
    }
}
