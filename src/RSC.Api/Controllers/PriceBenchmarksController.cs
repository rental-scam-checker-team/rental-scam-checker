

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
    /// Input model for adding and updating price benchmarks.
    /// Rent values accept numbers and can be null.
    /// </summary>
    public class PriceBenchmarkInput
    {
        public string City { get; set; } = "";

        public decimal? PrivateRentPerM2 { get; set; }

        public decimal? SocialRentPerM2 { get; set; }

        public bool IsActive { get; set; } = true;

        public string DataDate { get; set; } = "";

        public string SourceReference { get; set; } = "";
    }

    // GET /api/pricebenchmarks/cities
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
        var collection = new PriceBenchmarkCollection(connectionString);

        bool filtering =
            !string.IsNullOrWhiteSpace(city) ||
            isActive.HasValue;

        return filtering
            ? collection.Filter(city, isActive)
            : collection.GetAll();
    }

    // GET /api/pricebenchmarks/5
    [HttpGet("{id:int}")]
    public ActionResult<PriceBenchmark> Find(int id)
    {
        var benchmark =
            new PriceBenchmarkCollection(connectionString).Find(id);

        if (benchmark == null)
        {
            return NotFound(new
            {
                message = "No price benchmark found with that ID."
            });
        }

        return benchmark;
    }

    // POST /api/pricebenchmarks
    [HttpPost]
    public ActionResult<PriceBenchmark> Add(
        [FromBody] PriceBenchmarkInput input)
    {
        var (benchmark, errors) = BuildBenchmark(input);

        if (benchmark == null)
        {
            return BadRequest(new { message = errors });
        }

        benchmark.BenchmarkId =
            new PriceBenchmarkCollection(connectionString).Add(benchmark);

        return CreatedAtAction(
            nameof(Find),
            new { id = benchmark.BenchmarkId },
            benchmark);
    }

    // PUT /api/pricebenchmarks/5
    [HttpPut("{id:int}")]
    public ActionResult<PriceBenchmark> Update(
        int id,
        [FromBody] PriceBenchmarkInput input)
    {
        var (benchmark, errors) = BuildBenchmark(input);

        if (benchmark == null)
        {
            return BadRequest(new { message = errors });
        }

        benchmark.BenchmarkId = id;

        bool updated =
            new PriceBenchmarkCollection(connectionString).Update(benchmark);

        if (!updated)
        {
            return NotFound(new
            {
                message = "This price benchmark no longer exists."
            });
        }

        return Ok(benchmark);
    }

    // DELETE /api/pricebenchmarks/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        bool deleted =
            new PriceBenchmarkCollection(connectionString).Delete(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "This price benchmark no longer exists."
            });
        }

        return NoContent();
    }

    // ---------- HELPERS ----------

    /// <summary>
    /// Validates and converts input into a PriceBenchmark object.
    /// </summary>
    private static (PriceBenchmark? Benchmark, string Errors)
        BuildBenchmark(PriceBenchmarkInput input)
    {
        if (input == null)
        {
            return (null, "Request body is required.");
        }

        string privateRentText =
            input.PrivateRentPerM2?.ToString(CultureInfo.InvariantCulture) ?? "";

        string socialRentText =
            input.SocialRentPerM2?.ToString(CultureInfo.InvariantCulture) ?? "";

        var benchmark = new PriceBenchmark();

        string errors = benchmark.Valid(
            input.City,
            privateRentText,
            socialRentText,
            input.DataDate,
            input.SourceReference);

        if (!string.IsNullOrEmpty(errors))
        {
            return (null, errors);
        }

        benchmark.City = input.City.Trim();

        benchmark.PrivateRentPerM2 = input.PrivateRentPerM2;

        benchmark.SocialRentPerM2 = input.SocialRentPerM2;

        benchmark.IsActive = input.IsActive;

        benchmark.DataDate = DateTime.ParseExact(
            input.DataDate.Trim(),
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture);

        benchmark.SourceReference = input.SourceReference.Trim();

        return (benchmark, "");
    }
}
