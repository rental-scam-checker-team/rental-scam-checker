using Microsoft.AspNetCore.Mvc;
using RSC.Classes;

namespace RSC.Api.Controllers;

/// <summary>
/// Web API for Listings (owner: Aman Paudel). The React pages call these URLs.
/// Jira: RSC-18 Add, RSC-19 Edit, RSC-20 Delete, RSC-21 List, RSC-22 Find, RSC-23 Filter.
/// </summary>
[ApiController]
[Route("api/listings")]
public class ListingsController : ControllerBase
{
    private readonly string connectionString;

    public ListingsController(IConfiguration config)
    {
        connectionString = config.GetConnectionString("RSC")
            ?? throw new InvalidOperationException("Connection string 'RSC' is missing in appsettings.json.");
    }

    /// <summary>What the Add and Edit forms send. Everything is text, so Valid() can check it.</summary>
    public class ListingInput
    {
        public string Address { get; set; } = "";
        public string Area { get; set; } = "";
        public string MonthlyRent { get; set; } = "";
        public string SizeM2 { get; set; } = "";
        public string PostedDate { get; set; } = "";
        public string Furnished { get; set; } = "";
    }

    // GET /api/listings/cities -> the cities the user can choose from
    [HttpGet("cities")]
    public ActionResult<string[]> Cities() => Listing.AllowedCities;

    // GET /api/listings                       -> List (all listings, newest first)
    // GET /api/listings?area=Aarhus&maxRent=7000&furnished=true -> Filter
    [HttpGet]
    public ActionResult<List<Listing>> GetAll(string? area, decimal? minRent, decimal? maxRent, bool? furnished)
    {
        if (minRent.HasValue && maxRent.HasValue && minRent > maxRent)
            return BadRequest(new { message = "Minimum rent cannot be higher than maximum rent." });

        var listings = new ListingCollection(connectionString);
        bool filtering = !string.IsNullOrWhiteSpace(area) || minRent.HasValue || maxRent.HasValue || furnished.HasValue;
        return filtering ? listings.Filter(area, minRent, maxRent, furnished) : listings.GetAll();
    }

    // GET /api/listings/5 -> Find
    [HttpGet("{id:int}")]
    public ActionResult<Listing> Find(int id)
    {
        var listing = new ListingCollection(connectionString).Find(id);
        if (listing == null) return NotFound(new { message = "No listing found with that ID." });
        return listing;
    }

    // POST /api/listings -> Add (with Validate)
    [HttpPost]
    public ActionResult<Listing> Add(ListingInput input)
    {
        var listing = new Listing();
        string errors = listing.Valid(input.Address, input.Area, input.MonthlyRent, input.SizeM2,
                                      input.PostedDate, input.Furnished);
        if (errors != "") return BadRequest(new { message = errors });

        listing.SetFromText(input.Address, input.Area, input.MonthlyRent, input.SizeM2,
                            input.PostedDate, input.Furnished);
        listing.ListingId = new ListingCollection(connectionString).Add(listing);
        return CreatedAtAction(nameof(Find), new { id = listing.ListingId }, listing);
    }

    // PUT /api/listings/5 -> Edit (with Validate). Admin only: add the login check when the shared login is ready.
    [HttpPut("{id:int}")]
    public ActionResult<Listing> Update(int id, ListingInput input)
    {
        var listing = new Listing();
        string errors = listing.Valid(input.Address, input.Area, input.MonthlyRent, input.SizeM2,
                                      input.PostedDate, input.Furnished);
        if (errors != "") return BadRequest(new { message = errors });

        listing.SetFromText(input.Address, input.Area, input.MonthlyRent, input.SizeM2,
                            input.PostedDate, input.Furnished);
        listing.ListingId = id;
        if (!new ListingCollection(connectionString).Update(listing))
            return NotFound(new { message = "This listing no longer exists." });
        return listing;
    }

    // DELETE /api/listings/5 -> Delete. Admin only: add the login check when the shared login is ready.
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var result = new ListingCollection(connectionString).DeleteWithReason(id);
        return result switch
        {
            ListingCollection.DeleteResult.Deleted => NoContent(),
            ListingCollection.DeleteResult.HasRiskChecks =>
                Conflict(new { message = "This listing has risk checks and cannot be deleted." }),
            _ => NotFound(new { message = "This listing no longer exists." })
        };
    }
}
