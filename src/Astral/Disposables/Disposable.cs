using System;

namespace Astral.Disposables
{
    public static class Disposable
    {
        private class EmptyDisposable : IDisposed
        {
            public void Dispose()
            {
                
            }

            public bool IsDisposed => true;
        }
        
        public static IDisposed Empty = new EmptyDisposable();
        
        public static IDisposed Create(Action onDispose) => new DelegateDisposable(onDispose);
    }
}