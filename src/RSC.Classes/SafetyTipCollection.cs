using System.Data;
using Microsoft.Data.SqlClient;

namespace RSC.Classes;

public class SafetyTipCollection
{
    private readonly string _connectionString;

    public SafetyTipCollection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public int Add(SafetyTip tip)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sproc_tblSafetyTip_Insert", connection);
        command.CommandType = CommandType.StoredProcedure;
        AddTipValues(command, tip);

        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(SafetyTip tip)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sproc_tblSafetyTip_Update", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@TipId", tip.TipId);
        AddTipValues(command, tip);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Delete(int tipId)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sproc_tblSafetyTip_Delete", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@TipId", tipId);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public List<SafetyTip> GetAll()
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sproc_tblSafetyTip_SelectAll", connection);
        command.CommandType = CommandType.StoredProcedure;

        return ReadTips(connection, command);
    }

    public SafetyTip? Find(int tipId)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sproc_tblSafetyTip_FilterByTipId", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@TipId", tipId);

        return ReadTips(connection, command).FirstOrDefault();
    }

    public List<SafetyTip> Filter(string? category, bool? isPublished)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sproc_tblSafetyTip_Filter", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Category", (object?)category ?? DBNull.Value);
        command.Parameters.AddWithValue("@IsPublished", (object?)isPublished ?? DBNull.Value);

        return ReadTips(connection, command);
    }

    private static void AddTipValues(SqlCommand command, SafetyTip tip)
    {
        command.Parameters.AddWithValue("@Title", tip.Title);
        command.Parameters.AddWithValue("@Description", tip.Description);
        command.Parameters.AddWithValue("@Category", tip.Category);
        command.Parameters.AddWithValue("@Priority", tip.Priority);
        command.Parameters.AddWithValue("@IsPublished", tip.IsPublished);
        command.Parameters.AddWithValue("@DateAdded", tip.DateAdded.Date);
    }

    private static List<SafetyTip> ReadTips(SqlConnection connection, SqlCommand command)
    {
        var tips = new List<SafetyTip>();

        connection.Open();
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            tips.Add(new SafetyTip
            {
                TipId = reader.GetInt32(reader.GetOrdinal("TipId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Priority = reader.GetInt32(reader.GetOrdinal("Priority")),
                IsPublished = reader.GetBoolean(reader.GetOrdinal("IsPublished")),
                DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded"))
            });
        }

        return tips;
    }
}