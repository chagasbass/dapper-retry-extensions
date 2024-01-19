# dapper-retry-extensions
Extensão criada para uso do **[Retry pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/retry)** em chamadas usando dapper e a biblioteca **[Polly](https://github.com/App-vNext/Polly)**

## SqlServerTransientExceptionDetector

A classe `SqlServerTransientExceptionDetector` é responsável por verificar os erros gerados das exceções dos tipos `SqlException` e `Win32Exception`.

Os códigos de **SqlError** Mapeados são:

- **SQL Error Code: 49920**
  Cannot process request. Too many operations in progress for subscription "%ld".

- **SQL Error Code: 49919**
  Cannot process create or update request. Too many create or update operations in progress for subscription "%ld".

- **SQL Error Code: 49918**
Cannot process request. Not enough resources to process request.

- **SQL Error Code: 41839**
Transaction exceeded the maximum number of commit dependencies.

- **SQL Error Code: 41305**
The current transaction failed to commit due to a repeatable read validation failure.

- **SQL Error Code: 41302**
The current transaction attempted to update a record that has been updated since the transaction started.

- **SQL Error Code: 41301**
Dependency failure: a dependency was taken on another transaction that later failed to commit.

- **SQL Error Code: 40613**
Database XXXX on server YYYY is not currently available. Please retry the connection later.

- **SQL Error Code: 40501**
The service is currently busy. Retry the request after 10 seconds. Code: (reason code to be decoded).

- **SQL Error Code: 40197**
The service has encountered an error processing your request. Please try again.

- **SQL Error Code: 11001**
A connection attempt failed

- **SQL Error Code: 10929**
Resource ID: %d. The %s minimum guarantee is %d, maximum limit is %d and the current usage for the database is %d.
However, the server is currently too busy to support requests greater than %d for this database.
For more information, see http://go.microsoft.com/fwlink/?LinkId=267637. Otherwise, please try again.

- **SQL Error Code: 10928**
Resource ID: %d. The %s limit for the database is %d and has been reached. For more information,
see http://go.microsoft.com/fwlink/?LinkId=267637.

- **SQL Error Code: 10060**
A network-related or instance-specific error occurred while establishing a connection to SQL Server.
The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server
is configured to allow remote connections. (provider: TCP Provider, error: 0 - A connection attempt failed
because the connected party did not properly respond after a period of time, or established connection failed
because connected host has failed to respond.)"

- **SQL Error Code: 10054**
A transport-level error has occurred when sending the request to the server.

- **SQL Error Code: 10053**
A transport-level error has occurred when receiving results from the server.
An established connection was aborted by the software in your host machine.

- **SQL Error Code: 1205**
Deadlock

- **SQL Error Code: 233**
The client was unable to establish a connection because of an error during connection initialization process before login.

- **SQL Error Code: 121**
The semaphore timeout period has expired

- **SQL Error Code: 64**
A connection was successfully established with the server, but then an error occurred during the login process.

- **DBNETLIB Error Code: 20**
The instance of SQL Server you attempted to connect to does not support encryption

- **SQL Error Code : 40** A network-related or instance-specific error occurred while establishing a connection to SQL Server.The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections.
(provider: Named Pipes Provider, error: 40 - Could not open a connection to SQL Server)

- **SQL Error Code : 53** A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. 
Verify that the instance name is correct and that SQL Server is configured to allow remote connections.(provider: Named Pipes Provider, error: 40 - Could not open a connection to SQL Server)
(Microsoft SQL Server, Error : 53)

## Erros Nativos Win32Exception

- ** Error Code 80x102**

Timeout Expired

- ** Error Code 0x121**

Semaphore timeout expired

*IMPORTANTE: Caso a aplicação gere um - **SQL Error que não esteja na classe `SqlServerTransientExceptionDetector`, ele deve ser colocado para que a resiliência funcione.*

## DapperResiliencyExtensions

A classe `DapperResiliencyExtensions` é responsável pelas extensões dos métodos do Dapper que usam a resiliência do Polly.

## Métodos disponíveis

- `ExecuteAsyncWithRetry`
- `ExecuteScalarAsyncWithRetry<T>`
- `QueryAsyncWithRetry<T>`
- `QueryFirstOrDefaultAsyncWithRetry<T>`
- `QueryMultipleAsyncWithRetry`

## Exemplo de uso

```csharp
internal class PessoaRepository : IPessoaRepository
{
    const string connectionString = "SUA_CONNECTION STRING";

    public PessoaRepository() { }

    public async Task<IEnumerable<Pessoa>> ListarPessoasAsync(CancellationToken cancellationToken)
    {
        var query = "SELECT * FROM PESSOA";

        using var conexao = new SqlConnection(connectionString);

        return await conexao.QueryAsyncWithRetry<Pessoa>(query, cancellationToken);
    }
}