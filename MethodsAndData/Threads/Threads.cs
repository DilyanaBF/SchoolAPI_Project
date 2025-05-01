namespace SchoolAPI_TestProject.MethodsAndData.Threads
{
    public class Threads
    {
        public static void ThreadsMethod()
        {
            Thread thread = new Thread(PrintNumbers);
            thread.Start();

            Console.WriteLine("Main thread is running.");
        }

        static void PrintNumbers()
        {
            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine($"Thread: {i}");
                Thread.Sleep(500); // Simulate work
            }
        }
    }
}
