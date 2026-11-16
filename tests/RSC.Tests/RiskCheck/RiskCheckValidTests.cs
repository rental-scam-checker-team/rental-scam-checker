using RSC.Classes;

namespace RSC.Tests.RiskCheckTests;

/// <summary>
/// Boundary tests for RiskCheck.Valid(). Jira: RSC-25.
/// Each test changes ONE field and keeps the others valid,
/// so a failing test points at exactly one rule.
/// </summary>
public class RiskCheckValidTests
{
    // Good values for every field
    private const string GoodListingId = "1";
    private const string GoodScore = "50";
    private const string GoodFlags = "Rent 55% below area average";
    private const string GoodNote = "Checked by admin";
    private static string Today => DateTime.Today.ToString("yyyy-MM-dd");

    private static string Valid(
        string listingId = GoodListingId,
        string riskScore = GoodScore,
        string flags = GoodFlags,
        string? reviewNote = GoodNote,
        string? checkedDate = null)
    {
        return new RiskCheck().Valid(listingId, riskScore, flags, reviewNote, checkedDate ?? Today);
    }

    // ---------- All valid ----------

    [Fact]
    public void ValidDataOK()
    {
        Assert.Equal("", Valid());
    }

    // ---------- ListingId ----------

    [Fact]
    public void ListingIdMinOK() => Assert.Equal("", Valid(listingId: "1"));

    [Fact]
    public void ListingIdZeroFails() => Assert.NotEqual("", Valid(listingId: "0"));

    [Fact]
    public void ListingIdNegativeFails() => Assert.NotEqual("", Valid(listingId: "-1"));

    [Fact]
    public void ListingIdBlankFails() => Assert.NotEqual("", Valid(listingId: ""));

    [Fact]
    public void ListingIdTextFails() => Assert.NotEqual("", Valid(listingId: "abc"));

    // ---------- RiskScore (0-100) ----------

    [Fact]
    public void RiskScoreMinLessOneFails() => Assert.NotEqual("", Valid(riskScore: "-1"));

    [Fact]
    public void RiskScoreMinOK() => Assert.Equal("", Valid(riskScore: "0"));

    [Fact]
    public void RiskScoreMinPlusOneOK() => Assert.Equal("", Valid(riskScore: "1"));

    [Fact]
    public void RiskScoreMidOK() => Assert.Equal("", Valid(riskScore: "50"));

    [Fact]
    public void RiskScoreMaxLessOneOK() => Assert.Equal("", Valid(riskScore: "99"));

    [Fact]
    public void RiskScoreMaxOK() => Assert.Equal("", Valid(riskScore: "100"));

    [Fact]
    public void RiskScoreMaxPlusOneFails() => Assert.NotEqual("", Valid(riskScore: "101"));

    [Fact]
    public void RiskScoreExtremeMaxFails() => Assert.NotEqual("", Valid(riskScore: "100000"));

    [Fact]
    public void RiskScoreTextFails() => Assert.NotEqual("", Valid(riskScore: "high"));

    [Fact]
    public void RiskScoreBlankFails() => Assert.NotEqual("", Valid(riskScore: ""));

    // ---------- Flags (1-500 characters) ----------

    [Fact]
    public void FlagsBlankFails() => Assert.NotEqual("", Valid(flags: ""));

    [Fact]
    public void FlagsMinOK() => Assert.Equal("", Valid(flags: "a"));

    [Fact]
    public void FlagsMaxLessOneOK() => Assert.Equal("", Valid(flags: new string('a', 499)));

    [Fact]
    public void FlagsMaxOK() => Assert.Equal("", Valid(flags: new string('a', 500)));

    [Fact]
    public void FlagsMaxPlusOneFails() => Assert.NotEqual("", Valid(flags: new string('a', 501)));

    [Fact]
    public void FlagsMidOK() => Assert.Equal("", Valid(flags: new string('a', 250)));

    // ---------- ReviewNote (0-250 characters, optional) ----------

    [Fact]
    public void ReviewNoteNullOK() => Assert.Equal("", Valid(reviewNote: null));

    [Fact]
    public void ReviewNoteEmptyOK() => Assert.Equal("", Valid(reviewNote: ""));

    [Fact]
    public void ReviewNoteMaxLessOneOK() => Assert.Equal("", Valid(reviewNote: new string('a', 249)));

    [Fact]
    public void ReviewNoteMaxOK() => Assert.Equal("", Valid(reviewNote: new string('a', 250)));

    [Fact]
    public void ReviewNoteMaxPlusOneFails() => Assert.NotEqual("", Valid(reviewNote: new string('a', 251)));

    // ---------- CheckedDate (valid date, not in the future) ----------

    [Fact]
    public void CheckedDateTodayOK() => Assert.Equal("", Valid(checkedDate: Today));

    [Fact]
    public void CheckedDateYesterdayOK() =>
        Assert.Equal("", Valid(checkedDate: DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd")));

    [Fact]
    public void CheckedDateTomorrowFails() =>
        Assert.NotEqual("", Valid(checkedDate: DateTime.Today.AddDays(1).ToString("yyyy-MM-dd")));

    [Fact]
    public void CheckedDateExtremeMinOK() =>
        Assert.Equal("", Valid(checkedDate: DateTime.Today.AddYears(-100).ToString("yyyy-MM-dd")));

    [Fact]
    public void CheckedDateInvalidFails() => Assert.NotEqual("", Valid(checkedDate: "not a date"));

    [Fact]
    public void CheckedDateImpossibleFails() => Assert.NotEqual("", Valid(checkedDate: "2026-02-30"));

    // ---------- Risk level ----------

    [Theory]
    [InlineData(0, "Low")]
    [InlineData(39, "Low")]
    [InlineData(40, "Medium")]
    [InlineData(69, "Medium")]
    [InlineData(70, "High")]
    [InlineData(100, "High")]
    public void RiskLevelBoundaries(int score, string expected)
    {
        var check = new RiskCheck { RiskScore = score };
        Assert.Equal(expected, check.RiskLevel);
    }
}
