namespace MultiThreadingSynchronization.src;

public static class ThreadAndTasks
{
    // Exemplo de criação e início de uma Thread
    public static void RunThreadExample()
    {
        var thread = new Thread(() =>
        {
            Console.WriteLine("[Thread] Executando em uma thread separada.");
        });
        thread.Start();
        thread.Join();
        Console.WriteLine("[Thread] Thread finalizada.\n");
    }

    // Exemplo simples de uso de Task
    public static void RunTaskExample()
    {
        Task.Run(() =>
        {
            Console.WriteLine("[Task] Executando no ThreadPool através de Task.Run.");
        }).Wait();
        Console.WriteLine("[Task] Task finalizada.\n");
    }

    // Opção 1: new Task(Action).Start()
    public static void RunTaskOption1()
    {
        var task = new Task(() =>
        {
            Console.WriteLine("[TaskOption1] Executando uma Task criada com new Task().Start().");
        });
        task.Start();
        task.Wait();
        Console.WriteLine("[TaskOption1] Concluída.\n");
    }

    // Opção 2: Task.Factory.StartNew(Action)
    public static void RunTaskOption2()
    {
        var task = Task.Factory.StartNew(() =>
        {
            Console.WriteLine("[TaskOption2] Executando uma Task com Task.Factory.StartNew (LongRunning).");
        }, TaskCreationOptions.LongRunning);
        task.Wait();
        Console.WriteLine("[TaskOption2] Concluída.\n");
    }

    // Opção 3: Task.Run(Action)
    public static void RunTaskOption3()
    {
        Task.Run(() =>
        {
            Console.WriteLine("[TaskOption3] Executando uma Task com Task.Run no ThreadPool.");
        }).Wait();
        Console.WriteLine("[TaskOption3] Concluída.\n");
    }
}
