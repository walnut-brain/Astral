using System;
using System.Collections.Generic;
using Astral.Lavium;
using LanguageExt;
using LanguageExt.TypeClasses;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration
{
    public abstract class ConfigBase : IDisposable
    {
        protected ConfigBase(LawBook config)
        {
            Config = config;
        }
        protected LawBook Config { get; }


        public T Get<T>() => Config.Get<T>();

        public Option<T> TryGet<T>() => Config.TryGet<T>();

        public ILogger GetLogger(string category) => Config.Get<ILoggerFactory>().CreateLogger(category);
        public ILogger<T> GetLogger<T>() => Config.Get<ILoggerFactory>().CreateLogger<T>();

        public void SetOption<T>(T value)
            => Config.TryGet<Pred<T>>().Match(p =>
                {
                    if (p.True(value))
                        Config.RegisterAxiom(value);
                    else
                        throw new ArgumentException($"{value} not satisfy {p.GetType().Name}");
                },
                () => Config.RegisterAxiom(value));

        public Pred<T> GetPredicate<T>()
            => Config.GetExtenalFact<Pred<T>>();
            

        public void SetOptionPredicate<TOption>(Pred<TOption> predicate)
            => Config.SetExternalFact(predicate);
        
        private Law WithPredicateControl(Law law)
        {
            return law.Map(lw =>
                (logger, paramAccess, args) =>
                {
                    var results = lw.Executor(logger, paramAccess, args);
                    return results.Map((i, o) =>
                    {
                        var outType = law.Findings[i];
                        var predType = typeof(Pred<>).MakeGenericType(outType);
                        var method = predType.GetMethod(nameof(Pred<Unit>.True));
                        var pred = paramAccess(predType);
                        if (pred == null) return o;
                        if ((bool) method.Invoke(pred, new[] {o})) return o;
                        logger.LogWarning("{value} not satisfy predicate {pred}", o, pred.GetType().Name);
                        return null;
                    }).ToArr();
                });
        }

        public void SetLaw(Law law) => Config.RegisterLaw(WithPredicateControl(law));
        public void SetLawSet(IEnumerable<Law> lawSet) => lawSet.Iter(SetLaw);

        public void Dispose()
        {
            Config.Dispose();
        }
    }

    public abstract class EndpointLevelConfigBase : ConfigBase
    {
        protected EndpointLevelConfigBase(LawBook config) : base(config)
        {
        }
    }
}