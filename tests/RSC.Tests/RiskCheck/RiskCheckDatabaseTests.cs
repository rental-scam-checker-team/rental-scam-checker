using RSC.Classes;

namespace RSC.Tests.RiskCheckTests;

/// <summary>
/// Database tests for RiskCheckCollection and RiskCheck.Find. Jira: RSC-25 to RSC-30.
/// These need the local RentalScamChecker database (SQL Server Express) with ListingId 1 present.
/// Run only the fast tests with:  dotnet test --filter "Category!=Database"
/// Every test that adds a record deletes it again, so the test data stays the same.
/// </summary>
[Trait("Category", "Database")]
public class RiskCheckDatabaseTests
{
    private readonly RiskCheckCollection _all = new();

    private static RiskCheck NewCheck(int score = 42) => new()
    {
        ListingId = 1,
        RiskScore = score,
        Flags = "Unit test flag",
        ReviewNote = null,
        CheckedDate = DateTime.Today,
        UserNotified = false
    };

    [Fact]
    public void ListReturnsTestData()
    {
        Assert.NotEmpty(_all.GetAll());
    }

    [Fact]
    public void ListIsNewestFirst()
    {
        var list = _all.GetAll();
        for (int i = 1; i < list.Count; i++)
        {
            Assert.True(list[i - 1].CheckedDate >= list[i].CheckedDate);
        }
    }

    [Fact]
    public void AddThenFindReturnsSameData()
    {
        int id = _all.Add(NewCheck(77));
        try
        {
            var found = new RiskCheck();
            Assert.True(found.Find(id));
            Assert.Equal(77, found.RiskScore);
            Assert.Equal("Unit test flag", found.Flags);
            Assert.Equal(1, found.ListingId);
            Assert.Null(found.ReviewNote);
            Assert.False(found.UserNotified);
        }
        finally
        {
            _all.Delete(id);
        }
    }

    [Fact]
    public void FindMissingIdReturnsFalse()
    {
        Assert.False(new RiskCheck().Find(-1));
        Assert.Null(_all.Find(-1));
    }

    [Fact]
    public void UpdateChangesData()
    {
        int id = _all.Add(NewCheck());
        try
        {
            var check = _all.Find(id)!;
            check.RiskScore = 90;
            check.ReviewNote = "Edited by test";
            check.UserNotified = true;
            Assert.True(_all.Update(check));

            var again = _all.Find(id)!;
            Assert.Equal(90, again.RiskScore);
            Assert.Equal("Edited by test", again.ReviewNote);
            Assert.True(again.UserNotified);
        }
        finally
        {
            _all.Delete(id);
        }
    }

    [Fact]
    public void UpdateMissingIdReturnsFalse()
    {
        var check = NewCheck();
        check.RiskCheckId = -1;
        Assert.False(_all.Update(check));
    }

    [Fact]
    public void DeleteRemovesRecord()
    {
        int id = _all.Add(NewCheck());
        Assert.True(_all.Delete(id));
        Assert.Null(_all.Find(id));
    }

    [Fact]
    public void DeleteMissingIdReturnsFalse()
    {
        Assert.False(_all.Delete(-1));
    }

    [Fact]
    public void FilterByScoreRange()
    {
        var high = _all.Filter(70, 100, null, null, null);
        Assert.All(high, c => Assert.InRange(c.RiskScore, 70, 100));
    }

    [Fact]
    public void FilterByNotified()
    {
        var notified = _all.Filter(null, null, null, null, true);
        Assert.All(notified, c => Assert.True(c.UserNotified));
    }

    [Fact]
    public void FilterByDateRange()
    {
        var from = new DateTime(2026, 10, 5);
        var to = new DateTime(2026, 10, 12, 23, 59, 59);
        var inRange = _all.Filter(null, null, from, to, null);
        Assert.All(inRange, c => Assert.InRange(c.CheckedDate, from, to));
    }

    [Fact]
    public void FilterWithNoValuesReturnsAll()
    {
        Assert.Equal(_all.GetAll().Count, _all.Filter(null, null, null, null, null).Count);
    }
}
