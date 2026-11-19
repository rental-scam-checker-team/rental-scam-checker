using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace RSC.Classes;

/// <summary>
/// Data layer for tblPriceBenchmark.
/// Every database operation calls a stored procedure.
/// Owner: Price Benchmarks. Jira: RSC-36.
/// </summary>
public class PriceBenchmarkCollection
{
    private readonly string connectionString;

    public PriceBenchmarkCollection(string connectionString)
    {
        this.connectionString = connectionString;
    }

    /// <summary>Adds a benchmark and returns its new ID.</summary>
    public int Add(PriceBenchmark benchmark)
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = Proc(conn, "dbo.sproc_tblPriceBenchmark_Insert");

        AddFields(cmd, benchmark);

        conn.Open();
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    /// <summary>
    /// Updates an existing benchmark.
    /// Returns false if the benchmark does not exist.
    /// </summary>
    public bool Update(PriceBenchmark benchmark)
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = Proc(conn, "dbo.sproc_tblPriceBenchmark_Update");

        cmd.Parameters.Add("@BenchmarkId", SqlDbType.Int)
            .Value = benchmark.BenchmarkId;

        AddFields(cmd, benchmark);

        conn.Open();
        return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
    }

    /// <summary>Deletes a benchmark using its ID.</summary>
    public bool Delete(int benchmarkId)
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = Proc(conn, "dbo.sproc_tblPriceBenchmark_Delete");

        cmd.Parameters.Add("@BenchmarkId", SqlDbType.Int)
            .Value = benchmarkId;

        conn.Open();
        return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
    }

    /// <summary>Returns all benchmarks, newest first.</summary>
    public List<PriceBenchmark> GetAll()
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = Proc(conn, "dbo.sproc_tblPriceBenchmark_SelectAll");

        return ReadList(conn, cmd);
    }

    /// <summary>
    /// Finds one benchmark by ID.
    /// Returns null if it does not exist.
    /// </summary>
    public PriceBenchmark? Find(int benchmarkId)
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = Proc(
            conn,
            "dbo.sproc_tblPriceBenchmark_FilterByBenchmarkId");

        cmd.Parameters.Add("@BenchmarkId", SqlDbType.Int)
            .Value = benchmarkId;

        conn.Open();

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return null;

        return ReadBenchmark(reader);
    }

    /// <summary>
    /// Filters benchmarks by city and active status.
    /// Null parameters are ignored.
    /// </summary>
    public List<PriceBenchmark> Filter(
        string? city,
        bool? isActive)
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = Proc(conn, "dbo.sproc_tblPriceBenchmark_Filter");

        cmd.Parameters.Add("@City", SqlDbType.NVarChar, 50)
            .Value = string.IsNullOrWhiteSpace(city)
                ? DBNull.Value
                : city.Trim();

        cmd.Parameters.Add("@IsActive", SqlDbType.Bit)
            .Value = (object?)isActive ?? DBNull.Value;

        return ReadList(conn, cmd);
    }

    // ---------------- HELPERS ----------------

    private static SqlCommand Proc(
        SqlConnection conn,
        string name)
    {
        return new SqlCommand(name, conn)
        {
            CommandType = CommandType.StoredProcedure
        };
    }

    /// <summary>Adds benchmark fields to a SQL command.</summary>
    private static void AddFields(
        SqlCommand cmd,
        PriceBenchmark benchmark)
    {
        cmd.Parameters.Add("@City", SqlDbType.NVarChar, 50)
            .Value = benchmark.City.Trim();

        var privateRent = cmd.Parameters.Add(
            "@PrivateRentPerM2", SqlDbType.Decimal);

        privateRent.Precision = 10;
        privateRent.Scale = 2;
        privateRent.Value =
            (object?)benchmark.PrivateRentPerM2 ?? DBNull.Value;

        var socialRent = cmd.Parameters.Add(
            "@SocialRentPerM2", SqlDbType.Decimal);

        socialRent.Precision = 10;
        socialRent.Scale = 2;
        socialRent.Value =
            (object?)benchmark.SocialRentPerM2 ?? DBNull.Value;

        cmd.Parameters.Add("@IsActive", SqlDbType.Bit)
            .Value = benchmark.IsActive;

        cmd.Parameters.Add("@DataDate", SqlDbType.Date)
            .Value = benchmark.DataDate.Date;

        cmd.Parameters.Add("@SourceReference", SqlDbType.NVarChar, 200)
            .Value = benchmark.SourceReference.Trim();
    }

    /// <summary>Reads all returned benchmark records.</summary>
    private List<PriceBenchmark> ReadList(
        SqlConnection conn,
        SqlCommand cmd)
    {
        var list = new List<PriceBenchmark>();

        conn.Open();

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(ReadBenchmark(reader));
        }

        return list;
    }

    /// <summary>Maps one database record into a PriceBenchmark object.</summary>
    private static PriceBenchmark ReadBenchmark(
        SqlDataReader reader)
    {
        int privateRentIndex =
            reader.GetOrdinal("PrivateRentPerM2");

        int socialRentIndex =
            reader.GetOrdinal("SocialRentPerM2");

        return new PriceBenchmark
        {
            BenchmarkId = reader.GetInt32(
                reader.GetOrdinal("BenchmarkId")),

            City = reader.GetString(
                reader.GetOrdinal("City")),

            PrivateRentPerM2 = reader.IsDBNull(privateRentIndex)
                ? (decimal?)null
                : reader.GetDecimal(privateRentIndex),

            SocialRentPerM2 = reader.IsDBNull(socialRentIndex)
                ? (decimal?)null
                : reader.GetDecimal(socialRentIndex),

            IsActive = reader.GetBoolean(
                reader.GetOrdinal("IsActive")),

            DataDate = reader.GetDateTime(
                reader.GetOrdinal("DataDate")),

            SourceReference = reader.GetString(
                reader.GetOrdinal("SourceReference"))
        };
    }
}