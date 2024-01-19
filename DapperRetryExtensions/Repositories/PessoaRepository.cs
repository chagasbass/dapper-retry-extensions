namespace DapperRetryExtensions.Repositories;

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
