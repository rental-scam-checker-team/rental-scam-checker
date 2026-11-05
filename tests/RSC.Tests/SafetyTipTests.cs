using RSC.Classes;

namespace RSC.Tests;

/// <summary>
/// Unit tests for the SafetyTip class (Jira RSC-49).
/// Boundary tests for every rule: extreme min, min-1, min, min+1,
/// max-1, max, max+1, mid, extreme max, invalid type.
/// </summary>
public class SafetyTipTests
{
    // ---- Good test data: every value here is valid ----
    private const string Title = "Never pay before viewing";
    private const string Description = "Do not pay a deposit before you have seen the flat.";
    private const string Category = "Payment";
    private const string Priority = "3";
    private static readonly string DateAdded = DateTime.Today.ToString("yyyy-MM-dd");

    // ==================================================================
    // Instance and properties
    // ==================================================================

    [Fact]
    public void InstanceOK()
    {
        SafetyTip tip = new SafetyTip();
        Assert.NotNull(tip);
    }

    [Fact]
    public void TipIdPropertyOK()
    {
        SafetyTip tip = new SafetyTip();
        tip.TipId = 7;
        Assert.Equal(7, tip.TipId);
    }

    [Fact]
    public void TitlePropertyOK()
    {
        SafetyTip tip = new SafetyTip();
        tip.Title = Title;
        Assert.Equal(Title, tip.Title);
    }

    [Fact]
    public void DescriptionPropertyOK()
    {
        SafetyTip tip = new SafetyTip();
        tip.Description = Description;
        Assert.Equal(Description, tip.Description);
    }

    [Fact]
    public void CategoryPropertyOK()
    {
        SafetyTip tip = new SafetyTip();
        tip.Category = Category;
        Assert.Equal(Category, tip.Category);
    }

    [Fact]
    public void PriorityPropertyOK()
    {
        SafetyTip tip = new SafetyTip();
        tip.Priority = 4;
        Assert.Equal(4, tip.Priority);
    }

    [Fact]
    public void IsPublishedPropertyOK()
    {
        SafetyTip tip = new SafetyTip();
        tip.IsPublished = true;
        Assert.True(tip.IsPublished);
    }

    [Fact]
    public void IsPublishedDefaultIsFalse()
    {
        SafetyTip tip = new SafetyTip();
        Assert.False(tip.IsPublished);
    }

    [Fact]
    public void DateAddedPropertyOK()
    {
        SafetyTip tip = new SafetyTip();
        DateTime date = new DateTime(2026, 10, 1);
        tip.DateAdded = date;
        Assert.Equal(date, tip.DateAdded);
    }

    // ==================================================================
    // Valid() with all good data
    // ==================================================================

    [Fact]
    public void ValidMethodOK()
    {
        SafetyTip tip = new SafetyTip();
        string error = tip.Valid(Title, Description, Category, Priority, DateAdded);
        Assert.Equal("", error);
    }

    // Helpers so each test only changes the one value it is testing
    private static string ValidTitle(string title) =>
        new SafetyTip().Valid(title, Description, Category, Priority, DateAdded);
    private static string ValidDescription(string description) =>
        new SafetyTip().Valid(Title, description, Category, Priority, DateAdded);
    private static string ValidCategory(string category) =>
        new SafetyTip().Valid(Title, Description, category, Priority, DateAdded);
    private static string ValidPriority(string priority) =>
        new SafetyTip().Valid(Title, Description, Category, priority, DateAdded);
    private static string ValidDate(string date) =>
        new SafetyTip().Valid(Title, Description, Category, Priority, date);

    // ==================================================================
    // Title: 1-100 characters
    // ==================================================================

    [Fact] public void TitleMinLessOne() => Assert.NotEqual("", ValidTitle(""));                    // 0
    [Fact] public void TitleMin() => Assert.Equal("", ValidTitle(new string('a', 1)));              // 1
    [Fact] public void TitleMinPlusOne() => Assert.Equal("", ValidTitle(new string('a', 2)));       // 2
    [Fact] public void TitleMaxLessOne() => Assert.Equal("", ValidTitle(new string('a', 99)));      // 99
    [Fact] public void TitleMax() => Assert.Equal("", ValidTitle(new string('a', 100)));            // 100
    [Fact] public void TitleMaxPlusOne() => Assert.NotEqual("", ValidTitle(new string('a', 101)));  // 101
    [Fact] public void TitleMid() => Assert.Equal("", ValidTitle(new string('a', 50)));             // 50
    [Fact] public void TitleExtremeMax() => Assert.NotEqual("", ValidTitle(new string('a', 1000))); // 1000

