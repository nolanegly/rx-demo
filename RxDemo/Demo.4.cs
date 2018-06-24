using System;
using System.Reactive.Linq;

namespace RxDemo
{
    public partial class Demo
    {
        // Let's look at something besides a range of numbers.
        // This observable generates an incrementing counter at regular time intervals,
        // and it uses another thread by default.
        // We're using Take to reduce the number values we get, because Interval won't complete by itself.
        public void Four()
        {
            var interval = Observable.Interval(new TimeSpan(0, 0, 0, 1))
                .Take(5);
            interval.Subscribe(
                onNext => Console.WriteLine($"{onNext}"),
                onComplete => Console.WriteLine("Observable is done"));
            Console.WriteLine("Done subscribing");
        }
    }
}