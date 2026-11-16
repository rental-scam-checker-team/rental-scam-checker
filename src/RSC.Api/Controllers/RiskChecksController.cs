using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RSC.Api.Models;
using RSC.Classes;

namespace RSC.Api.Controllers;

/// <summary>
/// Web API for risk checks. The React pages call these URLs; each one uses the middle layer
/// (RiskCheck / RiskCheckCollection), which runs the stored procedures.
/// Owner: Guru Prasad Kandel. Jira: RSC-25 to RSC-30.
/// </summary>
[ApiController]
[Route("api/riskchecks")]
public class RiskChecksController : ControllerBase
{
    private const int ForeignKeyViolation = 547;
    private readonly RiskCheckCollection _all = new();

    /// <summary>
    /// LIST and FILTER. With no query values it returns every check, newest first.
    /// Example filter: GET /api/riskchecks?minScore=70&amp;notified=false
    /// </summary>
    [HttpGet]
    public ActionResult<List<RiskCheck>> List(
        [FromQuery] int? minScore, [FromQuery] int? maxScore,
        [FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] bool? notified)
    {
        bool filtering = minScore.HasValue || maxScore.HasValue || from.HasValue || to.HasValue || notified.HasValue;
        return filtering
            ? _all.Filter(minScore, maxScore, from, to, notified)
            : _all.GetAll();
    }

    /// <summary>FIND: GET /api/riskchecks/5</summary>
    [HttpGet("{id:int}")]
    public ActionResult<RiskCheck> Find(int id)
    {
        var check = _all.Find(id);
        if (check == null)
        {
            return NotFound(new { error = $"No risk check with ID {id}." });
        }
        return check;
    }

    /// <summary>ADD: POST /api/riskchecks with the form data as JSON. Validates first.</summary>
    [HttpPost]
    public ActionResult<RiskCheck> Add(RiskCheckInput input)
    {
        string error = Validate(input);
        if (error != "")
        {
            return BadRequest(new { error });
        }

        try
        {
            int newId = _all.Add(ToRiskCheck(input, 0));
            return CreatedAtAction(nameof(Find), new { id = newId }, _all.Find(newId));
        }
        catch (SqlException ex) when (ex.Number == ForeignKeyViolation)
        {
            return BadRequest(new { error = $"Listing {input.ListingId} does not exist." });
        }
    }

    /// <summary>EDIT: PUT /api/riskchecks/5 with the form data as JSON. Validates first.</summary>
    [HttpPut("{id:int}")]
    public IActionResult Edit(int id, RiskCheckInput input)
    {
        string error = Validate(input);
        if (error != "")
        {
            return BadRequest(new { error });
        }

        try
        {
            if (!_all.Update(ToRiskCheck(input, id)))
            {
                return NotFound(new { error = $"No risk check with ID {id}." });
            }
            return NoContent();
        }
        catch (SqlException ex) when (ex.Number == ForeignKeyViolation)
        {
            return BadRequest(new { error = $"Listing {input.ListingId} does not exist." });
        }
    }

    /// <summary>DELETE: DELETE /api/riskchecks/5 (the React page asks the user to confirm first).</summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!_all.Delete(id))
        {
            return NotFound(new { error = $"No risk check with ID {id}." });
        }
        return NoContent();
    }

    // ---------- helpers ----------

    /// <summary>VALIDATE: runs RiskCheck.Valid() on the incoming data.</summary>
    private static string Validate(RiskCheckInput input)
    {
        return new RiskCheck().Valid(
            input.ListingId.ToString(),
            input.RiskScore.ToString(),
            input.Flags,
            input.ReviewNote,
            input.CheckedDate.ToString("yyyy-MM-dd"));
    }

    private static RiskCheck ToRiskCheck(RiskCheckInput input, int id) => new()
    {
        RiskCheckId = id,
        ListingId = input.ListingId,
        RiskScore = input.RiskScore,
        Flags = input.Flags,
        ReviewNote = string.IsNullOrWhiteSpace(input.ReviewNote) ? null : input.ReviewNote,
        CheckedDate = input.CheckedDate,
        UserNotified = input.UserNotified
    };
}
