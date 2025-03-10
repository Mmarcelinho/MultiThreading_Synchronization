using System.Collections.Concurrent;

namespace MultiThreadingSynchronization.src;

/// <summary>
/// Demonstra as diferentes primitivas de sincronização disponíveis em .NET
/// para coordenar operações em ambientes multithread.
/// </summary>
public static class SynchronizationPrimitives
{
    /// <summary>
    /// Executa todos os exemplos de primitivas de sincronização.
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("=== Demonstração de Primitivas de Sincronização ===\n");

        RunLockExample();
        RunMonitorExample();
        RunMutexExample();
        RunSemaphoreExample();
        RunReaderWriterLockExample();
        RunInterlockedExample();
        RunBarrierExample();
        RunCountdownEventExample();
        RunConcurrentCollectionsExample();
    }

    /// <summary>
    /// Demonstra o uso da palavra-chave 'lock' para proteger um recurso compartilhado.
    /// </summary>
    private static void RunLockExample()
    {
        Console.WriteLine("--- Exemplo de Lock ---");

        var counter = 0;
        var lockObject = new object(); // Objeto de sincronização
        var tasks = new List<Task>();

        // Incrementar o contador de forma segura usando lock
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    lock (lockObject) // Bloqueia o acesso a este bloco para outras threads
                    {
                        counter++;
                    }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
        Console.WriteLine($"[Lock] Contador final: {counter} (esperado: 5000)\n");
    }

    /// <summary>
    /// Demonstra o uso de Monitor para sincronização mais avançada com timeout e pulso.
    /// </summary>
    private static void RunMonitorExample()
    {
        Console.WriteLine("--- Exemplo de Monitor ---");

        var lockObject = new object();
        bool isReady = false;

        // Consumidor - aguarda sinal de que os dados estão prontos
        Task consumer = Task.Run(() =>
        {
            Console.WriteLine("[Monitor] Consumidor aguardando os dados...");
            lock (lockObject)
            {
                // Aguarda até que isReady seja true ou timeout ocorra
                while (!isReady)
                {
                    // Libera o lock temporariamente e aguarda sinal
                    if (!Monitor.Wait(lockObject, 5000))
                    {
                        Console.WriteLine("[Monitor] Timeout ao aguardar os dados!");
                        return;
                    }
                }

                Console.WriteLine("[Monitor] Consumidor processando os dados.");
            }
        });

        // Produtor - prepara os dados e sinaliza
        Task producer = Task.Run(() =>
        {
            Console.WriteLine("[Monitor] Produtor preparando os dados...");
            Thread.Sleep(2000); // Simula preparação de dados

            lock (lockObject)
            {
                isReady = true;
                Console.WriteLine("[Monitor] Produtor sinalizando que os dados estão prontos.");
                Monitor.Pulse(lockObject); // Sinaliza uma thread aguardando
                // Monitor.PulseAll(lockObject); // Para sinalizar todas as threads aguardando
            }
        });

        Task.WaitAll(consumer, producer);
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra o uso de Mutex para sincronização entre processos.
    /// </summary>
    private static void RunMutexExample()
    {
        Console.WriteLine("--- Exemplo de Mutex ---");

        // Cria (ou obtém) um mutex nomeado que pode ser compartilhado entre processos
        // O segundo parâmetro determina se esta thread deve ser proprietária inicial do mutex
        using var mutex = new Mutex(false, "MultiThreadingSynchronizationDemo");

        // Tenta obter o mutex com timeout
        Console.WriteLine("[Mutex] Tentando obter o mutex...");
        bool acquired = mutex.WaitOne(2000);

        if (acquired)
        {
            try
            {
                // Temos acesso exclusivo ao recurso
                Console.WriteLine("[Mutex] Mutex adquirido, executando operação exclusiva...");
                Thread.Sleep(1000); // Simula operação
            }
            finally
            {
                // Sempre libere o mutex
                mutex.ReleaseMutex();
                Console.WriteLine("[Mutex] Mutex liberado.");
            }
        }
        else
        {
            Console.WriteLine("[Mutex] Falha ao adquirir o mutex (timeout).");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra o uso de SemaphoreSlim para limitar o acesso a um recurso.
    /// </summary>
    private static void RunSemaphoreExample()
    {
        Console.WriteLine("--- Exemplo de Semáforo ---");

        // Permite que no máximo 2 threads acessem o recurso simultaneamente
        using var semaphore = new SemaphoreSlim(2, 2);
        var tasks = new List<Task>();

        for (int i = 0; i < 5; i++)
        {
            int id = i;
            tasks.Add(Task.Run(async () =>
            {
                Console.WriteLine($"[Semáforo] Thread {id} aguardando acesso...");

                await semaphore.WaitAsync();
                try
                {
                    Console.WriteLine($"[Semáforo] Thread {id} acessando o recurso limitado.");
                    await Task.Delay(1000); // Simula trabalho
                }
                finally
                {
                    Console.WriteLine($"[Semáforo] Thread {id} liberando o recurso.");
                    semaphore.Release();
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra o uso de ReaderWriterLockSlim para otimizar operações de leitura/escrita.
    /// </summary>
    private static void RunReaderWriterLockExample()
    {
        Console.WriteLine("--- Exemplo de ReaderWriterLock ---");

        var rwLock = new ReaderWriterLockSlim();
        var sharedData = new List<int>();
        var tasks = new List<Task>();

        // Tarefas de leitura (múltiplas podem ler simultaneamente)
        for (int i = 0; i < 3; i++)
        {
            int id = i;
            tasks.Add(Task.Run(() =>
            {
                Console.WriteLine($"[RWLock] Leitor {id} tentando ler...");

                rwLock.EnterReadLock();
                try
                {
                    Console.WriteLine($"[RWLock] Leitor {id} lendo dados: {string.Join(", ", sharedData)}");
                    Thread.Sleep(500); // Simula leitura
                }
                finally
                {
                    rwLock.ExitReadLock();
                    Console.WriteLine($"[RWLock] Leitor {id} concluiu a leitura.");
                }
            }));
        }

        // Tarefas de escrita (apenas uma pode escrever por vez, sem leitores)
        for (int i = 0; i < 2; i++)
        {
            int id = i;
            tasks.Add(Task.Run(() =>
            {
                Console.WriteLine($"[RWLock] Escritor {id} tentando escrever...");

                rwLock.EnterWriteLock();
                try
                {
                    Console.WriteLine($"[RWLock] Escritor {id} modificando dados...");
                    sharedData.Add(id * 10);
                    Thread.Sleep(1000); // Simula escrita
                }
                finally
                {
                    rwLock.ExitWriteLock();
                    Console.WriteLine($"[RWLock] Escritor {id} concluiu a escrita.");
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
        Console.WriteLine($"[RWLock] Dados finais: {string.Join(", ", sharedData)}\n");
    }

    /// <summary>
    /// Demonstra o uso da classe Interlocked para operações atômicas.
    /// </summary>
    private static void RunInterlockedExample()
    {
        Console.WriteLine("--- Exemplo de Interlocked ---");

        long counter = 0;
        var tasks = new List<Task>();

        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    // Incremento atômico, sem necessidade de lock
                    Interlocked.Increment(ref counter);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
        Console.WriteLine($"[Interlocked] Contador final: {counter} (esperado: 5000)");

        // Outras operações atômicas
        long original = counter;
        long valorAnterior = Interlocked.Exchange(ref counter, 100);
        Console.WriteLine($"[Interlocked] Exchange: valor anterior = {valorAnterior}, novo valor = {counter}");

        // Comparar e trocar apenas se valor atual for igual ao esperado
        long comparandValue = 100;
        long newValue = 200;
        long result = Interlocked.CompareExchange(ref counter, newValue, comparandValue);
        Console.WriteLine($"[Interlocked] CompareExchange: resultado = {result}, novo valor = {counter}\n");
    }

    /// <summary>
    /// Demonstra o uso de Barrier para sincronizar múltiplas threads em um ponto comum.
    /// </summary>
    private static void RunBarrierExample()
    {
        Console.WriteLine("--- Exemplo de Barrier ---");

        const int participantCount = 3;
        int currentPhase = 0;

        // Barrier que aguarda 3 participantes em cada fase
        var barrier = new Barrier(participantCount, (b) =>
        {
            // Este código é executado quando todas as threads chegam à barreira
            currentPhase = (int)b.CurrentPhaseNumber;
            Console.WriteLine($"[Barrier] Fase {currentPhase} concluída por todos os participantes.");
        });

        var tasks = new List<Task>();

        for (int i = 0; i < participantCount; i++)
        {
            int id = i;
            tasks.Add(Task.Run(() =>
            {
                for (int phase = 0; phase < 3; phase++)
                {
                    // Simula trabalho para esta fase
                    Console.WriteLine($"[Barrier] Participante {id} trabalhando na fase {phase}...");
                    Thread.Sleep(id * 100 + 500); // Tempo variável para cada participante

                    Console.WriteLine($"[Barrier] Participante {id} esperando outros na barreira...");
                    barrier.SignalAndWait(); // Sinaliza chegada e aguarda outros

                    // Todos os participantes continuam daqui ao mesmo tempo
                    Console.WriteLine($"[Barrier] Participante {id} passou para a próxima fase.");
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra o uso de CountdownEvent para sincronização baseada em contagem.
    /// </summary>
    private static void RunCountdownEventExample()
    {
        Console.WriteLine("--- Exemplo de CountdownEvent ---");

        // Cria um CountdownEvent que aguarda 3 sinalizações
        using var countdown = new CountdownEvent(3);

        // Tarefa que aguarda o CountdownEvent
        Task waiter = Task.Run(() =>
        {
            Console.WriteLine("[CountdownEvent] Aguardando a conclusão de 3 operações...");
            countdown.Wait(); // Bloqueia até que o contador chegue a zero
            Console.WriteLine("[CountdownEvent] Todas as operações concluídas!");
        });

        // Tarefas que sinalizam o CountdownEvent
        for (int i = 0; i < 3; i++)
        {
            int operationId = i;
            Task.Run(() =>
            {
                int delay = (3 - operationId) * 500; // Atraso variável para cada operação
                Thread.Sleep(delay);

                Console.WriteLine($"[CountdownEvent] Operação {operationId} concluída.");
                countdown.Signal(); // Decrementa o contador
                Console.WriteLine($"[CountdownEvent] Contador atual: {countdown.CurrentCount}");
            });
        }

        waiter.Wait();
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstra o uso de coleções concorrentes para ambientes multithreaded.
    /// </summary>
    private static void RunConcurrentCollectionsExample()
    {
        Console.WriteLine("--- Exemplo de Coleções Concorrentes ---");

        // ConcurrentDictionary - thread-safe para múltiplas operações
        var dictionary = new ConcurrentDictionary<int, string>();

        // ConcurrentQueue - fila thread-safe (FIFO)
        var queue = new ConcurrentQueue<int>();

        // ConcurrentBag - coleção desordenada thread-safe
        var bag = new ConcurrentBag<int>();

        // ConcurrentStack - pilha thread-safe (LIFO)
        var stack = new ConcurrentStack<int>();

        var tasks = new List<Task>();

        // Adiciona itens concorrentemente
        for (int i = 0; i < 5; i++)
        {
            int id = i;
            tasks.Add(Task.Run(() =>
            {
                // Adiciona ao dicionário
                dictionary.TryAdd(id, $"Item {id}");

                // Adiciona à fila
                queue.Enqueue(id);

                // Adiciona à bag
                bag.Add(id);

                // Adiciona à pilha
                stack.Push(id);
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Lê itens do dicionário
        Console.WriteLine("[Coleções Concorrentes] Conteúdo do ConcurrentDictionary:");
        foreach (var pair in dictionary)
        {
            Console.WriteLine($"  - Chave: {pair.Key}, Valor: {pair.Value}");
        }

        // Lê itens da fila
        Console.WriteLine("[Coleções Concorrentes] Conteúdo da ConcurrentQueue:");
        while (queue.TryDequeue(out int queueItem))
        {
            Console.WriteLine($"  - {queueItem}");
        }

        // Lê itens da bag
        Console.WriteLine("[Coleções Concorrentes] Conteúdo da ConcurrentBag:");
        foreach (var bagItem in bag)
        {
            Console.WriteLine($"  - {bagItem}");
        }

        // Lê itens da pilha
        Console.WriteLine("[Coleções Concorrentes] Conteúdo da ConcurrentStack:");
        while (stack.TryPop(out int stackItem))
        {
            Console.WriteLine($"  - {stackItem}");
        }

        Console.WriteLine();
    }
}