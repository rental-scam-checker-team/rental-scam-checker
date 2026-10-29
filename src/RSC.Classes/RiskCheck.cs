using System.Globalization;

namespace RSC.Classes;

/// <summary>
/// One risk check on a rental listing (a row in tblRiskCheck).
/// Owner: Guru Prasad Kandel. Jira: RSC-25.
/// </summary>
public class RiskCheck
{
    public int RiskCheckId { get; set; }
    public int ListingId { get; set; }
    public int RiskScore { get; set; }
    public string Flags { get; set; } = string.Empty;
    public string? ReviewNote { get; set; }
    public DateTime CheckedDate { get; set; }
    public bool UserNotified { get; set; }

    // Limits shared by Valid() and the tests
    public const int MinScore = 0;
    public const int MaxScore = 100;
    public const int FlagsMaxLength = 500;
    public const int ReviewNoteMaxLength = 250;

    /// <summary>
    /// Risk level shown to the user. Worked out from RiskScore, not stored in the table.
    /// </summary>
    public string RiskLevel =>
        RiskScore >= 70 ? "High" :
        RiskScore >= 40 ? "Medium" :
        "Low";

    /// <summary>
    /// Checks the values typed into the form before Add or Edit.
    /// Inputs are strings because that is what the form sends, so bad input like "abc" can be tested.
    /// Returns "" if everything is valid, otherwise one message per field that failed.
    /// Note: whether the ListingId actually exists is checked by the database foreign key on save.
    /// </summary>
    public string Valid(string listingId, string riskScore, string flags, string? reviewNote, string checkedDate)
    {
        string error = "";

        // ListingId: must be a whole number of 1 or more
        if (!int.TryParse(listingId, out int listingIdValue) || listingIdValue < 1)
        {
            error += "Listing ID must be a whole number of 1 or more. ";
        }

        // RiskScore: whole number from 0 to 100
        if (!int.TryParse(riskScore, out int scoreValue))
        {
            error += "Risk score must be a whole number. ";
        }
        else if (scoreValue < MinScore || scoreValue > MaxScore)
        {
            error += $"Risk score must be between {MinScore} and {MaxScore}. ";
        }

        // Flags: required, 1 to 500 characters
        if (string.IsNullOrWhiteSpace(flags))
        {
            error += "Flags must not be blank. ";
        }
        else if (flags.Length > FlagsMaxLength)
        {
            error += $"Flags must be {FlagsMaxLength} characters or fewer. ";
        }

        // ReviewNote: optional, 0 to 250 characters
        if (reviewNote != null && reviewNote.Length > ReviewNoteMaxLength)
        {
            error += $"Review note must be {ReviewNoteMaxLength} characters or fewer. ";
        }

        // CheckedDate: a real date, not in the future
        if (!DateTime.TryParse(checkedDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
        {
            error += "Checked date is not a valid date. ";
        }
        else if (dateValue.Date > DateTime.Today)
        {
            error += "Checked date cannot be in the future. ";
        }

        return error.Trim();
    }
}
