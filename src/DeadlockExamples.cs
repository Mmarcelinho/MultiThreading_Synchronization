namespace MultiThreadingSynchronization.src;

/// <summary>
/// Demonstra cenários de deadlock em ambientes multithreaded e
/// técnicas para evitá-los.
/// 
/// Um deadlock ocorre quando duas ou mais threads ficam bloqueadas
/// permanentemente, esperando uma pela outra.
/// </summary>
public static class DeadlockExamples
{
    /// <summary>
    /// Executa exemplos de deadlock e suas soluções
    /// </summary>
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Demonstração de Deadlocks e Sua Prevenção ===\n");

        // Exemplo clássico de deadlock com dois locks
        RunClassicDeadlockExample();

        // Deadlock por hierarquia de chamadas assíncronas
        await RunAsyncDeadlockExampleAsync();

        // Solução para deadlocks: ordenação de recursos
        RunResourceOrderingExample();

        // Solução para deadlocks: timeout em locks
        RunTimeoutExample();

        // Solução para deadlocks: usando ConfigureAwait(false)
        await RunConfigureAwaitExampleAsync();
    }

    /// <summary>
    /// Demonstra o caso clássico de deadlock com dois recursos
    /// </summary>
    private static void RunClassicDeadlockExample()
    {
        Console.WriteLine("--- Exemplo de Deadlock Clássico ---");
        Console.WriteLine("[Deadlock] Demonstrando um cenário POTENCIAL de deadlock (não executado completamente)");

        var recursoA = new object();
        var recursoB = new object();

        // Thread 1: tenta adquirir recursoA e depois recursoB
        void Thread1()
        {
            Console.WriteLine("[Deadlock] Thread 1: Tentando adquirir recurso A...");
            lock (recursoA)
            {
                Console.WriteLine("[Deadlock] Thread 1: Recurso A adquirido, trabalhando...");
                Thread.Sleep(100); // Simulando trabalho

                Console.WriteLine("[Deadlock] Thread 1: Tentando adquirir recurso B...");
                lock (recursoB)
                {
                    Console.WriteLine("[Deadlock] Thread 1: Recurso B adquirido, trabalho completo.");
                }
            }
        }

        // Thread 2: tenta adquirir recursoB e depois recursoA
        void Thread2()
        {
            Console.WriteLine("[Deadlock] Thread 2: Tentando adquirir recurso B...");
            lock (recursoB)
            {
                Console.WriteLine("[Deadlock] Thread 2: Recurso B adquirido, trabalhando...");
                Thread.Sleep(100); // Simulando trabalho

                Console.WriteLine("[Deadlock] Thread 2: Tentando adquirir recurso A...");
                lock (recursoA)
                {
                    Console.WriteLine("[Deadlock] Thread 2: Recurso A adquirido, trabalho completo.");
                }
            }
        }

        // NOTA: Não executamos completamente este exemplo para não travar o programa!
        Console.WriteLine("[Deadlock] Em um cenário real, as duas threads ficariam bloqueadas permanentemente");
        Console.WriteLine("[Deadlock] Thread 1 esperando pelo recurso B (que está com Thread 2)");
        Console.WriteLine("[Deadlock] Thread 2 esperando pelo recurso A (que está com Thread 1)");

        // Demonstração controlada
        bool useDeadlock = false;

        if (useDeadlock)
        {
            // Esta parte causaria um deadlock real
            var thread1 = new Thread(() => Thread1());
            var thread2 = new Thread(() => Thread2());

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();
        }
        else
        {
            // Demonstramos apenas o início do processo para explicação
            Console.WriteLine("[Deadlock] Demonstração segura - sem executar o deadlock real");
            Thread1(); // Executa sequencialmente, sem deadlock
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra um deadlock causado por operações assíncronas e captura de contexto
    /// </summary>
    private static async Task RunAsyncDeadlockExampleAsync()
    {
        Console.WriteLine("--- Exemplo de Deadlock Assíncrono ---");

        // Simulação de um método que causa deadlock se chamado síncronamente
        async Task OperacaoAssincronaAsync()
        {
            await Task.Delay(500); // Operação assíncrona
            Console.WriteLine("[Deadlock Assíncrono] Operação assíncrona concluída");
        }

        Console.WriteLine("[Deadlock Assíncrono] Chamada segura com async/await:");
        await OperacaoAssincronaAsync(); // Maneira correta - usa await

        Console.WriteLine("\n[Deadlock Assíncrono] Demonstração de problema potencial quando chamado síncronamente:");
        Console.WriteLine("[Deadlock Assíncrono] Em aplicações UI/ASP.NET, chamar .Result/.Wait() pode causar deadlock");
        Console.WriteLine("[Deadlock Assíncrono] devido ao bloqueio do context capturer enquanto aguarda uma task");
        Console.WriteLine("[Deadlock Assíncrono] que tenta voltar ao mesmo contexto já bloqueado.");

        // Não demonstramos o código real do deadlock aqui para evitar travar o exemplo
        Console.WriteLine("[Deadlock Assíncrono] Exemplo seguro - não executando o código real de deadlock");

        // Exemplo de código que causaria deadlock em aplicações com SynchronizationContext:
        /*
        Código de deadlock:
        
        private void BotaoClick()
        {
            // Bloqueia a thread de UI, mas o método assíncrono tenta retornar para ela
            OperacaoAssincronaAsync().Wait(); // DEADLOCK!
        }
        */

        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra a solução de deadlock através de ordenação de recursos
    /// </summary>
    private static void RunResourceOrderingExample()
    {
        Console.WriteLine("--- Solução: Ordenação de Recursos ---");

        var recursoA = new object();
        var recursoB = new object();

        // Ambas as threads adquirem os recursos na mesma ordem, evitando deadlock
        void Thread1Corrigida()
        {
            Console.WriteLine("[Solução] Thread 1: Adquirindo recursos em ordem consistente...");
            lock (recursoA)  // Sempre lock A primeiro
            {
                Thread.Sleep(100);
                lock (recursoB)  // Depois lock B
                {
                    Console.WriteLine("[Solução] Thread 1: Trabalho concluído com sucesso");
                }
            }
        }

        void Thread2Corrigida()
        {
            Console.WriteLine("[Solução] Thread 2: Adquirindo recursos em ordem consistente...");
            lock (recursoA)  // Sempre lock A primeiro
            {
                Thread.Sleep(100);
                lock (recursoB)  // Depois lock B
                {
                    Console.WriteLine("[Solução] Thread 2: Trabalho concluído com sucesso");
                }
            }
        }

        // Execução
        var thread1 = new Thread(() => Thread1Corrigida());
        var thread2 = new Thread(() => Thread2Corrigida());

        thread1.Start();
        thread2.Start();

        thread1.Join();
        thread2.Join();

        Console.WriteLine("[Solução] Ambas as threads concluíram sem deadlock usando ordenação consistente de recursos");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra a solução de deadlock usando timeout em locks
    /// </summary>
    private static void RunTimeoutExample()
    {
        Console.WriteLine("--- Solução: Timeout em Locks ---");

        var mutex1 = new Mutex();
        var mutex2 = new Mutex();

        // Usando Mutex com timeout para evitar deadlock permanente
        void ThreadComTimeoutSegura()
        {
            Console.WriteLine("[Timeout] Thread: Tentando adquirir mutex 1...");
            if (mutex1.WaitOne(1000))  // Timeout de 1 segundo
            {
                try
                {
                    Console.WriteLine("[Timeout] Thread: Mutex 1 adquirido, trabalhando...");
                    Thread.Sleep(100);

                    Console.WriteLine("[Timeout] Thread: Tentando adquirir mutex 2...");
                    if (mutex2.WaitOne(1000))  // Timeout de 1 segundo
                    {
                        try
                        {
                            Console.WriteLine("[Timeout] Thread: Mutex 2 adquirido, trabalho concluído.");
                        }
                        finally
                        {
                            mutex2.ReleaseMutex();
                        }
                    }
                    else
                    {
                        Console.WriteLine("[Timeout] Thread: Timeout ao esperar pelo mutex 2. Liberando recursos e tentando novamente...");
                    }
                }
                finally
                {
                    mutex1.ReleaseMutex();
                }
            }
            else
            {
                Console.WriteLine("[Timeout] Thread: Timeout ao esperar pelo mutex 1. Tentando novamente mais tarde...");
            }
        }

        var thread = new Thread(() => ThreadComTimeoutSegura());
        thread.Start();
        thread.Join();

        Console.WriteLine("[Timeout] Uso de timeout evita que a thread fique bloqueada indefinidamente");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra o uso de ConfigureAwait(false) para evitar deadlocks assíncronos
    /// </summary>
    private static async Task RunConfigureAwaitExampleAsync()
    {
        Console.WriteLine("--- Solução: ConfigureAwait(false) ---");

        // Método que usa ConfigureAwait(false) para evitar retornar ao SynchronizationContext original
        async Task MetodoSeguroAsync()
        {
            Console.WriteLine("[ConfigureAwait] Iniciando operação assíncrona...");

            // Usando ConfigureAwait(false) para não retornar ao contexto original
            await Task.Delay(500).ConfigureAwait(false);

            // Este código continuará em qualquer thread disponível do pool
            Console.WriteLine($"[ConfigureAwait] Operação concluída na thread {Thread.CurrentThread.ManagedThreadId}");

            // Fazendo mais trabalho assíncrono
            await Task.Delay(300).ConfigureAwait(false);

            Console.WriteLine("[ConfigureAwait] Trabalho finalizado sem depender do contexto original");
        }

        // Chama o método seguro
        await MetodoSeguroAsync();

        Console.WriteLine("[ConfigureAwait] A operação usando ConfigureAwait(false) previne deadlocks em:");
        Console.WriteLine("- Aplicações UI (WPF, WinForms, etc.)");
        Console.WriteLine("- ASP.NET (contexto de sincronização web)");
        Console.WriteLine("- Bibliotecas reutilizáveis (para não depender do contexto do chamador)");

        Console.WriteLine();
    }
}