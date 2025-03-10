namespace MultiThreadingSynchronization.src;

public static class Threads
{
    static void SlowMethod()
    {
        Console.WriteLine($"[Thread] Iniciando em: {Thread.CurrentThread.Name}");
        Thread.Sleep(2000); // Simula operação longa
        Console.WriteLine($"[Thread] Concluído em: {Thread.CurrentThread.Name}");
    }

    public static void Run()
    {
        Thread thread = new Thread(SlowMethod)
        {
            Name = "Worker Thread"
        };
        thread.Start();
        Console.WriteLine("[Thread] Executando no Main Thread.");
        thread.Join(); // Aguarda a thread terminar
        Console.WriteLine();
    }
}
