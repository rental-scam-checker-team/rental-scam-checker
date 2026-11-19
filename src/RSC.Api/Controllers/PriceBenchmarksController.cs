
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using RSC.Classes;

namespace RSC.Api.Controllers;

/// <summary>
/// Web API for Price Benchmarks.
/// Owner: Shrayana. Jira: RSC-36.
/// </summary>
[ApiController]
[Route("api/pricebenchmarks")]
public class PriceBenchmarksController : ControllerBase
{
    private readonly string connectionString;

    public PriceBenchmarksController(IConfiguration config)
    {
        connectionString = config.GetConnectionString("RSC")
            ?? throw new InvalidOperationException(
                "Connection string 'RSC' is missing in appsettings.json.");
    }

    /// <summary>
    /// What the Add and Edit forms send.
    /// Text fields allow PriceBenchmark.Valid() to validate user input.
    /// </summary>
    public class PriceBenchmarkInput
    {
        public string City { get; set; } = "";
        public string PrivateRentPerM2 { get; set; } = "";
        public string SocialRentPerM2 { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public string DataDate { get; set; } = "";
        public string SourceReference { get; set; } = "";
    }

    // GET /api/pricebenchmarks/cities
    // Returns the cities available for price benchmarks.
    [HttpGet("cities")]
    public ActionResult<string[]> Cities()
    {
        return new[]
        {
            "Copenhagen",
            "Aarhus",
            "Odense",
            "Aalborg"
        };
    }

    // GET /api/pricebenchmarks
    // GET /api/pricebenchmarks?city=Aarhus&isActive=true
    [HttpGet]
    public ActionResult<List<PriceBenchmark>> GetAll(
        string? city,
        bool? isActive)
    {
        var benchmarks = new PriceBenchmarkCollection(connectionString);

        bool filtering =
            !string.IsNullOrWhiteSpace(city) ||
            isActive.HasValue;

        return filtering
            ? benchmarks.Filter(city, isActive)
            : benchmarks.GetAll();
    }

    // GET /api/pricebenchmarks/5
    [HttpGet("{id:int}")]
    public ActionResult<PriceBenchmark> Find(int id)
    {
        var benchmark =
            new PriceBenchmarkCollection(connectionString).Find(id);

        if (benchmark == null)
            return NotFound(new
            {
                message = "No price benchmark found with that ID."
            });

        return benchmark;
    }

    // POST /api/pricebenchmarks
    // Add a benchmark after validation.
    [HttpPost]
    public ActionResult<PriceBenchmark> Add(PriceBenchmarkInput input)
    {
        var (benchmark, errors) = BuildBenchmark(input);

        if (benchmark == null)
            return BadRequest(new { message = errors });

        benchmark.BenchmarkId =
            new PriceBenchmarkCollection(connectionString).Add(benchmark);

        return CreatedAtAction(
            nameof(Find),
            new { id = benchmark.BenchmarkId },
            benchmark);
    }

    // PUT /api/pricebenchmarks/5
    // Update an existing benchmark after validation.
    [HttpPut("{id:int}")]
    public ActionResult<PriceBenchmark> Update(
        int id,
        PriceBenchmarkInput input)
    {
        var (benchmark, errors) = BuildBenchmark(input);

        if (benchmark == null)
            return BadRequest(new { message = errors });

        benchmark.BenchmarkId = id;

        if (!new PriceBenchmarkCollection(connectionString).Update(benchmark))
            return NotFound(new
            {
                message = "This price benchmark no longer exists."
            });

        return benchmark;
    }

    // DELETE /api/pricebenchmarks/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        bool deleted =
            new PriceBenchmarkCollection(connectionString).Delete(id);

        if (!deleted)
            return NotFound(new
            {
                message = "This price benchmark no longer exists."
            });

        return NoContent();
    }

    // ---------- HELPERS ----------

    /// <summary>
    /// Validates and converts form input into a PriceBenchmark object.
    /// </summary>
    private static (PriceBenchmark? Benchmark, string Errors)
        BuildBenchmark(PriceBenchmarkInput input)
    {
        var benchmark = new PriceBenchmark();

        string errors = benchmark.Valid(
            input.City,
            input.PrivateRentPerM2,
            input.SocialRentPerM2,
            input.DataDate,
            input.SourceReference);

        if (errors != "")
            return (null, errors);

        // Convert optional rent values.
        benchmark.PrivateRentPerM2 =
            string.IsNullOrWhiteSpace(input.PrivateRentPerM2)
                ? null
                : decimal.Parse(
                    input.PrivateRentPerM2,
                    NumberStyles.AllowDecimalPoint |
                    NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture);

        benchmark.SocialRentPerM2 =
            string.IsNullOrWhiteSpace(input.SocialRentPerM2)
                ? null
                : decimal.Parse(
                    input.SocialRentPerM2,
                    NumberStyles.AllowDecimalPoint |
                    NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture);

        benchmark.City = input.City.Trim();
        benchmark.IsActive = input.IsActive;

        benchmark.DataDate = DateTime.ParseExact(
            input.DataDate.Trim(),
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture);

        benchmark.SourceReference = input.SourceReference.Trim();

        return (benchmark, "");
    }
}
