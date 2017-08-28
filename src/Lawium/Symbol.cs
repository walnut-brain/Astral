using System;

namespace Lawium
{
    /// <inheritdoc />
    public class Symbol<T> : ISymbol
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">symbol name</param>
        public Symbol(string name)
        {
            Name = name;
        }


        /// <inheritdoc />
        public Type Type => typeof(T);


        /// <inheritdoc />
        public string Name { get; }
    }
}