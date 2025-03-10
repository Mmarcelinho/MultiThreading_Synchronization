using System.Diagnostics;

namespace MultiThreadingSynchronization.src;

/// <summary>
/// Demonstra os conceitos de concorrência, paralelismo e execução assíncrona.
/// 
/// Concorrência: Múltiplas tarefas que progridem durante o mesmo período de tempo (alternância entre elas)
/// Paralelismo: Múltiplas tarefas executando simultaneamente (ao mesmo tempo)
/// Assincronia: Execução não bloqueante, permitindo que uma thread continue trabalhando enquanto aguarda
/// </summary>
public static class ConcurrencyParallelismAsync
{
    /// <summary>
    /// Demonstra concorrência usando threads que se alternam na execução.
    /// Na concorrência, as tarefas progridem durante o mesmo período, mas podem 
    /// não estar executando exatamente ao mesmo tempo (alternância de contexto).
    /// </summary>
    public static void RunConcurrencyExample()
    {
        Console.WriteLine("=== Exemplo de Concorrência ===");

        void Task1()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"[Concorrência] Task 1 - Iteração {i}");
                Thread.Sleep(100); // Simula processamento e força alternância
            }
        }

        void Task2()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"[Concorrência] Task 2 - Iteração {i}");
                Thread.Sleep(100); // Simula processamento e força alternância
            }
        }

        // Criação e execução de threads concorrentes
        Thread thread1 = new Thread(Task1);
        Thread thread2 = new Thread(Task2);

        // Iniciando as threads
        Console.WriteLine("[Concorrência] Iniciando threads concorrentes...");
        thread1.Start();
        thread2.Start();

        // Aguardando a conclusão das threads
        thread1.Join();
        thread2.Join();

        Console.WriteLine("[Concorrência] Tarefas concorrentes concluídas.\n");
    }

    /// <summary>
    /// Demonstra a concorrência com limitação de recursos usando um semáforo.
    /// Útil quando queremos limitar o número de threads que acessam um recurso.
    /// </summary>
    public static void RunConcurrencyWithSemaphoreExample()
    {
        Console.WriteLine("=== Exemplo de Concorrência com Semáforo ===");

        // Semáforo que permite apenas 2 threads ao mesmo tempo
        using var semaphore = new SemaphoreSlim(2, 2);
        var tasks = new List<Task>();

        for (int i = 0; i < 5; i++)
        {
            int taskId = i;
            tasks.Add(Task.Run(async () =>
            {
                Console.WriteLine($"[Semáforo] Task {taskId} aguardando acesso ao recurso...");
                await semaphore.WaitAsync();
                try
                {
                    Console.WriteLine($"[Semáforo] Task {taskId} acessando o recurso limitado");
                    await Task.Delay(1000); // Simula trabalho com o recurso
                }
                finally
                {
                    Console.WriteLine($"[Semáforo] Task {taskId} liberando o recurso");
                    semaphore.Release();
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
        Console.WriteLine("[Semáforo] Exemplo concluído.\n");
    }

    /// <summary>
    /// Demonstra paralelismo utilizando Parallel.Invoke.
    /// No paralelismo, múltiplas tarefas são executadas simultaneamente
    /// em diferentes núcleos de CPU, quando disponíveis.
    /// </summary>
    public static void RunParallelismExample()
    {
        Console.WriteLine("=== Exemplo de Paralelismo ===");

        Console.WriteLine("[Paralelismo] Executando tarefas em paralelo...");

        var stopwatch = Stopwatch.StartNew();

        // Usando Parallel.Invoke para executar tarefas em paralelo
        Parallel.Invoke(
            () =>
            {
                // Simula trabalho pesado
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"[Paralelismo] Task 1 - Iteração {i}");
                    Thread.Sleep(100);
                }
            },
            () =>
            {
                // Simula trabalho pesado
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"[Paralelismo] Task 2 - Iteração {i}");
                    Thread.Sleep(100);
                }
            }
        );

        stopwatch.Stop();
        Console.WriteLine($"[Paralelismo] Tarefas paralelas concluídas em {stopwatch.ElapsedMilliseconds}ms\n");

        // Comparação com execução sequencial
        RunSequentialComparisonExample();
    }

    /// <summary>
    /// Demonstra o tempo de execução sequencial para comparação com a execução paralela.
    /// </summary>
    private static void RunSequentialComparisonExample()
    {
        Console.WriteLine("=== Comparação com Execução Sequencial ===");

        var stopwatch = Stopwatch.StartNew();

        // Executando o mesmo trabalho sequencialmente
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"[Sequencial] Task 1 - Iteração {i}");
            Thread.Sleep(100);
        }

        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"[Sequencial] Task 2 - Iteração {i}");
            Thread.Sleep(100);
        }

        stopwatch.Stop();
        Console.WriteLine($"[Sequencial] Tarefas sequenciais concluídas em {stopwatch.ElapsedMilliseconds}ms\n");
    }

    /// <summary>
    /// Demonstra o processamento paralelo de dados com Parallel.ForEach.
    /// </summary>
    public static void RunParallelDataProcessingExample()
    {
        Console.WriteLine("=== Exemplo de Processamento Paralelo de Dados ===");

        var items = Enumerable.Range(1, 10).ToList();

        Console.WriteLine("[Processamento Paralelo] Iniciando processamento paralelo de uma coleção de dados...");

        // Usando Parallel.ForEach para processar dados em paralelo
        Parallel.ForEach(items,
            // Configurações opcionais de paralelismo
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            (item) =>
            {
                // Simula processamento de cada item
                Console.WriteLine($"[Processamento Paralelo] Processando item {item} na thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(100); // Simula trabalho
            });

        Console.WriteLine("[Processamento Paralelo] Processamento concluído.\n");
    }

    /// <summary>
    /// Exemplo de execução assíncrona com async/await.
    /// Demonstra como operações I/O-bound podem ser executadas de forma
    /// não bloqueante usando o padrão async/await.
    /// </summary>
    public static async Task RunAsyncExecutionExampleAsync()
    {
        Console.WriteLine("=== Exemplo de Execução Assíncrona ===");

        Console.WriteLine("[Async] Iniciando operações assíncronas...");

        // Executa múltiplas operações assíncronas em paralelo
        var task1 = SimulateAsyncOperationAsync("Operação 1", 2000);
        var task2 = SimulateAsyncOperationAsync("Operação 2", 1000);
        var task3 = SimulateAsyncOperationAsync("Operação 3", 1500);

        // Aguarda todas as tarefas completarem
        await Task.WhenAll(task1, task2, task3);

        Console.WriteLine("[Async] Todas as operações assíncronas foram concluídas.\n");
    }

    /// <summary>
    /// Simula uma operação assíncrona que leva um tempo específico para concluir.
    /// </summary>
    private static async Task SimulateAsyncOperationAsync(string operationName, int delayMs)
    {
        Console.WriteLine($"[Async] {operationName} iniciada");
        await Task.Delay(delayMs); // Simula operação assíncrona
        Console.WriteLine($"[Async] {operationName} concluída após {delayMs}ms");
    }

    /// <summary>
    /// Demonstra o uso de Progress para reportar progresso de operações assíncronas.
    /// </summary>
    public static async Task RunProgressReportingExampleAsync()
    {
        Console.WriteLine("=== Exemplo de Reporte de Progresso ===");

        // Cria um manipulador de progresso
        var progress = new Progress<int>(percentComplete =>
            Console.WriteLine($"[Progresso] Completado: {percentComplete}%"));

        // Executa operação assíncrona com reporte de progresso
        await SimulateWorkWithProgressAsync(progress);

        Console.WriteLine("[Progresso] Operação com progresso concluída.\n");
    }

    /// <summary>
    /// Simula trabalho assíncrono que reporta progresso.
    /// </summary>
    private static async Task SimulateWorkWithProgressAsync(IProgress<int> progress)
    {
        for (int i = 0; i <= 100; i += 10)
        {
            // Reporta progresso atual
            progress.Report(i);

            // Simula trabalho
            await Task.Delay(300);
        }
    }
}
