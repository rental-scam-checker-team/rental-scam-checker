namespace RSC.Classes;

/// <summary>
/// One safety tip from tblSafetyTip (owner: Ambika Pandey).
/// Jira: RSC-43 (add with validation), RSC-49 (tests).
/// </summary>
public class SafetyTip
{
    // ---- Rules from the spec (Table Design) ----
    public const int TitleMin = 1;
    public const int TitleMax = 100;
    public const int DescriptionMin = 10;
    public const int DescriptionMax = 500;
    public const int PriorityMin = 1;
    public const int PriorityMax = 5;

    /// <summary>The only categories a tip can have.</summary>
    public static readonly string[] Categories =
        { "Payment", "Contact", "Price", "Contract", "Viewing" };

    // ---- Properties: one per attribute in tblSafetyTip ----
    public int TipId { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public int Priority { get; set; }
    public bool IsPublished { get; set; }          // default false = not published
    public DateTime DateAdded { get; set; } = DateTime.Today;

    /// <summary>
    /// Checks the data a user typed into the form.
    /// The values come in as text (as they do from a web form), so we can
    /// also catch the wrong type, for example "abc" for Priority.
    /// Returns "" when everything is fine, or the error messages when not.
    /// </summary>
    public string Valid(string title, string description, string category,
                        string priority, string dateAdded)
    {
        string error = "";

        // Title: required, 1-100 characters
        title ??= "";
        if (title.Length < TitleMin)
            error += "The title may not be blank. ";
        if (title.Length > TitleMax)
            error += $"The title must be {TitleMax} characters or less. ";

        // Description: required, 10-500 characters
        description ??= "";
        if (description.Length < DescriptionMin)
            error += $"The description must be at least {DescriptionMin} characters. ";
        if (description.Length > DescriptionMax)
            error += $"The description must be {DescriptionMax} characters or less. ";

        // Category: must be one of the allowed values
        if (!Categories.Contains(category ?? ""))
            error += "The category must be one of: " + string.Join(", ", Categories) + ". ";

        // Priority: must be a whole number from 1 to 5
        if (!int.TryParse(priority, out int priorityValue))
            error += "The priority must be a whole number. ";
        else if (priorityValue < PriorityMin || priorityValue > PriorityMax)
            error += $"The priority must be between {PriorityMin} and {PriorityMax}. ";

        // DateAdded: must be a real date, and not in the future
        if (!DateTime.TryParse(dateAdded, out DateTime dateValue))
            error += "The date added is not a valid date. ";
        else if (dateValue.Date > DateTime.Today)
            error += "The date added cannot be in the future. ";

        return error;
    }
}
