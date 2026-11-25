using System.Data;
using Microsoft.Data.SqlClient;

namespace RSC.Classes;

public class AdminCollection
{
    public Admin? FindByUsername(string username)
    {
        using var connection = new SqlConnection(DbSettings.ConnectionString);
        using var command = new SqlCommand("sproc_tblAdmin_FilterByUsername", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Username", username);

        connection.Open();
        using var reader = command.ExecuteReader();
        if (!reader.Read())
            return null;

        return new Admin
        {
            AdminId = reader.GetInt32(reader.GetOrdinal("AdminId")),
            Username = reader.GetString(reader.GetOrdinal("Username")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
            DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated"))
        };
    }

    public Admin? Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            return null;

        var admin = FindByUsername(username.Trim());
        if (admin == null || !admin.CheckPassword(password))
            return null;

        return admin;
    }
}
