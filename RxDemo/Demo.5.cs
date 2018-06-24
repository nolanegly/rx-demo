using System;
using System.Reactive.Linq;
using System.Threading;

namespace RxDemo
{
    public partial class Demo
    {
        // So how do we stop observing an infinite observable? By disposing of the IDisposable returned by Subscribe.
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
        // This is probably most useful when adding Reactives into code using cancellation tokens already.
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
    }
}