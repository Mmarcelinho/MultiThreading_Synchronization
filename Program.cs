using MultiThreadingSynchronization.src;

namespace MultiThreadingSynchronization;

/// <summary>
/// Programa principal que demonstra vários conceitos de multithreading,
/// sincronização, concorrência, paralelismo e programação assíncrona em .NET.
/// </summary>
class Program
{
    /// <summary>
    /// Ponto de entrada principal do aplicativo.
    /// </summary>
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Demonstrações de Multithreading e Sincronização em .NET ===\n");
        Console.WriteLine("Este aplicativo demonstra vários conceitos importantes relacionados à:");
        Console.WriteLine("- Threads e Tasks");
        Console.WriteLine("- Concorrência e Paralelismo");
        Console.WriteLine("- Programação Assíncrona");
        Console.WriteLine("- Primitivas de Sincronização");
        Console.WriteLine("- Operações CPU-bound e I/O-bound");
        Console.WriteLine("- Deadlocks e Como Evitá-los");
        Console.WriteLine();

        // Exibe menu e aguarda escolha do usuário
        await RunUserChoiceAsync();
    }

    /// <summary>
    /// Exibe um menu para o usuário e executa a demonstração escolhida.
    /// </summary>
    private static async Task RunUserChoiceAsync()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\n=== MENU DE DEMONSTRAÇÕES ===");
            Console.WriteLine("1. Diferença entre Threads e Tasks");
            Console.WriteLine("2. Operações CPU-bound e I/O-bound");
            Console.WriteLine("3. Encadeamento, Exceções e Cancelamento de Tasks");
            Console.WriteLine("4. TPL e Padrões Assíncronos (TAP)");
            Console.WriteLine("5. Concorrência, Paralelismo e Execução Assíncrona");
            Console.WriteLine("6. Threads e Pool de Threads");
            Console.WriteLine("7. Alternância de Contexto (Context Switching)");
            Console.WriteLine("8. Primitivas de Sincronização");
            Console.WriteLine("9. Exemplo de Multi-Core");
            Console.WriteLine("D. Deadlocks e Como Evitá-los");
            Console.WriteLine("0. Executar todos os exemplos");
            Console.WriteLine("X. Sair");

            Console.Write("\nEscolha uma opção: ");
            var key = Console.ReadKey();
            Console.WriteLine("\n");

            switch (key.KeyChar)
            {
                case '1':
                    await RunThreadsAndTasksExamplesAsync();
                    break;
                case '2':
                    await RunBoundOperationExamplesAsync();
                    break;
                case '3':
                    RunTaskChainingAndExceptionExamples();
                    break;
                case '4':
                    await RunTplAndAsyncPatternsExamplesAsync();
                    break;
                case '5':
                    await RunConcurrencyParallelismAsyncExamplesAsync();
                    break;
                case '6':
                    RunThreadsAndPoolExamples();
                    break;
                case '7':
                    RunContextSwitchingExample();
                    break;
                case '8':
                    RunSynchronizationPrimitivesExamples();
                    break;
                case '9':
                    RunMultiCoreExample();
                    break;
                case 'd':
                case 'D':
                    await RunDeadlockExamplesAsync();
                    break;
                case '0':
                    await RunAllExamplesAsync();
                    break;
                case 'x':
                case 'X':
                    exit = true;
                    Console.WriteLine("Saindo...");
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            if (!exit)
            {
                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    /// <summary>
    /// Executa exemplos de Threads e Tasks
    /// </summary>
    private static async Task RunThreadsAndTasksExamplesAsync()
    {
        Console.WriteLine(">> Diferença entre Threads e Tasks:");
        ThreadAndTasks.RunThreadExample();
        ThreadAndTasks.RunTaskExample();
        ThreadAndTasks.RunTaskOption1();
        ThreadAndTasks.RunTaskOption2();
        ThreadAndTasks.RunTaskOption3();
    }

    /// <summary>
    /// Executa exemplos de operações CPU-bound e I/O-bound
    /// </summary>
    private static async Task RunBoundOperationExamplesAsync()
    {
        Console.WriteLine(">> Operações CPU-bound e I/O-bound:");
        BoundOperation.RunCpuBoundExample();
        await BoundOperation.RunIoBoundExampleAsync();
    }

    /// <summary>
    /// Executa exemplos de encadeamento de tarefas, tratamento de exceções e cancelamento
    /// </summary>
    private static void RunTaskChainingAndExceptionExamples()
    {
        Console.WriteLine(">> Encadeamento, Exceções e Cancelamento de Tasks:");
        TaskChainingAndException.RunTaskChainingExample();
        TaskChainingAndException.RunExceptionHandlingExample();
        TaskChainingAndException.RunCancellationExample();
    }

    /// <summary>
    /// Executa exemplos de TPL e padrões assíncronos
    /// </summary>
    private static async Task RunTplAndAsyncPatternsExamplesAsync()
    {
        Console.WriteLine(">> TPL e Padrões Assíncronos:");
        TplAndAsyncPatterns.RunTplExample();
        await TplAndAsyncPatterns.RunAsyncAwaitExampleAsync();
    }

    /// <summary>
    /// Executa exemplos de concorrência, paralelismo e execução assíncrona
    /// </summary>
    private static async Task RunConcurrencyParallelismAsyncExamplesAsync()
    {
        Console.WriteLine(">> Concorrência, Paralelismo e Execução Assíncrona:");
        ConcurrencyParallelismAsync.RunConcurrencyExample();
        ConcurrencyParallelismAsync.RunConcurrencyWithSemaphoreExample();
        ConcurrencyParallelismAsync.RunParallelismExample();
        ConcurrencyParallelismAsync.RunParallelDataProcessingExample();
        await ConcurrencyParallelismAsync.RunAsyncExecutionExampleAsync();
        await ConcurrencyParallelismAsync.RunProgressReportingExampleAsync();
    }

    /// <summary>
    /// Executa exemplos de threads e pool de threads
    /// </summary>
    private static void RunThreadsAndPoolExamples()
    {
        Console.WriteLine(">> Threads e Pool de Threads:");
        Threads.Run();
        Thread_Pool.Run();
    }

    /// <summary>
    /// Executa exemplo de alternância de contexto
    /// </summary>
    private static void RunContextSwitchingExample()
    {
        Console.WriteLine(">> Alternância de Contexto:");
        ContextSwitching.Run();
    }

    /// <summary>
    /// Executa exemplos de primitivas de sincronização
    /// </summary>
    private static void RunSynchronizationPrimitivesExamples()
    {
        Console.WriteLine(">> Primitivas de Sincronização:");
        SynchronizationPrimitives.Run();
    }

    /// <summary>
    /// Executa exemplo de multi-core
    /// </summary>
    private static void RunMultiCoreExample()
    {
        Console.WriteLine(">> Exemplo de Multi-Core:");
        MultiCoreExample.Run();
    }

    /// <summary>
    /// Executa exemplos de deadlocks e como evitá-los
    /// </summary>
    private static async Task RunDeadlockExamplesAsync()
    {
        Console.WriteLine(">> Deadlocks e Como Evitá-los:");
        await DeadlockExamples.RunAsync();
    }

    /// <summary>
    /// Executa todos os exemplos em sequência
    /// </summary>
    private static async Task RunAllExamplesAsync()
    {
        await RunThreadsAndTasksExamplesAsync();
        await RunBoundOperationExamplesAsync();
        RunTaskChainingAndExceptionExamples();
        await RunTplAndAsyncPatternsExamplesAsync();
        await RunConcurrencyParallelismAsyncExamplesAsync();
        RunThreadsAndPoolExamples();
        RunContextSwitchingExample();
        RunSynchronizationPrimitivesExamples();
        RunMultiCoreExample();
        await RunDeadlockExamplesAsync();
    }
}

