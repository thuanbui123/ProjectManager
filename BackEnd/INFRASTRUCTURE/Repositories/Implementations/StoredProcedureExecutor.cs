using INFRASTRUCTURE.AppDbContext;
using INFRASTRUCTURE.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Reflection;

namespace INFRASTRUCTURE.Repositories.Implementations;

public class StoredProcedureExecutor : IStoredProcedureExecutor
{
    private readonly ProjectDbContext _context;
    private readonly ILogger<StoredProcedureExecutor> _logger;

    public StoredProcedureExecutor(ProjectDbContext context, ILogger<StoredProcedureExecutor> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object parameters = null) where T : new()
    {
        var results = new List<T>();

        using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = storedProcedure;
        command.CommandType = CommandType.StoredProcedure;

        // Add parameters dynamically
        if (parameters != null)
        {
            foreach (var prop in parameters.GetType().GetProperties())
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@{prop.Name}";
                parameter.Value = prop.GetValue(parameters) ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }

        try
        {
            _logger.LogInformation("Executing stored procedure: {Procedure} with parameters {@Params}", storedProcedure, parameters);

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

            _logger.LogInformation("Stored procedure {Procedure} executed successfully. Returned {Count} records.", storedProcedure, results.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure: {Procedure}", storedProcedure);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }

        return results;
    }

    public async Task<int> ExecuteAsync(string storedProcedure, object parameters = null)
    {
        using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = storedProcedure;
        command.CommandType = CommandType.StoredProcedure;

        if (parameters != null)
        {
            foreach (var prop in parameters.GetType().GetProperties())
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@{prop.Name}";
                parameter.Value = prop.GetValue(parameters) ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }

        try
        {
            _logger.LogInformation("Executing non-query stored procedure: {Procedure} with parameters {@Params}", storedProcedure, parameters);
            int affected = await command.ExecuteNonQueryAsync();
            _logger.LogInformation("Stored procedure {Procedure} affected {Count} rows.", storedProcedure, affected);
            return affected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure: {Procedure}", storedProcedure);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
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

