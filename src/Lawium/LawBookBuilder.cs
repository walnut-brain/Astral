using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using FunEx;
using FunEx.Monads;
using Microsoft.Extensions.Logging;


namespace Lawium
{
    /// <summary>
    /// Law book builder
    /// </summary>
    public class LawBookBuilder
    {
        private readonly ushort _maxRecursion;

        /// <summary>
        /// Maximum level of inference recursion
        /// </summary>
        //public static volatile ushort MaxRecursion = 100;

        private readonly Func<IReadOnlyList<Law>> _parentLaws;
        private readonly IReadOnlyDictionary<Type, Inference> _parentFacts;
        private readonly string _path = "";
        private readonly Dictionary<object, LawBookBuilder> _subBuilders = new Dictionary<object, LawBookBuilder>();

        /// <summary>
        /// Create law book builder
        /// </summary>
        /// <param name="loggerFactory">logger factory for logging inference process</param>
        public LawBookBuilder(ILoggerFactory loggerFactory = null, ushort maxRecursion = 100)
        {
            _maxRecursion = maxRecursion;
            LoggerFactory = loggerFactory ?? new FakeLoggerFactory();
            _parentLaws = () => new List<Law>();
        }

        internal LawBookBuilder(ILoggerFactory loggerFactory, ushort maxRecursion, Func<IReadOnlyList<Law>> parentLaws, string path)
        {
            LoggerFactory = loggerFactory;
            _maxRecursion = maxRecursion;
            _parentLaws = parentLaws;
            _path = path;
        }
        
        internal LawBookBuilder(ILoggerFactory loggerFactory, ushort maxRecursion, IReadOnlyList<Law> parentLaws, IReadOnlyDictionary<Type, Inference> parentFacts, string path)
        {
            LoggerFactory = loggerFactory;
            _parentLaws = () => parentLaws;
            _maxRecursion = maxRecursion;
            _parentFacts = parentFacts;
            _path = path;
        }

        private readonly List<Law> _laws = new List<Law>();

        private List<LawRec> GetLaws() => 
            _parentFacts == null 
                ?  _parentLaws().Union(_laws).Select(p => new LawRec(p)).ToList()
                : _parentLaws().Select(p => new LawRec(p, true)).Union(_laws.Select(p => new LawRec(p))).ToList();

        /// <summary>
        /// Logger factory
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }

        
        /// <summary>
        /// Register law in builder
        /// </summary>
        /// <param name="law">law</param>
        public void RegisterLaw(Law law)
        {
            _laws.Add(law);
        }


        /// <summary>
        /// Get sub book builder
        /// </summary>
        /// <param name="key">sub book key</param>
        /// <param name="onCreate">on create law registration</param>
        /// <returns>sub book builder</returns>
        public LawBookBuilder GetSubBookBuilder(object key, Action<LawBookBuilder> onCreate = null)
        {
            if (_subBuilders.TryGetValue(key, out var sub))
                return sub;
            sub = new LawBookBuilder(LoggerFactory, _maxRecursion, () => GetLaws().Select(p => p.Law).ToList(), _path + "/" + key);
            _subBuilders.Add(key, sub);
            onCreate?.Invoke(sub);
            return sub;
        }

