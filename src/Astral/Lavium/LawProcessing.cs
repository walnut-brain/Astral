using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Astral.Lavium.Internals;
using LanguageExt;

namespace Astral.Lavium
{
    internal static class LawProcessing
    {
        /// <summary>
        /// Have meaning execute law
        /// </summary>
        /// <param name="law">law record</param>
        /// <param name="facts">fact dictionary</param>
        /// <param name="index">map law id to law order</param>
        /// <returns></returns>
        [Pure]
        public static bool HaveMeaningExecuteLaw(LawRec law, IDictionary<Type, Axiom> facts, Map<int, int> index)
            => law.Law.Findings.Any(p => !facts.ContainsKey(p) || facts[p] is Inference inf &&  index[inf.LawId] <= law.Order);

        [Pure]
        public static bool CanExecuteLaw(Law law, IDictionary<Type, Axiom> facts) => law.Arguments.All(facts.ContainsKey);
    }
}