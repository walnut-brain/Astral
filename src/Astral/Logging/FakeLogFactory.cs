using System;

namespace Astral.Logging
{
    public class FakeLogFactory : ILogFactory
    {
        private readonly ILogFactory _adapter;
        
        public FakeLogFactory()
        {
            _adapter = new LogFactoryAdapter(new FakeLoggerFactory());
        }

        public ILog CreateLog(string category) => _adapter.CreateLog(category);


        public ILog CreateLog<T>() => _adapter.CreateLog<T>();

        public ILog CreateLog(Type type) => _adapter.CreateLog(type);
    }
}