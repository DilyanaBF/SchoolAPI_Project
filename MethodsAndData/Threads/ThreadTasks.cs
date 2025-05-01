namespace SchoolAPI_TestProject.MethodsAndData.Threads
{
    public class ThreadTasks
    {
        public static void ThreadTasksMethod()
        {
            Thread thread1 = new Thread(() => PerformTask("Task 1"));
            Thread thread2 = new Thread(() => PerformTask("Task 2"));

            thread1.Start();
            thread2.Start();

            thread1.Join();
            //thread2.Join();

            Console.WriteLine("All tasks completed.");
        }

        static void PerformTask(string taskName)
        {
            for (int i = 1; i <= 3; i++)
            {
                if (i == 2 && taskName == "Task 1")
                {
                    Thread.Sleep(10000);
                    Console.WriteLine("Too much work");
                }
                Console.WriteLine($"{taskName} - Step {i}");
                Thread.Sleep(300); // Simulate work
            }
        }
    }
}
