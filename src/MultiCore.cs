using System.Diagnostics;

namespace MultiThreadingSynchronization.src;

/// <summary>
/// Demonstra a diferença entre operações concorrentes e paralelas
/// em um sistema multi-core.
/// </summary>
public static class MultiCoreExample
{
    /// <summary>
    /// Executa uma tarefa de forma concorrente, onde as operações
    /// são intercaladas por meio de troca de contexto.
    /// </summary>
    private static void ConcurrentTask()
    {
        Console.WriteLine("=== Demonstração de Execução Concorrente ===");
        Console.WriteLine("[Multi-Core] Iniciando execução concorrente...");

        var stopwatch = Stopwatch.StartNew();

        // Simula uma tarefa concorrente (executada com troca de contexto)
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine($"[Multi-Core - Concorrente] Iteração {i}");
            Thread.Sleep(100); // Força troca de contexto
        }

        stopwatch.Stop();
        Console.WriteLine($"[Multi-Core] Execução concorrente concluída em {stopwatch.ElapsedMilliseconds}ms\n");
    }

    /// <summary>
    /// Executa uma tarefa de forma paralela, onde as operações
    /// são distribuídas entre os núcleos disponíveis do processador.
    /// </summary>
    private static void ParallelTask()
    {
        Console.WriteLine("=== Demonstração de Execução Paralela ===");
        Console.WriteLine("[Multi-Core] Iniciando execução paralela...");

        var stopwatch = Stopwatch.StartNew();

        // O Parallel.For tenta distribuir o trabalho entre núcleos disponíveis
        Parallel.For(0, 10, new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, i =>
        {
            Console.WriteLine($"[Multi-Core - Paralelo] Iteração {i} na Thread {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(100); // Simula trabalho
        });

        stopwatch.Stop();
        Console.WriteLine($"[Multi-Core] Execução paralela concluída em {stopwatch.ElapsedMilliseconds}ms\n");
    }

    /// <summary>
    /// Demonstra como verificar o número de núcleos disponíveis e executar
    /// tarefas concorrentes e paralelas para comparação.
    /// </summary>
    public static void Run()
    {
        // Informação sobre o ambiente de execução
        int processorCount = Environment.ProcessorCount;
        Console.WriteLine($"[Multi-Core] Executando em uma máquina com {processorCount} núcleos lógicos");

        // Executa a tarefa concorrente (sequencial com troca de contexto)
        ConcurrentTask();

        // Executa a tarefa paralela (distribuída entre núcleos)
        ParallelTask();

        // Demonstra diferença de desempenho com tarefas CPU-bound
        DemonstrateCpuBoundPerformance();

        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra a diferença de desempenho entre execução sequencial e paralela
    /// para operações CPU-bound em um sistema multi-core.
    /// </summary>
    private static void DemonstrateCpuBoundPerformance()
    {
        Console.WriteLine("=== Comparação de Desempenho Multi-Core ===");

        const int arraySize = 10_000_000;
        var numbers = new int[arraySize];

        // Preenche o array com números
        for (int i = 0; i < arraySize; i++)
        {
            numbers[i] = i;
        }

        // Execução sequencial
        Console.WriteLine("[Multi-Core] Calculando soma sequencialmente...");
        var sequentialStopwatch = Stopwatch.StartNew();

        long sequentialSum = 0;
        for (int i = 0; i < numbers.Length; i++)
        {
            sequentialSum += numbers[i];
        }

        sequentialStopwatch.Stop();
        Console.WriteLine($"[Multi-Core] Soma sequencial: {sequentialSum}, tempo: {sequentialStopwatch.ElapsedMilliseconds}ms");

        // Execução paralela
        Console.WriteLine("[Multi-Core] Calculando soma em paralelo...");
        var parallelStopwatch = Stopwatch.StartNew();

        long parallelSum = 0;
        object lockObj = new object(); // Para sincronização

        Parallel.For(0, numbers.Length, i =>
        {
            // Acesso sincronizado para evitar condições de corrida
            lock (lockObj)
            {
                parallelSum += numbers[i];
            }
        });

        parallelStopwatch.Stop();
        Console.WriteLine($"[Multi-Core] Soma paralela (com lock): {parallelSum}, tempo: {parallelStopwatch.ElapsedMilliseconds}ms");

        // Execução paralela otimizada com Interlocked
        Console.WriteLine("[Multi-Core] Calculando soma em paralelo (otimizado)...");
        var optimizedStopwatch = Stopwatch.StartNew();

        long optimizedSum = 0;

        Parallel.For(0, numbers.Length, i =>
        {
            // Incremento atômico usando Interlocked - mais eficiente que lock
            Interlocked.Add(ref optimizedSum, numbers[i]);
        });

        optimizedStopwatch.Stop();
        Console.WriteLine($"[Multi-Core] Soma paralela (com Interlocked): {optimizedSum}, tempo: {optimizedStopwatch.ElapsedMilliseconds}ms");
    }
}
