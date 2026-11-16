using System.Data;
using Microsoft.Data.SqlClient;

namespace RSC.Classes;

/// <summary>
/// All database operations on tblRiskCheck, each one calling a stored procedure.
/// Owner: Guru Prasad Kandel. Jira: RSC-25 to RSC-30.
/// </summary>
public class RiskCheckCollection
{
    /// <summary>ADD: saves a new risk check and returns its new RiskCheckId. Jira: RSC-25.</summary>
    public int Add(RiskCheck check)
    {
        using var conn = Open();
        using var cmd = Proc("sproc_tblRiskCheck_Insert", conn);
        AddFields(cmd, check);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    /// <summary>EDIT: updates an existing risk check. Returns false if the ID does not exist. Jira: RSC-26.</summary>
    public bool Update(RiskCheck check)
    {
        using var conn = Open();
        using var cmd = Proc("sproc_tblRiskCheck_Update", conn);
        cmd.Parameters.AddWithValue("@RiskCheckId", check.RiskCheckId);
        AddFields(cmd, check);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    /// <summary>DELETE: removes one risk check. Returns false if the ID does not exist. Jira: RSC-27.</summary>
    public bool Delete(int riskCheckId)
    {
        using var conn = Open();
        using var cmd = Proc("sproc_tblRiskCheck_Delete", conn);
        cmd.Parameters.AddWithValue("@RiskCheckId", riskCheckId);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    /// <summary>LIST: all risk checks, newest first. Jira: RSC-28.</summary>
    public List<RiskCheck> GetAll()
    {
        using var conn = Open();
        using var cmd = Proc("sproc_tblRiskCheck_SelectAll", conn);
        return ReadAll(cmd);
    }

    /// <summary>FIND: one risk check by ID, or null if not found. Jira: RSC-29.</summary>
    public RiskCheck? Find(int riskCheckId)
    {
        var check = new RiskCheck();
        return check.Find(riskCheckId) ? check : null;
    }

    /// <summary>
    /// FILTER: narrows the list. Any value left null is ignored, so filters can be combined. Jira: RSC-30.
    /// </summary>
    public List<RiskCheck> Filter(int? minScore, int? maxScore, DateTime? fromDate, DateTime? toDate, bool? userNotified)
    {
        using var conn = Open();
        using var cmd = Proc("sproc_tblRiskCheck_Filter", conn);
        cmd.Parameters.AddWithValue("@MinScore", (object?)minScore ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@MaxScore", (object?)maxScore ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FromDate", (object?)fromDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ToDate", (object?)toDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@UserNotified", (object?)userNotified ?? DBNull.Value);
        return ReadAll(cmd);
    }

    // ---------- helpers ----------

    private static SqlConnection Open()
    {
        var conn = new SqlConnection(DbSettings.ConnectionString);
        conn.Open();
        return conn;
    }

    private static SqlCommand Proc(string name, SqlConnection conn)
    {
        return new SqlCommand(name, conn) { CommandType = CommandType.StoredProcedure };
    }

    private static void AddFields(SqlCommand cmd, RiskCheck check)
    {
        cmd.Parameters.AddWithValue("@ListingId", check.ListingId);
        cmd.Parameters.AddWithValue("@RiskScore", check.RiskScore);
        cmd.Parameters.AddWithValue("@Flags", check.Flags);
        cmd.Parameters.AddWithValue("@ReviewNote", (object?)check.ReviewNote ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CheckedDate", check.CheckedDate);
        cmd.Parameters.AddWithValue("@UserNotified", check.UserNotified);
    }

    private static List<RiskCheck> ReadAll(SqlCommand cmd)
    {
        var list = new List<RiskCheck>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(RiskCheck.FromRecord(reader));
        }
        return list;
    }
}
