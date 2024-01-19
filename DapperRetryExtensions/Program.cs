// See https://aka.ms/new-console-template for more information


Console.WriteLine("Hello, World!");

try
{
    var pessoaRepository = new PessoaRepository();

    var cancellationToken = new CancellationTokenSource();

    var resultado = await pessoaRepository.ListarPessoasAsync(cancellationToken.Token);

    if (resultado.Any())
        Console.WriteLine("Retorno OK");
    else
        Console.WriteLine("Sem Retorno");

    Console.ReadKey();
}
catch (Exception ex)
{
    throw;
}
