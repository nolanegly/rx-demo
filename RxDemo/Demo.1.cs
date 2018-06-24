using System;
using System.Reactive.Linq;

namespace RxDemo
{
    public partial class Demo
    {
        // Here's a simple example of creating an observable and subscribing to it.
        // The subscribe method takes a couple of action methods.
        // The first is receiving the observable type, and is invoked as each value published.
        // The second is invoked whenever the observable successfully completes.
        // Note that Observable.Range is (by default) using the thread of the observer (subscriber).
        // Let's take a look at how we can customize thread handling in the next example.
        public void One()
        {
            var range = Observable.Range(1, 10);
            var disposable = range.Subscribe(
                onNext => Console.WriteLine($"{onNext}"),
                onComplete => Console.WriteLine("Observable is done"));
        }
    }
}