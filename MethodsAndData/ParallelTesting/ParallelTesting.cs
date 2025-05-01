namespace SchoolAPI_TestProject.MethodsAndData.ParallelTesting
{
    public class ParallelTesting
    {
        public static void ParallelTestingMethod()
        {
            Parallel.For(1, 101, i =>
            {
                Console.WriteLine($"Processing item {i} on thread {Task.CurrentId}");
            });

            Console.WriteLine("All tasks completed.");
        }
    }
}
