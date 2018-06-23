using System;
using System.Threading.Tasks;

namespace RxDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            DoSync(new Demo().Ten_B);
            Console.ReadLine();
        }

        private static void DoAsync(Func<Task> f)
        {
            var task = Task.Run(f);
            Console.WriteLine("Async demo task was launched");
            task.Wait();
            Console.WriteLine("Async demo task is completed");
        }

        private static void DoSync(Action a)
        {
            a();
            Console.WriteLine("Demo task is completed");
        }
    }
}
