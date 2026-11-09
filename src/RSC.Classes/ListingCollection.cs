using System.Data;
using Microsoft.Data.SqlClient;

namespace RSC.Classes;

/// <summary>
/// Data layer for tblListing: every method calls one stored procedure.
/// Owner: Aman Paudel. Jira: RSC-18 to RSC-23.
/// </summary>
public class ListingCollection
{
    private readonly string connectionString;

    public ListingCollection(string connectionString)
    {
        this.connectionString = connectionString;
    }

    /// <summary>Result of Delete, so the page can show the right message.</summary>
    public enum DeleteResult { Deleted, NotFound, HasRiskChecks }

    /// <summary>Add: saves a new listing and returns its new ListingId.</summary>
    public int Add(Listing listing)
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = Proc(conn, "dbo.sproc_tblListing_Insert");
        AddFields(cmd, listing);
        conn.Open();
        return (int)cmd.ExecuteScalar()!;
    }

    /// <summary>Edit: saves changes. Returns false if the listing does not exist.</summary>
    public bool Update(Listing listing)
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = Proc(conn, "dbo.sproc_tblListing_Update");
        cmd.Parameters.AddWithValue("@ListingId", listing.ListingId);
        AddFields(cmd, listing);
        conn.Open();
        return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
    }

    /// <summary>
    /// Delete: removes a listing, unless it has risk checks (business rule).
    /// </summary>
    public DeleteResult DeleteWithReason(int listingId)
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = Proc(conn, "dbo.sproc_tblListing_Delete");
        cmd.Parameters.AddWithValue("@ListingId", listingId);
        conn.Open();
        int result = Convert.ToInt32(cmd.ExecuteScalar());
        return result switch
        {
            -1 => DeleteResult.HasRiskChecks,
            1 => DeleteResult.Deleted,
            _ => DeleteResult.NotFound
        };
    }

    /// <summary>Delete, as in the class diagram: true only if the listing was deleted.</summary>
    public bool Delete(int listingId) => DeleteWithReason(listingId) == DeleteResult.Deleted;

    /// <summary>List: all listings, newest first.</summary>
    public List<Listing> GetAll()
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = Proc(conn, "dbo.sproc_tblListing_SelectAll");
        return ReadList(conn, cmd);
    }

    /// <summary>Find: one listing by its ID, or null if there is none.</summary>
    public Listing? Find(int listingId)
    {
        var listing = new Listing(connectionString);
        return listing.Find(listingId) ? listing : null;
    }

    /// <summary>Filter: by city, rent range and furnished. Leave a value null to ignore it.</summary>
    public List<Listing> Filter(string? area, decimal? minRent, decimal? maxRent, bool? furnished)
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = Proc(conn, "dbo.sproc_tblListing_Filter");
        cmd.Parameters.AddWithValue("@Area", string.IsNullOrWhiteSpace(area) ? DBNull.Value : area.Trim());
        cmd.Parameters.AddWithValue("@MinRent", (object?)minRent ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@MaxRent", (object?)maxRent ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Furnished", (object?)furnished ?? DBNull.Value);
        return ReadList(conn, cmd);
    }

    // ---------- helpers ----------

    private static SqlCommand Proc(SqlConnection conn, string name) =>
        new SqlCommand(name, conn) { CommandType = CommandType.StoredProcedure };

    // Parameters are typed, so user text is never pasted into SQL (stops SQL injection)
    private static void AddFields(SqlCommand cmd, Listing l)
    {
        cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 100).Value = l.Address;
        cmd.Parameters.Add("@Area", SqlDbType.NVarChar, 50).Value = l.Area;
        var rent = cmd.Parameters.Add("@MonthlyRent", SqlDbType.Decimal);
        rent.Precision = 8; rent.Scale = 2; rent.Value = l.MonthlyRent;
        cmd.Parameters.Add("@SizeM2", SqlDbType.Int).Value = l.SizeM2;
        cmd.Parameters.Add("@PostedDate", SqlDbType.Date).Value = l.PostedDate.Date;
        cmd.Parameters.Add("@Furnished", SqlDbType.Bit).Value = l.Furnished;
    }

    private List<Listing> ReadList(SqlConnection conn, SqlCommand cmd)
    {
        var list = new List<Listing>();
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var l = new Listing(connectionString);
            l.CopyFrom(reader);
            list.Add(l);
        }
        return list;
    }
}
