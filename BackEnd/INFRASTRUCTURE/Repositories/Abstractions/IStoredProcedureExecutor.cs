namespace INFRASTRUCTURE.Repositories.Abstractions;

public interface IStoredProcedureExecutor
{
    /// <summary>
    /// Executes a stored procedure and maps the result to a list of custom model objects.
    /// </summary>
    Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object parameters = null) where T : new();

    /// <summary>
    /// Executes a stored procedure for insert/update/delete that does not return results.
    /// </summary>
    Task<int> ExecuteAsync(string storedProcedure, object parameters = null);
}
