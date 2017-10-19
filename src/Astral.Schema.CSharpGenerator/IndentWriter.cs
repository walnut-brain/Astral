using System;
using System.Reactive.Disposables;
using System.Text;

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
            return Disposable.Create(() => _indent--);
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
    }
}
