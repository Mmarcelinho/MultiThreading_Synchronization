namespace MultiThreadingSynchronization.src;

public static class TplAndAsyncPatterns
{
    // Exemplo de uso da TPL para paralelismo de dados
    public static void RunTplExample()
    {
        var numbers = Enumerable.Range(1, 10);
        Parallel.ForEach(numbers, number =>
        {
            Console.WriteLine($"[TPL] Processando número: {number}");
        });
        Console.WriteLine();
    }

    // Exemplo do padrão assíncrono TAP com async/await
    public static async Task RunAsyncAwaitExampleAsync()
    {
        int result = await ComputeAsync();
        Console.WriteLine($"[Async/Await] Resultado: {result}\n");
    }

    private static async Task<int> ComputeAsync()
    {
        await Task.Delay(1000); // Simula operação longa
        return 42;
    }
}

