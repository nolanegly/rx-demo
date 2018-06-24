using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace RxDemo
{
    // I thought it was interesting to discover that observables can be awaited without being subscribed to.
    public partial class Demo
    {
        // If you await an Observable, you only get the last completion value
        public async Task Three_A()
        {
            Console.WriteLine("getting...");
            var range = await Observable.Range(1, 9999999);
            Console.WriteLine(range);
        }

        // Works even if it's set onto a different thread
        public async Task Three_B()
        {
            Console.WriteLine("getting...");
            var range = await Observable.Range(1, 9999999, new NewThreadScheduler());
            Console.WriteLine(range);
        }
    }
}