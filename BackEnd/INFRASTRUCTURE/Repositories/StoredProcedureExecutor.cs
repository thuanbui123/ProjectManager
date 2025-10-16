using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Reflection;

public class StoredProcedureExecutor : IStoredProcedureExecutor
{
    private readonly string _connectionString;
    private readonly ILogger<StoredProcedureExecutor> _logger;

    public StoredProcedureExecutor(IConfiguration configuration, ILogger<StoredProcedureExecutor> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object parameters = null) where T : new()
    {
        var results = new List<T>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(storedProcedure, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        // Add parameters
        if (parameters != null)
        {
            foreach (var prop in parameters.GetType().GetProperties())
            {
                command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(parameters) ?? DBNull.Value);
            }
        }

        using var reader = await command.ExecuteReaderAsync();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        while (await reader.ReadAsync())
        {
            var item = new T();
            foreach (var prop in props)
            {
                if (!reader.HasColumn(prop.Name) || reader[prop.Name] is DBNull) continue;
                prop.SetValue(item, reader[prop.Name]);
            }
            results.Add(item);
        }

        return results;
    }

    public async Task<int> ExecuteAsync(string storedProcedure, object parameters = null)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(storedProcedure, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        if (parameters != null)
        {
            foreach (var prop in parameters.GetType().GetProperties())
            {
                command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(parameters) ?? DBNull.Value);
            }
        }

        return await command.ExecuteNonQueryAsync();
    }
}


// Helper: Check if a data reader contains a specific column
internal static class DataReaderExtensions
{
    public static bool HasColumn(this IDataRecord reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}

