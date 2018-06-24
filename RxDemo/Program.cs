using System;
using System.Threading.Tasks;

namespace RxDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //DoSync(new Demo().One);
            //DoSync(new Demo().Two);
            //DoAsync(new Demo().Three_A);
            //DoAsync(new Demo().Three_B);
            //DoSync(new Demo().Four);
            //DoSync(new Demo().Five_A);
            //DoSync(new Demo().Five_B);
            //DoSync(new Demo().Six_A);
            //DoSync(new Demo().Six_B);
            //DoSync(new Demo().Seven_A);
            //DoSync(new Demo().Seven_B);
            //DoSync(new Demo().Seven_C);
            //DoSync(new Demo().Seven_D);
            //DoSync(new Demo().Eight);
            //DoSync(new Demo().Nine_A);
            //DoSync(new Demo().Nine_B);
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
