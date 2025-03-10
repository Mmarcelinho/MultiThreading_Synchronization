namespace MultiThreadingSynchronization.src;

public static class ContextSwitching
{
    static void PrintNumbers(string threadName)
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine($"{threadName}: {i}");
            Thread.Sleep(100);
        }
    }

    public static void Run()
    {
        Thread thread1 = new Thread(() => PrintNumbers("Thread 1"));
        Thread thread2 = new Thread(() => PrintNumbers("Thread 2"));

        thread1.Start();
        thread2.Start();

        thread1.Join();
        thread2.Join();
        Console.WriteLine();
    }
}
