using System;
using System.Reactive.Linq;

namespace RxDemo
{
    public partial class Demo
    {
        // You can create multiple subscriptions on an observable, each with their own pipeline.
        public void Nine_A()
        {
            var range = Observable.Range(1, 10);

            var multiplesOfTwo = range.Where(i => i % 2 == 0);
            multiplesOfTwo.Subscribe(
                onNext => Console.WriteLine($"multiple of two: {onNext}"));

            var multiplesOfThree = range.Where(i => i % 3 == 0);
            multiplesOfThree.Subscribe(
                onNext => Console.WriteLine($"multiple of three: {onNext}"));
        }

        // You can also create derivation chains with observables, e.g. a-> b -> c
        public void Nine_B()
        {
            var range = Observable.Range(1, 10);

            var firstSeven = range.Take(7);
            firstSeven.Subscribe(onNext => Console.WriteLine($"take 7: {onNext}"));

            var odds = firstSeven.Where(i => i % 2 != 0);
            odds.Subscribe(onNext => Console.WriteLine($"odds: {onNext}"));
        }
    }
}