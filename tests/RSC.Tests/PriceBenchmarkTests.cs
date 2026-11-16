
using RSC.Classes;

namespace RSC.Tests;

public class PriceBenchmarkTests
{
    [Fact]
public void ValidCity_ShouldBeAccepted()
{
    var benchmark = new PriceBenchmark
    {
        City = "Copenhagen",
        DataDate = DateTime.Today,
        SourceReference = "Bolighub"
    };

    Assert.Empty(benchmark.Valid());
}
   [Fact]
public void MissingPrivateRent_ShouldBeAllowed()
{
    var benchmark = new PriceBenchmark
    {
        City = "Copenhagen",
        PrivateRentPerM2 = null,
        SocialRentPerM2 = 93m,
        DataDate = DateTime.Today,
        SourceReference = "Bolighub"
    };

    Assert.Empty(benchmark.Valid());
}

   [Fact]
public void MissingSocialRent_ShouldBeAllowed()
{
    var benchmark = new PriceBenchmark
    {
        City = "Copenhagen",
        PrivateRentPerM2 = 225m,
        SocialRentPerM2 = null,
        DataDate = DateTime.Today,
        SourceReference = "Bolighub"
    };

    Assert.Empty(benchmark.Valid());
}
    [Fact]
    public void EmptyCity_ShouldBeRejected()
    {
        var benchmark = new PriceBenchmark
        {
            City = "",
            SourceReference = "Bolighub",
            DataDate = DateTime.Today
        };

        var result = benchmark.Valid();

        Assert.NotEmpty(result);
    }

   [Fact]
public void ZeroPrivateRent_ShouldBeRejected()
{
    var benchmark = new PriceBenchmark
    {
        City = "Copenhagen",
        PrivateRentPerM2 = 0m,
        DataDate = DateTime.Today,
        SourceReference = "Bolighub"
    };

    var result = benchmark.Valid();

    Assert.NotEmpty(result);
}
[Fact]
public void NegativePrivateRent_ShouldBeRejected()
{
    var benchmark = new PriceBenchmark
    {
        City = "Copenhagen",
        PrivateRentPerM2 = -1,
        DataDate = DateTime.Today,
        SourceReference = "Bolighub"
    };

    var result = benchmark.Valid();

    Assert.NotEmpty(result);
}
[Fact]
public void NegativeSocialRent_ShouldBeRejected()
{
    var benchmark = new PriceBenchmark
    {
        City = "Copenhagen",
        SocialRentPerM2 = -1,
        DataDate = DateTime.Today,
        SourceReference = "Bolighub"
    };

    var result = benchmark.Valid();

    Assert.NotEmpty(result);
}
[Fact]
public void MissingSourceReference_ShouldBeRejected()
{
    var benchmark = new PriceBenchmark
    {
        City = "Copenhagen",
        DataDate = DateTime.Today,
        SourceReference = ""
    };

    var result = benchmark.Valid();

    Assert.NotEmpty(result);
}
[Fact]
public void MissingDate_ShouldBeRejected()
{
    var benchmark = new PriceBenchmark
    {
        City = "Copenhagen",
        SourceReference = "Bolighub"
    };

    var result = benchmark.Valid();

    Assert.NotEmpty(result);
}
[Fact]
public void FutureDate_ShouldBeRejected()
{
    var benchmark = new PriceBenchmark
    {
        City = "Copenhagen",
        DataDate = DateTime.Today.AddDays(1),
        SourceReference = "Bolighub"
    };

    var result = benchmark.Valid();

    Assert.NotEmpty(result);
}
[Fact]
public void ValidBenchmark_ShouldBeAccepted()
{
    var benchmark = new PriceBenchmark
    {
        City = "Copenhagen",
        PrivateRentPerM2 = 225m,
        SocialRentPerM2 = 93m,
        IsActive = true,
        DataDate = DateTime.Today,
        SourceReference = "Bolighub"
    };

    var result = benchmark.Valid();

    Assert.Empty(result);
}
[Fact]
public void MissingBothRentValues_ShouldBeAllowed()
{
    var benchmark = new PriceBenchmark
    {
        City = "Copenhagen",
        PrivateRentPerM2 = null,
        SocialRentPerM2 = null,
        IsActive = true,
        DataDate = DateTime.Today,
        SourceReference = "Bolighub"
    };

    var result = benchmark.Valid();

    Assert.Empty(result);
}
[Fact]
public void ZeroSocialRent_ShouldBeRejected()
{
    var benchmark = new PriceBenchmark
    {
        City = "Copenhagen",
        SocialRentPerM2 = 0m,
        DataDate = DateTime.Today,
        SourceReference = "Landsbyggefonden"
    };

    var result = benchmark.Valid();

    Assert.NotEmpty(result);
}
}


