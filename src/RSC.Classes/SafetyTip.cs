namespace RSC.Classes;

public class SafetyTip
{
    public const int TitleMin = 1;
    public const int TitleMax = 100;
    public const int DescriptionMin = 10;
    public const int DescriptionMax = 500;
    public const int PriorityMin = 1;
    public const int PriorityMax = 5;

    public static readonly string[] Categories =
        { "Payment", "Contact", "Price", "Contract", "Viewing" };

    public int TipId { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public int Priority { get; set; }
    public bool IsPublished { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.Today;

    public string Valid(string title, string description, string category,
                        string priority, string dateAdded)
    {
        string error = "";

        title ??= "";
        if (title.Length < TitleMin)
            error += "The title may not be blank. ";
        if (title.Length > TitleMax)
            error += $"The title must be {TitleMax} characters or less. ";

        description ??= "";
        if (description.Length < DescriptionMin)
            error += $"The description must be at least {DescriptionMin} characters. ";
        if (description.Length > DescriptionMax)
            error += $"The description must be {DescriptionMax} characters or less. ";

        if (!Categories.Contains(category ?? ""))
            error += "The category must be one of: " + string.Join(", ", Categories) + ". ";

        if (!int.TryParse(priority, out int priorityValue))
            error += "The priority must be a whole number. ";
        else if (priorityValue < PriorityMin || priorityValue > PriorityMax)
            error += $"The priority must be between {PriorityMin} and {PriorityMax}. ";

        if (!DateTime.TryParse(dateAdded, out DateTime dateValue))
            error += "The date added is not a valid date. ";
        else if (dateValue.Date > DateTime.Today)
            error += "The date added cannot be in the future. ";

        return error;
    }
}