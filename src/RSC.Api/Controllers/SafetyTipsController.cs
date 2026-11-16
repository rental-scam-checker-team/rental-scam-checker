using Microsoft.AspNetCore.Mvc;
using RSC.Classes;

namespace RSC.Api.Controllers;

[ApiController]
[Route("api/safetytips")]
public class SafetyTipsController : ControllerBase
{
    private readonly SafetyTipCollection _tips;

    public SafetyTipsController(IConfiguration configuration)
    {
        _tips = new SafetyTipCollection(configuration.GetConnectionString("RSC")!);
    }

    [HttpGet]
    public ActionResult<List<SafetyTip>> List([FromQuery] string? category, [FromQuery] bool? isPublished)
    {
        if (category == null && isPublished == null)
            return _tips.GetAll();

        return _tips.Filter(category, isPublished);
    }

    [HttpGet("{id:int}")]
    public ActionResult<SafetyTip> Find(int id)
    {
        var tip = _tips.Find(id);
        if (tip == null)
            return NotFound($"No safety tip with id {id}.");

        return tip;
    }

    [HttpPost]
    public ActionResult<SafetyTip> Add(SafetyTipInput input)
    {
        string error = Check(input);
        if (error != "")
            return BadRequest(error);

        var tip = ToTip(input);
        tip.TipId = _tips.Add(tip);
        return CreatedAtAction(nameof(Find), new { id = tip.TipId }, tip);
    }

    [HttpPut("{id:int}")]
    public IActionResult Edit(int id, SafetyTipInput input)
    {
        if (_tips.Find(id) == null)
            return NotFound($"No safety tip with id {id}.");

        string error = Check(input);
        if (error != "")
            return BadRequest(error);

        var tip = ToTip(input);
        tip.TipId = id;
        _tips.Update(tip);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_tips.Find(id) == null)
            return NotFound($"No safety tip with id {id}.");

        _tips.Delete(id);
        return NoContent();
    }

    private static string Check(SafetyTipInput input)
    {
        return new SafetyTip().Valid(input.Title ?? "", input.Description ?? "",
                                     input.Category ?? "", input.Priority ?? "",
                                     input.DateAdded ?? "");
    }

    private static SafetyTip ToTip(SafetyTipInput input)
    {
        return new SafetyTip
        {
            Title = input.Title!,
            Description = input.Description!,
            Category = input.Category!,
            Priority = int.Parse(input.Priority!),
            IsPublished = input.IsPublished,
            DateAdded = DateTime.Parse(input.DateAdded!)
        };
    }
}

public class SafetyTipInput
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Priority { get; set; }
    public bool IsPublished { get; set; }
    public string? DateAdded { get; set; }
}