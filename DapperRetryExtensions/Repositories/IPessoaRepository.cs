namespace DapperRetryExtensions.Repositories;

public interface IPessoaRepository
{
    Task<IEnumerable<Pessoa>> ListarPessoasAsync(CancellationToken cancellationToken);
}
