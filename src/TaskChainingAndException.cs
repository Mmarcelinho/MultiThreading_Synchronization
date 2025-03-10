namespace MultiThreadingSynchronization.src;

/// <summary>
/// Demonstra técnicas de encadeamento de tarefas, tratamento de exceções e 
/// cancelamento de operações assíncronas.
/// </summary>
public static class TaskChainingAndException
{
    /// <summary>
    /// Demonstra o encadeamento de tarefas (Task Chaining), permitindo
    /// que o resultado de uma tarefa seja processado por outra, criando
    /// um pipeline de operações.
    /// </summary>
    public static void RunTaskChainingExample()
    {
        Console.WriteLine("=== Exemplo de Encadeamento de Tarefas ===");

        // Tarefa inicial
        Console.WriteLine("[Task Chaining] Iniciando encadeamento...");
        Task<int> initialTask = Task.Run(() =>
        {
            Console.WriteLine("[Task Chaining] Gerando número aleatório...");
            Thread.Sleep(1000); // Simula operação longa
            return new Random().Next(1, 100);
        });

        // Continua com uma transformação
        Task<string> formatTask = initialTask.ContinueWith(antecedent =>
        {
            int number = antecedent.Result;
            Console.WriteLine($"[Task Chaining] Número gerado: {number}, aplicando formatação...");
            return $"O número sorteado foi: {number}";
        });

        // Continua com outra operação
        Task<string> appendDateTask = formatTask.ContinueWith(antecedent =>
        {
            string formattedText = antecedent.Result;
            Console.WriteLine("[Task Chaining] Adicionando data ao texto...");
            return $"{formattedText} (em {DateTime.Now.ToShortDateString()})";
        });

        // Resultado final
        string finalResult = appendDateTask.Result;
        Console.WriteLine($"[Task Chaining] Resultado final: {finalResult}\n");

        // Demonstra encadeamento com async/await que é mais limpo
        RunTaskChainingWithAsyncAwait();
    }

    /// <summary>
    /// Demonstra o encadeamento de tarefas usando async/await, que oferece
    /// uma sintaxe mais clara e simplificada comparada ao ContinueWith.
    /// </summary>
    private static void RunTaskChainingWithAsyncAwait()
    {
        Console.WriteLine("=== Encadeamento com Async/Await ===");

        // Executa o método assíncrono e espera o resultado
        string result = Task.Run(async () =>
        {
            Console.WriteLine("[Async Chaining] Iniciando fluxo assíncrono...");

            // Primeira operação assíncrona
            int number = await GenerateRandomNumberAsync();
            Console.WriteLine($"[Async Chaining] Número gerado: {number}");

            // Segunda operação assíncrona
            string formatted = await FormatNumberAsync(number);
            Console.WriteLine($"[Async Chaining] Texto formatado: {formatted}");

            // Terceira operação assíncrona
            string finalText = await AppendDateAsync(formatted);
            Console.WriteLine($"[Async Chaining] Texto com data: {finalText}");

            return finalText;
        }).Result;

        Console.WriteLine($"[Async Chaining] Resultado final: {result}\n");
    }

    // Métodos auxiliares para a demonstração de async/await
    private static async Task<int> GenerateRandomNumberAsync()
    {
        await Task.Delay(500); // Simula operação assíncrona
        return new Random().Next(1, 100);
    }

    private static async Task<string> FormatNumberAsync(int number)
    {
        await Task.Delay(300); // Simula operação assíncrona
        return $"O número sorteado foi: {number}";
    }

    private static async Task<string> AppendDateAsync(string text)
    {
        await Task.Delay(200); // Simula operação assíncrona
        return $"{text} (em {DateTime.Now.ToShortDateString()})";
    }

