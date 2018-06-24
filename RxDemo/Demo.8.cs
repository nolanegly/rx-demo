using System;
using System.Reactive.Linq;

namespace RxDemo
{
    public partial class Demo
    {
        // Do is a prehook, and could be applied to your observable before giving it out for other code to consume.
        // Put any behavior with side effects in Do. Most likely, you'll use it for logging.
        public void Eight()
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
    }
}