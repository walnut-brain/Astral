using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using Astral.Lavium.Internals;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace Astral.Lavium
{
    public class LawBook : IDisposable
    {
        
        #region External interface

        public LawBook(ILogger logger, int maxRetreat = 100) : this(logger, None, None, Disposable.Empty, false, maxRetreat)
        {
            
        }

        public Option<LawBook> Parent { get; private set; }

        public Option<object> Key { get; }
        

        public void RegisterLaw(Law law)
        {
            _logger.Checked().LogActivity(
                () =>
                {
                    if (_disposed) throw new ObjectDisposedException(nameof(LawBook));
                    if (law == null) throw new ArgumentNullException(nameof(law));
                    lock (_locker)
                    {
                        _myLaws = _myLaws.Add(new Law(law.Name, law.Arguments, law.Findings, law.Executor));
                        _version++;
                    }
                },
            "Register law {name} in {key} {thread}", law.Name, Key , Thread.CurrentThread.ManagedThreadId);
        }

        internal void RegisterAxiom(Type type, object value, bool externallyOwned)
        {
            _logger.Checked().LogActivity(() =>
                {
                    if (_disposed) throw new ObjectDisposedException(nameof(LawBook));
                    if (type == null) throw new ArgumentNullException(nameof(type));
                    if (value == null) throw new ArgumentNullException(nameof(value));
                    if (!type.IsInstanceOfType(value))
                        throw new ArgumentException($"{value} is not instance of {type}");

                    var newAxiom = new Axiom(type, value, externallyOwned);
                    lock (_locker)
                    {
                        _myAxiom = _myAxiom.Find(p => p.Id == type).Match(p =>
                        {
                            if (!p.ExternallyOwned && p.Value is IDisposable disp)
                                disp.Dispose();
                            return _myAxiom.Remove(p).Add(newAxiom);
                        }, () => _myAxiom.Add(newAxiom));
                        _version++;
                    }
                }, "Register axiom {type} {value} in {key} {thread}", type, value, Key,
                Thread.CurrentThread.ManagedThreadId);
        }

        internal Option<object> GetFact(Type type)
        {
            return _logger.Checked().LogActivity(() =>
                {
                    if (_disposed) throw new ObjectDisposedException(nameof(LawBook));
                    Recalculate();
                    lock (_locker)
                        return _factsIndex.Find(type);
                },
                "Reading fact {type} in {key} {thread}", type, Key, Thread.CurrentThread.ManagedThreadId);
        }



        public LawBook GetOrAddBook(object bookId, bool permanent = true)
        {
            if(_disposed) throw new ObjectDisposedException(nameof(LawBook));
            if (bookId == null) throw new ArgumentNullException(nameof(bookId));
            return _chieldren.GetOrAdd(bookId, _ => new LawBook(_logger, this, Some(bookId), Disposable.Create(() => _chieldren.TryRemove(bookId, out var _)), permanent, _maxRetreat));
        }

        

        public bool IsPermanent { get; }

        public bool Disposed => _disposed;

        public void Dispose() => Dispose(false);

        internal object GetExternalFact(Type key)
        {
            return _externalFacts.TryGetValue(key, out var res) 
                ? res 
                : Parent.MatchUnsafe(p => p.GetExternalFact(key), () => null);
        }

        internal void SetExternalFact(Type key, object value)
        {
            _externalFacts.TryAdd(key, value);
        }

        #endregion

        #region Private methods

        private LawBook(ILogger logger, Option<LawBook> parent, Option<object> key, IDisposable disposer, bool preventDirectDispose, int maxRetreat)
        {
            _disposer = disposer;
            IsPermanent = preventDirectDispose;
            _maxRetreat = maxRetreat;
            Key = key;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Parent = parent;
        }

        

        private void Recalculate()
        {
            lock (_locker)
            {
                while (NeedRecalculate())
                {
                    RecalculateStep();
                }
            }
        }

        private bool NeedRecalculate()
        {
            lock(_locker)
                return _version != _calculatedVersion || Parent.Match(p => p._locker.Lock(() => p._version != _parentVersion || p.NeedRecalculate()), () => false);
        }

        private void RecalculateStep()
        {

            var parentSnapshot = Parent.Match(p => p.MakeSnapshot(),
                    () => new BookSnapshot(0, Arr<Law>.Empty, Arr<Axiom>.Empty));

            _ancestorsLaws = parentSnapshot.Laws;
            Dictionary<Type, Axiom> facts;

            var axioms = _myAxiom.ToDictionary(p => p.Id, p => p);
            var laws = _ancestorsLaws.Select(p => new LawRec(0, !p.Arguments.Any(t => axioms.ContainsKey(t)), p))
                .ToList();
            laws.AddRange(_myLaws.Select(p => new LawRec(0, false, p)));
            // ordering laws
            laws.Iter((i, p) => p.Order = i);
            // indexing laws
            var lawIndex = new Map<int, int>(laws.Select(p => (p.Law.Id, p.Order)));

            // initial facts
            facts = parentSnapshot.Facts.ToDictionary(p => p.Id, p => p);
            foreach (var axiom in _myAxiom)
            {
                facts[axiom.Id] = axiom;
            }

            var retreat = 0;
            var current = 0;
            while (current < laws.Count)
            {
                var law = laws[current];
                if (law.Processed || !LawProcessing.CanExecuteLaw(law.Law, facts) || !LawProcessing.HaveMeaningExecuteLaw(law, facts, lawIndex))
                {
                    current++;
                    continue;
                }
                ProcessLaw(law);
                var unprocessed = laws.FirstOrDefault(p => !p.Processed);
                if (unprocessed != null)
                    current = unprocessed.Order;
                retreat++;
                if(retreat > _maxRetreat) throw new InvalidOperationException($"Laws are to difficult or cicled. Not resolved in {retreat} attempt!");
            }

            _facts = facts.Values.ToArr();
            _factsIndex = new HashMap<Type, object>(_facts.Select(p => (p.Id, p.Value)));
            _calculatedVersion = _version;
            _parentVersion = parentSnapshot.Version;

            void ProcessLaw(LawRec law)
            {
                _logger.Checked().LogTrace("Begin process law {name}", law.Law.Name);
                var args = law.Law.Arguments.Map(p => facts[p].Value).ToArr();
                var results = law.Law.Executor(_logger, GetExternalFact, args);
                for (var i = 0; i < results.Count; i++)
                {
                    var id = law.Law.Findings[i];
                    var result = results[i];
                    _logger.Checked().LogTrace("Calculated {type} {value}", id, result);

                    if (Equals(result, null)) continue;
                    
                    if (!id.IsInstanceOfType(result))
                        throw new InvalidOperationException($"Invalid type of law result {law.Law.Name} - must be {id}, but is {result.GetType()}, index {i}");
                    if (facts.ContainsKey(id) &&
                        !(facts[id] is Inference inf && lawIndex[inf.LawId] <= law.Order))
                        continue;
                    if (!facts.ContainsKey(id))
                    {
                        facts.Add(id, new Inference(id, result, true, law.Law.Id));
                        _logger.Checked().LogTrace("Added {type} {value}", id, result);
                        continue;
                    }
                    if (!Equals(facts[id].Value, result))
                    {
                        var old = facts[id].Value;
                        facts[id] = new Inference(id, result, true, law.Law.Id);
                        laws.Where(p => p.Law.Arguments.Contains(id)).Iter(p => p.Processed = false);
                        _logger.Checked().LogTrace("Changed {type} {old} => {value}", id, old, result);
                    }
                    else
                    {
                        if (((Inference) facts[id]).LawId != law.Law.Id)
                        {
                            facts[id] = new Inference(id, result, true, law.Law.Id);
                        }
                    }
                }
                law.Processed = true;
            }
        }




        internal BookSnapshot MakeSnapshot()
        {
            lock (_locker)
            {
                Recalculate();
                var lawCopy = _ancestorsLaws + _myLaws;
                var factCopy = _facts;
                return new BookSnapshot(_version, lawCopy, factCopy);
            }
        }
        

        private Arr<Law> _ancestorsLaws = Arr<Law>.Empty;
        private readonly int _maxRetreat;
        private volatile bool _disposed;
        private readonly IDisposable _disposer;
        private readonly ILogger _logger;
        private Arr<Law> _myLaws = Arr<Law>.Empty;
        private Arr<Axiom> _myAxiom = Arr<Axiom>.Empty;
        private readonly ConcurrentDictionary<object, LawBook> _chieldren = new ConcurrentDictionary<object, LawBook>();
        private Arr<Axiom> _facts = Arr<Axiom>.Empty;
        private HashMap<Type, object> _factsIndex = HashMap<Type, object>();
        private readonly ConcurrentDictionary<Type, object> _externalFacts = new ConcurrentDictionary<Type, object>();
        
        private readonly object _locker = new object();

        
        private int _version = -1;
        
        
        private int _calculatedVersion;
        
        
        private int _parentVersion;

        

        

        private void Dispose(bool byParent)
        {
            if(!byParent && IsPermanent) return;
            if (_disposed) return;
            _disposed = true;
            while (!_chieldren.IsEmpty)
            {
                var pair = _chieldren.First();
                pair.Value.Dispose(true);
                _chieldren.TryRemove(pair.Key, out var _);

            }
            _locker.Lock(() =>
            {
                _myLaws.Clear();
                _myAxiom.Where(p => !p.ExternallyOwned).Map(p => p.Value).OfType<IDisposable>().Iter(p => p.Dispose());
                _myAxiom.Clear();
                _factsIndex.Clear();
                _facts = _facts.Clear();
                Parent = Option<LawBook>.None;
                _disposer.Dispose();
            });
        }

        ~LawBook()
        {
            Dispose(true);
        }
        
        #endregion
    }
}