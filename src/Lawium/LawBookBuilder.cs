using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using FunEx;
using Microsoft.Extensions.Logging;


namespace Lawium
{
    /// <summary>
    /// Law book builder
    /// </summary>
    public class LawBookBuilder<T>
    {
        /// <summary>
        /// Maximum level of inference recursion
        /// </summary>
        public static volatile ushort MaxRecursion = 100;

        private readonly Func<IEnumerable<Law<T>>> _parentLaws;
        private readonly string _path = "";
        private readonly Dictionary<object, LawBookBuilder<T>> _subBuilders = new Dictionary<object, LawBookBuilder<T>>();

        /// <summary>
        /// Create law book builder
        /// </summary>
        /// <param name="loggerFactory">logger factory for logging inference process</param>
        public LawBookBuilder(ILoggerFactory loggerFactory = null)
        {
            LoggerFactory = loggerFactory ?? new FakeLoggerFactory();
            _parentLaws = Enumerable.Empty<Law<T>>;
        }

        internal LawBookBuilder(ILoggerFactory loggerFactory, Func<IEnumerable<Law<T>>> parentLaws, string path)
        {
            LoggerFactory = loggerFactory;
            _parentLaws = parentLaws;
            _path = path;
        }

        private readonly List<Law<T>> _laws = new List<Law<T>>();

        private IEnumerable<Law<T>> GetLaws() => _parentLaws().Union(_laws);

        /// <summary>
        /// Logger factory
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }

        
        /// <summary>
        /// Register law in builder
        /// </summary>
        /// <param name="law">law</param>
        public void RegisterLaw(Law<T> law)
        {
            _laws.Add(law);
        }


        /// <summary>
        /// Get sub book builder
        /// </summary>
        /// <param name="key">sub book key</param>
        /// <param name="onCreate">on create law registration</param>
        /// <returns>sub book builder</returns>
        public LawBookBuilder<T> GetSubBookBuilder(object key, Action<LawBookBuilder<T>> onCreate = null)
        {
            if (_subBuilders.TryGetValue(key, out var sub))
                return sub;
            sub = new LawBookBuilder<T>(LoggerFactory, GetLaws, _path + "/" + key);
            _subBuilders.Add(key, sub);
            onCreate?.Invoke(sub);
            return sub;
        }

        /// <summary>
        /// build law book 
        /// </summary>
        /// <returns>law book</returns>
        public LawBook<T> Build()
        {
            var logger = LoggerFactory.CreateLogger<LawBookBuilder<T>>();
            using (logger.BeginScope("{path}", _path))
            {
                try
                {
                    var maxRecursion = MaxRecursion;
                    var loopNum = 0;
                    var laws = GetLaws().Select((p, n) => new LawRec(n, false, p)).ToList();
                    IDictionary<Type, Inference> inferences = new Dictionary<Type, Inference>();
                    // process axioms
                    foreach (var law in laws.Where(p => p.Law.Arguments.Length == 0))
                    {
                        using (logger.BeginScope("{law}", law.Law.Name))
                        {
                            try
                            {
                                logger.LogTrace("Processing");
                                var result = law.Law.Executor(logger, ImmutableArray<object>.Empty);
                                for (var i = 0; i < law.Law.Findings.Length; i++)
                                    if (result[i] != null)
                                    {
                                        if (!law.Law.Findings[i].IsInstanceOfType(result[i]))
                                            throw new InvalidOperationException($"{result[i]} is not {law.Law.Findings[i]}");
                                        inferences[law.Law.Findings[i]] = new Inference((T) result[i], law);
                                        logger.LogTrace("Writing {type} {value}", law.Law.Findings[i], result[i]);
                                    }
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(0, ex, "Error processing");
                                throw;
                            }
                        }
                        law.Processed = true;
                    }

                    var nextLaw = 0;
                    while (loopNum < maxRecursion && nextLaw < laws.Count)
                    {
                        var law = laws[nextLaw];
                        if (law.Processed)
                        {
                            nextLaw++;
                            continue;
                        }
                        // Skip no enough arguments
                        if (!law.Law.Arguments.All(p => inferences.ContainsKey(p)))
                        {
                            nextLaw++;
                            continue;
                        }
                        // Skip lated
                        if (law.Law.Findings.All(p =>
                            inferences
                                .TryGetValue(p)
                                .Map(t => t.Law.Index > nextLaw)
                                .IfNone(() => false)))
                        {
                            law.Processed = true;
                            nextLaw++;
                            continue;
                        }
                        var args = law.Law.Arguments.Map(p => inferences[p].Value);
                        using (logger.BeginScope("{law}", law.Law.Name))
                        {
                            try
                            {
                                logger.LogTrace("Processing");
                                var result = law.Law.Executor(logger, ImmutableArray<object>.Empty);
                                law.Processed = true;
                                for (var i = 0; i < law.Law.Findings.Length; i++)
                                    if (result[i] != null)
                                    {
                                        if (inferences.TryGetValue(law.Law.Findings[i], out var current))
                                        {
                                            if(Equals(current.Value, result[i])) continue;
                                            if (current.Law.Index > nextLaw) continue;
                                        }
                                        if (!law.Law.Findings[i].IsInstanceOfType(result[i]))
                                            throw new InvalidOperationException($"{result[i]} is not {law.Law.Findings[i]}");
                                        inferences[law.Law.Findings[i]] = new Inference((T) result[i], law);
                                        foreach (var rec in laws)
                                        {
                                            if (rec.Law.Arguments.Contains(law.Law.Findings[i]))
                                            {
                                                if (!law.Law.Findings.All(p =>
                                                    inferences
                                                        .TryGetValue(p)
                                                        .Map(t => t.Law.Index > nextLaw)
                                                        .IfNone(() => false)))
                                                {
                                                    rec.Processed = false;
                                                }
                                            }
                                        }
                                        logger.LogTrace("Writing {type} {value}", law.Law.Findings[i], result[i]);
                                    }
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(0, ex, "Error processing");
                                throw;
                            }
                        }
                        var minUnprocessed = laws.Where(p => !p.Processed).Min(p => (int?) p.Index);
                        if (minUnprocessed != null && minUnprocessed.Value <= nextLaw)
                        {
                            nextLaw = minUnprocessed.Value;
                            loopNum++;
                        }
                        else
                        {
                            nextLaw++;
                        }

                    }
                    if(loopNum >= maxRecursion)
                        throw new InvalidOperationException($"Maximal recursion {maxRecursion} reached on build law book {_path}");

                    return new LawBook<T>(LoggerFactory, _path, 
                        GetLaws().ToImmutableArray()
                        , 
                        new ReadOnlyDictionary<Type, T>(
                                inferences.ToDictionary(p => p.Key, p => p.Value.Value)), 
                        new ReadOnlyDictionary<object, LawBook<T>>(
                            _subBuilders.ToDictionary(p => p.Key, p => p.Value.Build())));

                }
                catch (Exception ex)
                {
                    logger.LogError(0, ex, "Build law book");
                    throw;
                }
            }

        }

        private class Inference
        {
            public Inference(T value, LawRec law)
            {
                Value = value;
                Law = law;
            }

            public T Value { get; }
            public LawRec Law { get; }
        }

        private class LawRec
        {
            public LawRec(int index, bool processed, Law<T> law)
            {
                Index = index;
                Processed = processed;
                Law = law;
            }

            public int Index { get; }
            public bool Processed { get; set; }
            public Law<T> Law { get; }
        }
    }
}