        /// <summary>
        /// build law book 
        /// </summary>
        /// <returns>law book</returns>
        public LawBook Build()
        {
            var logger = LoggerFactory.CreateLogger<LawBookBuilder>();
            using (logger.BeginScope("{path}", _path))
            {
                if (_parentFacts != null && _laws.Count == 0)
                {
                    logger.LogTrace("No laws in builder, use parent facts");
                    return new LawBook(LoggerFactory, _path,
                        GetLaws().Select(p => p.Law).ToImmutableArray(),
                        _parentFacts,
                        _subBuilders.ToDictionary(p => p.Key, p => p.Value.Build()));
                }
                try
                {
                    var maxRecursion = _maxRecursion;
                    var loopNum = 0;
                    var laws = GetLaws();
                    var inferences = 
                        _parentFacts?.ToDictionary(p => p.Key, p => p.Value) ?? new Dictionary<Type, Inference>();


                    // process axioms
                    foreach (var (law, index) in laws.Where(p => p.Law.Arguments.Length == 0).Select((p, i) => (p, i)))
                    {
                        if(law.Processed) continue;
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
                                            throw new InvalidOperationException(
                                                $"{result[i]} is not {law.Law.Findings[i]}");
                                        inferences[law.Law.Findings[i]] = new Inference(result[i], index);
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

                    var current = laws.FindIndex(p => !p.Processed);
                    while (loopNum < maxRecursion && current >= 0)
                    {
                        var law = laws[current];
                        if (law.Processed)
                        {
                            current++;
                            continue;
                        }
                        // Skip not enough arguments
                        if (!law.Law.Arguments.All(p => inferences.ContainsKey(p) || p.OptionOf().IsSome))
                        {
                            current++;
                            continue;
                        }
                        // Skip lated
                        var ind1 = current;
                        if (law.Law.Findings.All(p =>
                            inferences
                                .TryGetValue(p)
                                .Map(t => t.LawIndex > ind1)
                                .IfNone(() => false)))
                        {
                            law.Processed = true;
                            current++;
                            continue;
                        }
                        var args = law.Law.Arguments.Map(p =>
                        {
                            if (inferences.TryGetValue(p, out var val)) return val.Value;
                            var optionOf = p.OptionOf();
                            if (optionOf.IsNone) throw new InvalidOperationException();
                            return (object) optionOf.Bind(t => inferences.TryGetValue(t).Map(k => k.Value));
                        }).ToImmutableArray();
                        using (logger.BeginScope("{law}", law.Law.Name))
                        {
                            try
                            {
                                logger.LogTrace("Processing");
                                var result = law.Law.Executor(logger, args);
                                law.Processed = true;
                                for (var i = 0; i < law.Law.Findings.Length; i++)
                                    if (result[i] != null)
                                    {
                                        if (!law.Law.Findings[i].IsInstanceOfType(result[i]))
                                            throw new InvalidOperationException(
                                                $"{result[i]} is not {law.Law.Findings[i]}");
                                        if (inferences.TryGetValue(law.Law.Findings[i], out var currentValue))
                                        {
                                            if (Equals(currentValue.Value, result[i])) continue;
                                            if (currentValue.LawIndex > current) continue;
                                        }
                                        inferences[law.Law.Findings[i]] = new Inference(result[i], current);
                                        foreach (var (rec, num) in laws.Select((p, i1) => (p, i1)))
                                        {
                                            var optType = typeof(Option<>).MakeGenericType(law.Law.Findings[i]);
                                            if (rec.Law.Arguments.Contains(law.Law.Findings[i]) ||
                                                rec.Law.Arguments.Contains(optType))
                                            {
                                                var index = num;
                                                if (!rec.Law.Findings.All(p =>
                                                    inferences
                                                        .TryGetValue(p)
                                                        .Map(t => t.LawIndex > index)
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
                        var nxtLaw = laws.FindIndex(p => !p.Processed);
                        if (nxtLaw >=0 && nxtLaw <= current)
                        {
                            current = nxtLaw;
                            loopNum++;
                        }
                        else
                        {
                            current++;
                        }

                    }
                    if (loopNum >= maxRecursion)
                        throw new InvalidOperationException(
                            $"Maximal recursion {maxRecursion} reached on build law book {_path}");

                    return new LawBook(LoggerFactory, _path,
                        GetLaws().Select(p => p.Law).ToImmutableArray(),
                        new ReadOnlyDictionary<Type, Inference>(inferences),
                        _subBuilders.ToDictionary(p => p.Key, p => p.Value.Build()));

                }
                catch (Exception ex)
                {
                    logger.LogError(0, ex, "Build law book");
                    throw;
                }
            }
        }

        private class LawRec
        {
            public LawRec(Law law, bool processed = false)
            {
                Law = law;
                Processed = processed;
            }

            public Law Law { get; }
            public bool Processed { get; set; }
        }

        
    }
}