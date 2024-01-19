using Dapper;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using System.ComponentModel;
using System.Data;

namespace DapperRetryExtensions.Extensions;

public static class DapperResiliencyExtensions
{
    private const int RetryCount = 2;

    private static readonly Random Random = new Random();

    private static readonly AsyncRetryPolicy RetryPolicy = Policy
        .Handle<SqlException>(SqlServerTransientExceptionDetector.ShouldRetryOn)
        .Or<TimeoutException>()
        .OrInner<Win32Exception>(SqlServerTransientExceptionDetector.ShouldRetryOn)
        .WaitAndRetryAsync(
            RetryCount,
            currentRetryNumber => TimeSpan.FromSeconds(Math.Pow(1.5, currentRetryNumber - 1)) + TimeSpan.FromMilliseconds(Random.Next(0, 10)),
            (currentException, currentSleepDuration, currentRetryNumber, currentContext) =>
            {
                Console.WriteLine($"=== Tentativa {currentRetryNumber} ===");
                Console.WriteLine($"{nameof(currentException)} {currentException.Message}");
                //Console.WriteLine(nameof(currentContext) + ": " + currentContext);
                Console.WriteLine(nameof(currentSleepDuration) + ": " + currentSleepDuration);

            });

    /// <summary>
    /// Wraps ExecuteAsync with retry policy.
    /// </summary>
    /// <param name="cnn">The db connection</param>
    /// <param name="sql">The sql query</param>
    /// <param name="param">The sql query parameters</param>
    /// <param name="transaction">The db transaction</param>
    /// <param name="commandTimeout">The command timeout value</param>
    /// <param name="commandType">The command type</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public static Task<int> ExecuteAsyncWithRetry(
        this IDbConnection cnn,
        string sql,
        CancellationToken cancellationToken,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null) => RetryPolicy.ExecuteAsync(() => cnn.ExecuteAsync(
            new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken)));

    /// <summary>
    /// Wraps ExecuteScalarAsync with retry policy.
    /// </summary>
    /// <typeparam name="T">The generic type of returning object</typeparam>
    /// <param name="cnn">The db connection</param>
    /// <param name="sql">The sql query</param>
    /// <param name="param">The sql query parameters</param>
    /// <param name="transaction">The db transaction</param>
    /// <param name="commandTimeout">The command timeout value</param>
    /// <param name="commandType">The command type</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public static Task<T?> ExecuteScalarAsyncWithRetry<T>(
        this IDbConnection cnn,
        string sql,
        CancellationToken cancellationToken,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null) => RetryPolicy.ExecuteAsync(() =>
        {
            return cnn.ExecuteScalarAsync<T>(
                new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
        });

    /// <summary>
    /// Wraps QueryAsync with retry policy.
    /// </summary>
    /// <typeparam name="T">The generic type of returning object</typeparam>
    /// <param name="cnn">The db connection</param>
    /// <param name="sql">The sql query</param>
    /// <param name="param">The sql query parameters</param>
    /// <param name="transaction">The db transaction</param>
    /// <param name="commandTimeout">The command timeout value</param>
    /// <param name="commandType">The command type</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<IEnumerable<T>> QueryAsyncWithRetry<T>(
             this IDbConnection cnn,
             string sql,
             CancellationToken cancellationToken,
             object? param = null,
             IDbTransaction? transaction = null,
             int? commandTimeout = null,
             CommandType? commandType = null) => RetryPolicy.ExecuteAsync(() =>
             {
                 return cnn.QueryAsync<T>(
                 new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));

             });

    /// <summary>
    /// Wraps QueryFirstOrDefaultAsync with retry policy.
    /// </summary>
    /// <typeparam name="T">The generic type of returning object</typeparam>
    /// <param name="cnn">The db connection</param>
    /// <param name="sql">The sql query</param>
    /// <param name="param">The sql query parameters</param>
    /// <param name="transaction">The db transaction</param>
    /// <param name="commandTimeout">The command timeout value</param>
    /// <param name="commandType">The command type</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<T?> QueryFirstOrDefaultAsyncWithRetry<T>(
        this IDbConnection cnn,
        string sql,
        CancellationToken cancellationToken,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null) => RetryPolicy.ExecuteAsync(() =>
        {
            return cnn.QueryFirstOrDefaultAsync<T>(
                new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
        });

    /// <summary>
    /// Wraps QueryMultipleAsync with retry policy.
    /// </summary>
    /// <param name="cnn">The db connection</param>
    /// <param name="sql">The sql query</param>
    /// <param name="param">The sql query parameters</param>
    /// <param name="transaction">The db transaction</param>
    /// <param name="commandTimeout">The command timeout value</param>
    /// <param name="commandType">The command type</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<SqlMapper.GridReader> QueryMultipleAsyncWithRetry(
        this IDbConnection cnn,
        string sql,
        CancellationToken cancellationToken,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null) => RetryPolicy.ExecuteAsync(() =>
        {
            return cnn.QueryMultipleAsync(
                new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken));
        });
}
