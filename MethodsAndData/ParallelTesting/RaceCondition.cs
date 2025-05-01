namespace SchoolAPI_TestProject.MethodsAndData.ParallelTesting
{
    public class RaceCondition
    {
        private static int sharedCounter = 0;

        public static void RaceConditionMethod()
        {
            Parallel.For(0, 1000, _ =>
            {
                sharedCounter = sharedCounter + 1;
            });

            Console.WriteLine($"Final value of shared counter: {sharedCounter}");
        }
    }
}