    // ==================================================================
    // Description: 10-500 characters
    // ==================================================================

    [Fact] public void DescriptionExtremeMin() => Assert.NotEqual("", ValidDescription(""));                   // 0
    [Fact] public void DescriptionMinLessOne() => Assert.NotEqual("", ValidDescription(new string('a', 9)));   // 9
    [Fact] public void DescriptionMin() => Assert.Equal("", ValidDescription(new string('a', 10)));            // 10
    [Fact] public void DescriptionMinPlusOne() => Assert.Equal("", ValidDescription(new string('a', 11)));     // 11
    [Fact] public void DescriptionMaxLessOne() => Assert.Equal("", ValidDescription(new string('a', 499)));    // 499
    [Fact] public void DescriptionMax() => Assert.Equal("", ValidDescription(new string('a', 500)));           // 500
    [Fact] public void DescriptionMaxPlusOne() => Assert.NotEqual("", ValidDescription(new string('a', 501))); // 501
    [Fact] public void DescriptionMid() => Assert.Equal("", ValidDescription(new string('a', 255)));           // 255
    [Fact] public void DescriptionExtremeMax() => Assert.NotEqual("", ValidDescription(new string('a', 5000)));// 5000

    // ==================================================================
    // Category: Payment, Contact, Price, Contract, Viewing
    // ==================================================================

    [Theory]
    [InlineData("Payment")]
    [InlineData("Contact")]
    [InlineData("Price")]
    [InlineData("Contract")]
    [InlineData("Viewing")]
    public void CategoryAllowedValues(string category) => Assert.Equal("", ValidCategory(category));

    [Fact] public void CategoryBlank() => Assert.NotEqual("", ValidCategory(""));
    [Fact] public void CategoryNotInList() => Assert.NotEqual("", ValidCategory("Deposit"));
    [Fact] public void CategoryWrongCase() => Assert.NotEqual("", ValidCategory("payment"));

    // ==================================================================
    // Priority: whole number 1-5
    // ==================================================================

    [Fact] public void PriorityExtremeMin() => Assert.NotEqual("", ValidPriority("-1000"));
    [Fact] public void PriorityMinLessOne() => Assert.NotEqual("", ValidPriority("0"));
    [Fact] public void PriorityMin() => Assert.Equal("", ValidPriority("1"));
    [Fact] public void PriorityMinPlusOne() => Assert.Equal("", ValidPriority("2"));
    [Fact] public void PriorityMid() => Assert.Equal("", ValidPriority("3"));
    [Fact] public void PriorityMaxLessOne() => Assert.Equal("", ValidPriority("4"));
    [Fact] public void PriorityMax() => Assert.Equal("", ValidPriority("5"));
    [Fact] public void PriorityMaxPlusOne() => Assert.NotEqual("", ValidPriority("6"));
    [Fact] public void PriorityExtremeMax() => Assert.NotEqual("", ValidPriority("1000"));
    [Fact] public void PriorityInvalidType() => Assert.NotEqual("", ValidPriority("abc"));
    [Fact] public void PriorityNotWholeNumber() => Assert.NotEqual("", ValidPriority("2.5"));

    // ==================================================================
    // DateAdded: a real date, not in the future
    // ==================================================================

    private static string Days(int offset) => DateTime.Today.AddDays(offset).ToString("yyyy-MM-dd");

    [Fact] public void DateAddedExtremeMin() => Assert.Equal("", ValidDate(Days(-365 * 100)));   // 100 years ago
    [Fact] public void DateAddedMaxLessOne() => Assert.Equal("", ValidDate(Days(-1)));           // yesterday
    [Fact] public void DateAddedMax() => Assert.Equal("", ValidDate(Days(0)));                   // today
    [Fact] public void DateAddedMaxPlusOne() => Assert.NotEqual("", ValidDate(Days(1)));         // tomorrow
    [Fact] public void DateAddedExtremeMax() => Assert.NotEqual("", ValidDate(Days(365 * 100))); // 100 years ahead
    [Fact] public void DateAddedInvalidType() => Assert.NotEqual("", ValidDate("this is not a date"));
    [Fact] public void DateAddedBlank() => Assert.NotEqual("", ValidDate(""));
}
