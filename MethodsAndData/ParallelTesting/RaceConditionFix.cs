namespace SchoolAPI_TestProject.MethodsAndData.ParallelTesting
{
    public class RaceConditionFix
    {
        private static int sharedCounter = 0;
        private static readonly object lockObject = new object();

        public static void RaceConditionFixMethod()
        {
            Parallel.For(0, 1000, _ =>
            {
                lock (lockObject)
                {
                    sharedCounter= sharedCounter+99;
                }
            });

            Console.WriteLine($"Final value of shared counter: {sharedCounter}");
        }
    }
}
