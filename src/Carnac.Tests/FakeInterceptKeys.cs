using System;
using System.Reactive.Subjects;
using Carnac.Logic.KeyMonitor;

namespace Carnac.Tests
{
    public class FakeInterceptKeys : IInterceptKeys
    {
        readonly IObservable<InterceptKeyEventArgs> interceptKeysSource;

        public FakeInterceptKeys(IObservable<InterceptKeyEventArgs> interceptKeysSource)
        {
            this.interceptKeysSource = interceptKeysSource;
        }

        public IObservable<InterceptKeyEventArgs> GetKeyStream()
        {
            return interceptKeysSource;
        }
    }
}