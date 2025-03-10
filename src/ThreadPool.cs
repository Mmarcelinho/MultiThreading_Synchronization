using System.Diagnostics;

namespace MultiThreadingSynchronization.src;

/// <summary>
/// Demonstra o uso do Pool de Threads do .NET para executar
/// operações de forma eficiente, evitando a criação excessiva de threads.
/// 
/// O ThreadPool gerencia automaticamente um conjunto de threads de trabalho
/// que são reutilizados para diferentes tarefas, reduzindo a sobrecarga
/// de criação e destruição de threads.
/// </summary>
public static class Thread_Pool
{
    /// <summary>
    /// Executa exemplos de uso do ThreadPool
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("=== Demonstração de ThreadPool ===\n");
        
        // Configuração do ThreadPool para o exemplo
        SetupThreadPoolConfiguration();
        
        // Exemplo simples de QueueUserWorkItem
        RunBasicThreadPoolExample();
        
        // Comparação de desempenho: Thread vs ThreadPool
        RunPerformanceComparisonExample();
        
        // Demonstração do controle de threads no pool
        RunThreadPoolControlExample();
    }

    /// <summary>
    /// Obtém e apresenta a configuração atual do ThreadPool
    /// </summary>
    private static void SetupThreadPoolConfiguration()
    {
        // Obtém a configuração atual do ThreadPool
        ThreadPool.GetMinThreads(out int minWorkerThreads, out int minCompletionPortThreads);
        ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);
        
        Console.WriteLine("Configuração atual do ThreadPool:");
        Console.WriteLine($"- Threads de trabalho: Min = {minWorkerThreads}, Max = {maxWorkerThreads}");
        Console.WriteLine($"- Threads de porta de conclusão: Min = {minCompletionPortThreads}, Max = {maxCompletionPortThreads}\n");
    }

    /// <summary>
    /// Demonstra o uso básico do ThreadPool com QueueUserWorkItem
    /// </summary>
    private static void RunBasicThreadPoolExample()
    {
        Console.WriteLine("--- Exemplo Básico de ThreadPool ---");
        
        var resetEvent = new ManualResetEvent(false);
        
        // Enfileira um trabalho para ser executado por uma thread do pool
        Console.WriteLine("[ThreadPool] Enfileirando trabalho no ThreadPool...");
        ThreadPool.QueueUserWorkItem(state =>
        {
            Console.WriteLine($"[ThreadPool] Executando trabalho na thread {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(1000); // Simula trabalho
            Console.WriteLine("[ThreadPool] Trabalho concluído.");
            
            // Sinaliza que o trabalho foi concluído
            resetEvent.Set();
        });
        
        // Aguarda a conclusão do trabalho
        resetEvent.WaitOne();
        Console.WriteLine();
    }

    /// <summary>
    /// Compara o desempenho de criação de threads manualmente vs uso do ThreadPool
    /// </summary>
    private static void RunPerformanceComparisonExample()
    {
        Console.WriteLine("--- Comparação de Desempenho: Thread vs ThreadPool ---");
        
        const int taskCount = 100;
        var stopwatch = new Stopwatch();
        
        // 1. Criação manual de threads
        Console.WriteLine($"[ThreadPool] Criando {taskCount} threads manualmente...");
        stopwatch.Restart();
        
        var threads = new Thread[taskCount];
        var threadResetEvent = new ManualResetEvent(false);
        int threadCounter = 0;
        
        for (int i = 0; i < taskCount; i++)
        {
            threads[i] = new Thread(() =>
            {
                // Simula uma operação rápida
                if (Interlocked.Increment(ref threadCounter) == taskCount)
                {
                    threadResetEvent.Set();
                }
            });
            threads[i].Start();
        }
        
        threadResetEvent.WaitOne();
        stopwatch.Stop();
        
        Console.WriteLine($"[ThreadPool] Tempo para criar e executar {taskCount} threads: {stopwatch.ElapsedMilliseconds}ms");
        
        // 2. Usando ThreadPool
        Console.WriteLine($"[ThreadPool] Enfileirando {taskCount} trabalhos no ThreadPool...");
        stopwatch.Restart();
        
        var poolResetEvent = new ManualResetEvent(false);
        int poolCounter = 0;
        
        for (int i = 0; i < taskCount; i++)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                // Simula uma operação rápida
                if (Interlocked.Increment(ref poolCounter) == taskCount)
                {
                    poolResetEvent.Set();
                }
            });
        }
        
        poolResetEvent.WaitOne();
        stopwatch.Stop();
        
        Console.WriteLine($"[ThreadPool] Tempo para enfileirar e executar {taskCount} trabalhos no ThreadPool: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra o controle de threads no pool com configurações personalizadas
    /// </summary>
    private static void RunThreadPoolControlExample()
    {
        Console.WriteLine("--- Controle de Threads no Pool ---");
        
        // Obtém o número atual de threads no pool
        ThreadPool.GetAvailableThreads(out int workerThreads, out int completionPortThreads);
        Console.WriteLine($"[ThreadPool] Threads disponíveis: {workerThreads} worker, {completionPortThreads} I/O");
        
        // Configura o número mínimo de threads
        ThreadPool.GetMinThreads(out int minWorkerThreads, out int minCompletionPortThreads);
        
        Console.WriteLine("[ThreadPool] Ajustando o número mínimo de threads...");
        if (ThreadPool.SetMinThreads(Math.Min(Environment.ProcessorCount * 2, minWorkerThreads + 4), minCompletionPortThreads))
        {
            Console.WriteLine("[ThreadPool] Configuração mínima atualizada com sucesso.");
        }
        
        // Demonstra a execução de múltiplas tarefas com o novo mínimo
        Console.WriteLine("[ThreadPool] Executando várias tarefas simultaneamente...");
        var countdownEvent = new CountdownEvent(10);
        
        for (int i = 0; i < 10; i++)
        {
            int taskId = i;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Console.WriteLine($"[ThreadPool] Tarefa {taskId} iniciada na thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(500); // Simula trabalho
                Console.WriteLine($"[ThreadPool] Tarefa {taskId} concluída.");
                countdownEvent.Signal();
            });
        }
        
        // Aguarda todas as tarefas serem concluídas
        countdownEvent.Wait();
        
        // Restaura a configuração mínima original
        ThreadPool.SetMinThreads(minWorkerThreads, minCompletionPortThreads);
        
        Console.WriteLine();
    }
}

