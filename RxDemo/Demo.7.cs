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
        // Let's make our own observable completely from scratch, and mimic the Range observable we've been using.
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
                    return Disposable.Create(() => Console.WriteLine("Subscription disposed"));
                });

            var disposable = observable.Subscribe(
                    onNext => Console.WriteLine($"{onNext}"),
                    onComplete => Console.WriteLine("Observable is done"));
            Console.WriteLine("Done subscribing");
        }







        // Here's a more realistic example of emitting values over time, but it has a problem at runtime.
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








        // Don't forget to clean up any allocated resources inside your observable!
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








        // Error handling. Be sure to catch your exceptions and invoke OnError, so subscribers
        // know something went wrong.
        // This is the first example where we see all three actions being handled by the observer.
        public void Seven_D()
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
    }
}