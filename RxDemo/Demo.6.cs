using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace RxDemo
{
    public partial class Demo
    {
        // So far we've been looking at pre-made observable sources, but we want to be able to make our own.
        // There are a variety of observable creation methods to integrate with existing code.
        // Start allows you to run a long running action and complete with a single result.
        // The return result with be Unit, which is functionally equivalant to void.
        public void Six_A()
        {
            var actionObservable = Observable.Start(() =>
            {
                for (var ix = 0; ix < 10; ix++)
                {
                    Console.WriteLine(ix);
                    Thread.Sleep(100);
                }
            });

            actionObservable.Subscribe(
                onNext => Console.WriteLine("Received unit (void equivalent)"),
                onCompleted => Console.WriteLine("Completed") // This is never called? Interesting.
            );
        }

        // Start also allows you to run a long running action or function, and complete with a single result.
        public void Six_B()
        {
            var funcObservable = Observable.Start(() =>
            {
                for (var ix = 0; ix < 10; ix++)
                {
                    Console.WriteLine(ix);
                    Thread.Sleep(100);
                }

                return 99;
            });

            funcObservable.Subscribe(
                onNext => Console.WriteLine($"Received {onNext}"),
                onCompleted => Console.WriteLine("Completed") // This is never called? Interesting.
            );
        }

        public void Six_C()
        {
            // These allow you to turn .NET events into Observables. Every invoke triggers OnNext
            // var eventObservable = Observable.FromEvent(addHandlerAction, RemoveHandlerAction, optionalScheduler);
            
            // Same, but for the .NET standard event (sender, eventArgs) pattern
            // var eventPatternObservable = Observable.FromEventPattern();

            // Also - enumerables (but think about why you're doing it)
            // Also - Ye Olde Asynchronous Programming Model (APM) pattern, looks like BeginFoo() and EndFoo()

            // You can also convert Tasks to Observables with an extension method
            var task = Task.Factory.StartNew(() => "Hello world!");
            var taskObservable = task.ToObservable();
            taskObservable.Subscribe(
                onNext => Console.WriteLine($"{onNext}"),
                onCompleted => Console.WriteLine("Completed")); // This is never called? Interesting
        }
    }
}