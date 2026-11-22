using RSC.Classes;
using Xunit;

namespace RSC.Tests.RiskCheckTests;

/// <summary>
/// Boundary tests for the price check (Jira RSC-31). No database needed:
/// PriceCheck.Calculate() only uses the numbers it is given.
/// Benchmark used: 200 kr/m², so 1% = 2 kr/m².
/// </summary>
public class PriceCheckTests
{
    private const decimal Benchmark = 200m;

    // Rent for a 100 m² flat that is the given % below a 200 kr/m² benchmark
    private static PriceCheckResult CheckPercentBelow(decimal percentBelow) =>
        PriceCheck.Calculate(1, "Copenhagen", (Benchmark * (100m - percentBelow) / 100m) * 100m, 100, Benchmark);

    // ---------- Score rules at their boundaries ----------

    [Fact] public void AboveBenchmarkIsNormal() => Assert.Equal(PriceCheck.NormalScore, CheckPercentBelow(-20).SuggestedScore);
    [Fact] public void ExactlyBenchmarkIsNormal() => Assert.Equal(PriceCheck.NormalScore, CheckPercentBelow(0).SuggestedScore);
    [Fact] public void JustUnder30IsNormal() => Assert.Equal(PriceCheck.NormalScore, CheckPercentBelow(29.5m).SuggestedScore);
    [Fact] public void Exactly30IsMedium() => Assert.Equal(PriceCheck.SlightlyLowScore, CheckPercentBelow(30).SuggestedScore);
    [Fact] public void JustUnder40IsMedium() => Assert.Equal(PriceCheck.SlightlyLowScore, CheckPercentBelow(39.5m).SuggestedScore);
    [Fact] public void Exactly40IsHigh() => Assert.Equal(PriceCheck.LowScore, CheckPercentBelow(40).SuggestedScore);
    [Fact] public void JustUnder60IsHigh() => Assert.Equal(PriceCheck.LowScore, CheckPercentBelow(59.5m).SuggestedScore);
    [Fact] public void Exactly60IsVeryHigh() => Assert.Equal(PriceCheck.VeryLowScore, CheckPercentBelow(60).SuggestedScore);
    [Fact] public void ExtremeCheapIsVeryHigh() => Assert.Equal(PriceCheck.VeryLowScore, CheckPercentBelow(99).SuggestedScore);

    // ---------- Suggested score gives the right risk level ----------

    [Fact]
    public void SuggestedScoresGiveExpectedLevels()
    {
        Assert.Equal("Low", new RiskCheck { RiskScore = PriceCheck.NormalScore }.RiskLevel);
        Assert.Equal("Medium", new RiskCheck { RiskScore = PriceCheck.SlightlyLowScore }.RiskLevel);
        Assert.Equal("High", new RiskCheck { RiskScore = PriceCheck.LowScore }.RiskLevel);
        Assert.Equal("High", new RiskCheck { RiskScore = PriceCheck.VeryLowScore }.RiskLevel);
    }

    // ---------- Calculations ----------

    [Fact]
    public void RentPerM2AndPercentAreCorrect()
    {
        // Amagerbrogade test listing: 3200 kr, 70 m², Copenhagen benchmark 225 kr/m²
        var r = PriceCheck.Calculate(2, "Copenhagen", 3200m, 70, 225m);
        Assert.Equal(45.71m, r.ListingRentPerM2);
        Assert.Equal(79.7m, r.PercentBelow);
        Assert.Equal(PriceCheck.VeryLowScore, r.SuggestedScore);
        Assert.Contains("below the Copenhagen average", r.Flag);
    }

    [Fact]
    public void NormalRentFlagSaysNormal()
    {
        var r = PriceCheck.Calculate(1, "Copenhagen", 9500m, 55, 225m);
        Assert.Contains("normal range", r.Flag);
    }

    // ---------- Missing or bad data ----------

    [Fact]
    public void NoBenchmarkGivesNormalScoreAndMessage()
    {
        var r = PriceCheck.Calculate(1, "Odense", 5000m, 50, null);
        Assert.Equal(PriceCheck.NormalScore, r.SuggestedScore);
        Assert.Null(r.PercentBelow);
        Assert.Contains("No active price benchmark", r.Flag);
    }

    [Fact]
    public void ZeroSizeDoesNotCrash()
    {
        var r = PriceCheck.Calculate(1, "Odense", 5000m, 0, 120m);
        Assert.Equal(PriceCheck.NormalScore, r.SuggestedScore);
        Assert.Contains("not possible", r.Flag);
    }

    [Fact]
    public void ZeroRentDoesNotCrash()
    {
        var r = PriceCheck.Calculate(1, "Odense", 0m, 50, 120m);
        Assert.Equal(PriceCheck.NormalScore, r.SuggestedScore);
    }
}
