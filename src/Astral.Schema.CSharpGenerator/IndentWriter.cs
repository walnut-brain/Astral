using System;
using System.Text;
using System.Threading;

namespace Astral.Schema.CSharpGenerator
{
    public class IndentWriter
    {
        private readonly string _indentString;
        private int _indent = 0;
        private readonly StringBuilder _builder;

        public IndentWriter(string indent = "    ")
        {
            _indentString = indent;
            _builder = new StringBuilder();
        }

        public IDisposable Indent()
        {
            _indent++;
            return new Disposable(() => _indent--);
        }

        public void WriteLine(string str = "")
        {
            for (var i = 0; i < _indent; i++)
                _builder.Append(_indentString);
            _builder.AppendLine(str);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        private class Disposable : IDisposable
        {
            private Action _action;
            private int _disposed;

            public Disposable(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                if(Interlocked.CompareExchange(ref _disposed, 1, 0) == 1)
                    return;
                _action();
                _action = null;
            }

            ~Disposable()
            {
                Dispose();
            }
        }
    }
}
