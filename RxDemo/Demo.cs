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
    public class Demo
    {
        // create an observable and subscribe to it
        // subscribe method takes an action receiving the observable type, and is invoked as values are published
        // Observable.Range by default uses thread of caller
        public void One()
        {
            var range = Observable.Range(1, 10);
            var disposable = range.Subscribe(
                onNext => Console.WriteLine($"{onNext}"),
                onComplete => Console.WriteLine("Observable is done"));
        }

        // Add a scheduler that will cause observable subscription to run on a different thread,
        // and see how caller continues running immediately
        public void Two()
        {
            var range = Observable.Range(1, 10, new NewThreadScheduler());
            var disposable = range.Subscribe(
                onNext => Console.WriteLine($"{onNext}"), 
                onComplete => Console.WriteLine("Observable is done"));
        }

        // If you await an Observable, you only get the last completion value
        public async Task Three_A()
        {
            Console.WriteLine("getting...");
            var range = await Observable.Range(1, 9999999);
            Console.WriteLine(range);
        }

        // Only get last completion value even if it's on a different thread (just faster to complete)
        public async Task Three_B()
        {
            Console.WriteLine("getting...");
            var range = await Observable.Range(1, 9999999, new NewThreadScheduler());
            Console.WriteLine(range);
        }

        // This observable generates an incrementing counter at regular time intervals, uses another thread by default.
        // We're using Take to reduce the number values we get, othewise it'd never terminate
        public void Four()
        {
            var interval = Observable.Interval(new TimeSpan(0, 0, 0, 1))
                .Take(5);
            interval.Subscribe(
                onNext => Console.WriteLine($"{onNext}"),
                onComplete => Console.WriteLine("Observable is done"));
            Console.WriteLine("Done subscribing");
        }

        // So how do we stop an infinite observable? By disposing of the Subscribe return object
        // Notice how the observable never completes.
        public void Five_A()
        {
            var interval = Observable.Interval(new TimeSpan(0, 0, 0, 1));
            var disposable = interval.Subscribe(
                onNext => Console.WriteLine($"{onNext}"),
                onComplete => Console.WriteLine("Observable is done"));
            Thread.Sleep(5000);
            Console.WriteLine("Disposing");
            disposable.Dispose();
        }

        // You can terminate a neverending subscription with a cancellation token instead of a disposable.
        // This is probably most useful when adding Reactives into Task based methods.
        public void Five_B()
        {
            var cancelSource = new CancellationTokenSource();
            var interval = Observable.Interval(new TimeSpan(0, 0, 0, 1));
            interval.Subscribe(
                    onNext => Console.WriteLine($"{onNext}"),
                    onComplete => Console.WriteLine("Observable is done"),
                    cancelSource.Token); // return type is void when passing in a token
            Thread.Sleep(5000);
            Console.WriteLine("Cancelling");
            cancelSource.Cancel();
        }

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

            // You can also convert Tasks to Observables
            var task = Task.Factory.StartNew(() => "Hello world!");
            var taskObservable = task.ToObservable();
            taskObservable.Subscribe(
                onNext => Console.WriteLine($"{onNext}"),
                onCompleted => Console.WriteLine("Completed")); // This is never called? Interesting
        }


        // Let's make our own observable completely from scratch.
        // Note when it runs, we have to call OnCompleted before execution continues past the Subscribe call.
        public void Seven_A()
        {
            var observable = Observable.Create(
                (IObserver<int> o) =>
                {
                    o.OnNext(1); Thread.Sleep(100);
                    o.OnNext(2); Thread.Sleep(100);
                    o.OnNext(3); Thread.Sleep(100);
                    o.OnNext(4); Thread.Sleep(100);
                    o.OnNext(5); Thread.Sleep(100);
                    o.OnNext(6); Thread.Sleep(100);
                    o.OnNext(7); Thread.Sleep(100);
                    o.OnNext(8); Thread.Sleep(100);
                    o.OnNext(9); Thread.Sleep(100);
                    o.OnNext(10); Thread.Sleep(100);
                    o.OnCompleted();
                    return Disposable.Create(() => Console.WriteLine("Observer disposed of subscription"));
                });

            var disposable = observable.Subscribe(
                    onNext => Console.WriteLine($"{onNext}"),
                    onComplete => Console.WriteLine("Observable is done"));
            Console.WriteLine("Done subscribing");
        }

        // Here's a more realistic example of emitting values over time, but it has a problem.
        public void Seven_B()
        {
            var observable = Observable.Create(
                (IObserver<string> o) =>
                {
                    var timer = new System.Timers.Timer {Interval = 1000};
                    timer.Elapsed += OnTimerElapsed;
                    timer.Elapsed += (s, e) => o.OnNext("tick");
                    timer.Start();
                    return Disposable.Empty;
                });

            var disposable = observable.Subscribe(
                onNext => Console.WriteLine($"{onNext}"),
                onComplete => Console.WriteLine("Observable is done"));
            Console.WriteLine("Done subscribing");

            Thread.Sleep(5000);
            disposable.Dispose();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine($"Timer says {e.SignalTime}");
        }

        // Don't forget to clean up any allocated resources inside your observable
        public void Seven_C()
        {
            var observable = Observable.Create(
                (IObserver<string> o) =>
                {
                    var timer = new System.Timers.Timer {Interval = 1000};
                    timer.Elapsed += OnTimerElapsed;
                    timer.Elapsed += (s, e) => o.OnNext("tick");
                    timer.Start();
                    return Disposable.Create(() =>
                    {
                        timer.Elapsed -= OnTimerElapsed;
                        timer.Dispose();
                    });
                });

            var disposable = observable.Subscribe(
                onNext => Console.WriteLine($"{onNext}"),
                onComplete => Console.WriteLine("Observable is done"));
            Console.WriteLine("Done subscribing");

            Thread.Sleep(5000);
            disposable.Dispose();
        }

        // Error handling. Be sure to catch your exceptions and invoke OnError.
        public void Eight()
        {
            var observable = Observable.Create(
                (IObserver<int> o) =>
                {
                    o.OnNext(1); Thread.Sleep(100);
                    o.OnNext(2); Thread.Sleep(100);
                    try
                    {
                        throw new OverflowException("Overflowed the counter");
                    }
                    catch (Exception ex)
                    {
                        o.OnError(ex);
                    }
                    o.OnNext(4); Thread.Sleep(100);
                    o.OnNext(5); Thread.Sleep(100);

                    o.OnCompleted();
                    return Disposable.Create(() => Console.WriteLine("Disposal called automatically when error occurs"));
                });

            var disposable = observable.Subscribe(
                onNext => Console.WriteLine($"{onNext}"),
                onError => Console.WriteLine($"Error: {onError}"),
                () => Console.WriteLine("Observable is done")
                );
            Console.WriteLine("Done subscribing");
        }

        // Do is a prehook, and could be applied to your observable before giving it out for other code to consume.
        // Put any behavior with side effects in Do. Most likely, you'll use it for logging.
        public void Nine()
        {
            var range = Observable.Range(1, 10)
                .Do(
                    onNext => Console.WriteLine($"LOGGING: {onNext}"),
                    onExcept => Console.WriteLine($"LOGGING: Observable exception {onExcept.Message}"),
                    ()=> Console.WriteLine($"LOGGING: Observable completed")
                );

            var disposable = range.Subscribe(
                onNext => Console.WriteLine($"{onNext}"),
                onComplete => Console.WriteLine("Observable is done"));
        }

        // You can create multiple subscriptions on an observable, each with their own pipeline
        public void Ten_A()
        {
            var range = Observable.Range(1, 10);

            var multiplesOfTwo = range.Where(i => i % 2 == 0);
            multiplesOfTwo.Subscribe(
                onNext => Console.WriteLine($"multiple of two: {onNext}"));

            var multiplesOfThree = range.Where(i => i % 3 == 0);
            multiplesOfThree.Subscribe(
                onNext => Console.WriteLine($"multiple of three: {onNext}"));
        }

        // You can also create derivation chains with observables, e.g. a -> b -> c
        public void Ten_B()
        {
            var range = Observable.Range(1, 10);

            var firstSeven = range.Take(7);
            firstSeven.Subscribe(onNext => Console.WriteLine($"take 7: {onNext}"));

            var odds = firstSeven.Where(i => i % 2 != 0);
            odds.Subscribe(onNext => Console.WriteLine($"odds: {onNext}"));
        }
    }
}