    /// <summary>
    /// Demonstra o tratamento de exceções em contextos assíncronos,
    /// incluindo a captura de exceções lançadas dentro de Tasks.
    /// </summary>
    public static void RunExceptionHandlingExample()
    {
        Console.WriteLine("=== Exemplo de Tratamento de Exceções ===");

        // 1. Capturando exceção com try/catch em uma Task síncrona
        Console.WriteLine("[Exception Handling] Demonstrando captura de exceção em Task.Wait...");
        try
        {
            // Cria e inicia uma Task que lançará uma exceção
            Task.Run(() =>
            {
                Console.WriteLine("[Exception Handling] Gerando exceção dentro da Task...");
                throw new InvalidOperationException("Erro simulado dentro da Task");
            }).Wait();
        }
        catch (AggregateException ex)
        {
            // O Wait causa o lançamento de AggregateException que encapsula a exceção original
            Console.WriteLine($"[Exception Handling] Exceção capturada via AggregateException: {ex.InnerException?.Message}");
        }

        // 2. Demonstrando tratamento de exceção com ContinueWith
        Console.WriteLine("\n[Exception Handling] Demonstrando tratamento com ContinueWith...");
        Task.Run(() =>
        {
            Console.WriteLine("[Exception Handling] Gerando outra exceção...");
            throw new ApplicationException("Outro erro simulado");
        })
        .ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Exception error = task.Exception?.InnerException;
                Console.WriteLine($"[Exception Handling] Tratado via ContinueWith: {error?.Message}");
            }
        }).Wait();

        // 3. Demonstrando como as exceções são propagadas com await
        Console.WriteLine("\n[Exception Handling] Demonstrando exceções com async/await...");
        try
        {
            Task.Run(async () =>
            {
                await Task.Delay(100);
                await ThrowExceptionAsync();
            }).Wait();
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"[Exception Handling] Exceção de método assíncrono capturada: {ex.InnerException?.Message}\n");
        }
    }

    /// <summary>
    /// Método assíncrono que lança uma exceção para demonstrar o tratamento.
    /// </summary>
    private static async Task ThrowExceptionAsync()
    {
        await Task.Delay(100); // Simula algum trabalho assíncrono
        throw new InvalidOperationException("Exceção em método assíncrono");
    }

    /// <summary>
    /// Demonstra como implementar e usar o mecanismo de cancelamento 
    /// de tarefas assíncronas com CancellationToken.
    /// </summary>
    public static void RunCancellationExample()
    {
        Console.WriteLine("=== Exemplo de Cancelamento de Tarefas ===");

        // Criando a fonte de cancelamento com tempo limite de 2 segundos
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        // Iniciando uma tarefa cancelável que verifica o token
        Console.WriteLine("[Cancellation] Iniciando tarefa cancelável...");
        var task = Task.Run(() =>
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    // Verifica se o cancelamento foi solicitado
                    if (cts.Token.IsCancellationRequested)
                    {
                        Console.WriteLine("[Cancellation] Cancelamento detectado, limpando recursos...");
                        cts.Token.ThrowIfCancellationRequested(); // Gera OperationCanceledException
                    }

                    Console.WriteLine($"[Cancellation] Processando... {i + 1}/10");
                    Thread.Sleep(500); // Simula trabalho
                }

                Console.WriteLine("[Cancellation] Tarefa concluída com sucesso!");
                return "Sucesso";
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[Cancellation] Tarefa foi cancelada graciosamente.");
                throw; // Propaga a exceção para preservar o estado da Task como Canceled
            }
        }, cts.Token);

        try
        {
            // Aguardando a conclusão da tarefa
            string result = task.Result; // Irá bloquear até concluir, cancelar ou falhar
            Console.WriteLine($"[Cancellation] Resultado: {result}");
        }
        catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
        {
            Console.WriteLine("[Cancellation] A tarefa foi cancelada conforme esperado.");
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"[Cancellation] Erro inesperado: {ex.InnerException?.Message}");
        }

        Console.WriteLine($"[Cancellation] Estado final da Task: {task.Status}\n");

        // Demonstrar também cancelamento cooperativo assíncrono
        RunCooperativeCancellationExample();
    }

    /// <summary>
    /// Demonstra o cancelamento cooperativo em um contexto assíncrono
    /// com async/await.
    /// </summary>
    private static void RunCooperativeCancellationExample()
    {
        Console.WriteLine("=== Cancelamento Cooperativo Assíncrono ===");

        // Executa e aguarda o método assíncrono
        try
        {
            Task.Run(async () =>
            {
                using var cts = new CancellationTokenSource();

                // Configura cancelamento automático após 2 segundos
                cts.CancelAfter(2000);

                Console.WriteLine("[Cancelamento Cooperativo] Iniciando operação de longa duração...");

                try
                {
                    // Simula uma operação assíncrona cancelável
                    string result = await SimulateLongRunningOperationAsync(cts.Token);
                    Console.WriteLine($"[Cancelamento Cooperativo] Resultado: {result}");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("[Cancelamento Cooperativo] Operação foi cancelada conforme esperado.");
                }
            }).Wait();
        }
        catch (AggregateException)
        {
            // Não deve chegar aqui se as exceções forem tratadas corretamente
            Console.WriteLine("[Cancelamento Cooperativo] Falha ao tratar o cancelamento.");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Simula uma operação de longa duração que suporta cancelamento.
    /// </summary>
    private static async Task<string> SimulateLongRunningOperationAsync(CancellationToken cancellationToken)
    {
        for (int i = 0; i < 10; i++)
        {
            // Verifica cancelamento antes de cada etapa
            cancellationToken.ThrowIfCancellationRequested();

            Console.WriteLine($"[Cancelamento Cooperativo] Etapa {i + 1}/10 em processamento...");

            try
            {
                // Usa o token para cancelar o Delay se necessário
                await Task.Delay(500, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[Cancelamento Cooperativo] Delay foi cancelado.");
                throw; // Propaga para o chamador
            }
        }

        return "Operação de longa duração concluída com sucesso!";
    }
}
