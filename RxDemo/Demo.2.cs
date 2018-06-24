using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace RxDemo
{
    public partial class Demo
    {
        // Here we're adding a scheduler that will cause the observable to run on a different thread,
        // and see how the observer continues running immediately.
        public void Two()
        {
            var range = Observable.Range(1, 10, new NewThreadScheduler());
            var disposable = range.Subscribe(
                onNext => Console.WriteLine($"{onNext}"), 
                onComplete => Console.WriteLine("Observable is done"));
        }
    }
}