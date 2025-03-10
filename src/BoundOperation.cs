namespace MultiThreadingSynchronization.src;

/// <summary>
/// Demonstra a diferença entre operações CPU-bound e I/O-bound.
/// 
/// CPU-bound: Operações que dependem principalmente do processador para completar
/// I/O-bound: Operações que dependem principalmente de entrada/saída e têm tempos de espera
/// </summary>
public static class BoundOperation
{
    /// <summary>
    /// Executa uma operação CPU-bound usando uma Task.
    /// Operações CPU-bound são melhor executadas em threads separadas 
    /// para não bloquear a thread principal e aproveitar múltiplos núcleos.
    /// </summary>
    public static void RunCpuBoundExample()
    {
        Console.WriteLine("[CPU-bound] Iniciando cálculo intensivo de CPU...");

        // Usando Task para executar operação intensiva de CPU em thread separada
        Task.Run(() =>
        {
            // Simulando operação intensiva de CPU com cálculo de soma
            var result = Enumerable.Range(1, 10_000_000).Sum(x => (long)x);
            Console.WriteLine($"[CPU-bound] Soma calculada: {result}");
        }).Wait(); // Aguarda a conclusão da tarefa

        // Exemplo alternativo com Parallel.For para paralelizar cálculos
        RunParallelCpuBoundExample();

        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra como paralelizar operações CPU-bound para melhor desempenho.
    /// Usando Parallel.For para dividir o trabalho entre múltiplos threads.
    /// </summary>
    private static void RunParallelCpuBoundExample()
    {
        Console.WriteLine("[CPU-bound] Iniciando cálculo paralelo...");

        long totalSum = 0;
        var rangeSize = 1_000_000;
        var rangeCount = 10;

        // Usando Parallel.For para dividir o trabalho em múltiplos threads
        Parallel.For(0, rangeCount, i =>
        {
            var rangeStart = i * rangeSize + 1;
            var rangeSum = Enumerable.Range(rangeStart, rangeSize).Sum(x => (long)x);

            // Incremento atômico para evitar condições de corrida
            Interlocked.Add(ref totalSum, rangeSum);
        });

        Console.WriteLine($"[CPU-bound] Soma paralela calculada: {totalSum}");
    }

    /// <summary>
    /// Executa uma operação I/O-bound usando async/await.
    /// Operações I/O-bound são ideais para async/await porque permitem
    /// que a thread seja liberada durante a espera da operação I/O.
    /// </summary>
    public static async Task RunIoBoundExampleAsync()
    {
        Console.WriteLine("[I/O-bound] Iniciando requisição HTTP...");

        using var client = new HttpClient();

        try
        {
            // Operação I/O-bound: requisição HTTP
            string data = await client.GetStringAsync("https://jsonplaceholder.typicode.com/posts");
            Console.WriteLine($"[I/O-bound] Conteúdo (primeiros 100 caracteres): {data.Substring(0, Math.Min(100, data.Length))}");

            // Exemplo adicional com tempo limite
            await RunTimedIoBoundExampleAsync();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[I/O-bound] Erro na requisição: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra como implementar um tempo limite para operações I/O-bound.
    /// </summary>
    private static async Task RunTimedIoBoundExampleAsync()
    {
        Console.WriteLine("[I/O-bound] Demonstrando operação com tempo limite...");

        using var client = new HttpClient();

        // Criando um token de cancelamento com tempo limite de 5 segundos
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        try
        {
            // Leitura de arquivo simulada com Task.Delay
            await Task.Delay(1000, cts.Token); // Simula I/O por 1 segundo
            Console.WriteLine("[I/O-bound] Operação com tempo limite concluída com sucesso.");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("[I/O-bound] Operação cancelada por exceder o tempo limite.");
        }
    }
}
