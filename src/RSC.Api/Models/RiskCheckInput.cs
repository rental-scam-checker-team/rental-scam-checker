namespace RSC.Api.Models;

/// <summary>
/// What the React form sends when adding or editing a risk check (the JSON body).
/// RiskCheckId is not here: on Add the database creates it, on Edit it comes from the URL.
/// </summary>
public class RiskCheckInput
{
    public int ListingId { get; set; }
    public int RiskScore { get; set; }
    public string Flags { get; set; } = string.Empty;
    public string? ReviewNote { get; set; }
    public DateTime CheckedDate { get; set; }
    public bool UserNotified { get; set; }
}
