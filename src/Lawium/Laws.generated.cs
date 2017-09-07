
using System;
using System.Linq;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using FunEx;

namespace Lawium
{
	public partial class Law<T>
    {
		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TO1>(string name, Func<TI1, TO1> execute)		    
		    where TI1 : T		    
		    where TO1 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                object r1 = execute((TI1) args[0]);
                
                return ImmutableArray.Create<object>(r1);
			}

			var inArr = ImmutableArray.Create(typeof(TI1));
			var outArr = ImmutableArray.Create(typeof(TO1));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TO1, TO2>(string name, Func<TI1, (TO1, TO2)> execute)		    
		    where TI1 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2) = execute((TI1) args[0]);
                
                return ImmutableArray.Create<object>(r1, r2);
			}

			var inArr = ImmutableArray.Create(typeof(TI1));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TO1, TO2, TO3>(string name, Func<TI1, (TO1, TO2, TO3)> execute)		    
		    where TI1 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3) = execute((TI1) args[0]);
                
                return ImmutableArray.Create<object>(r1, r2, r3);
			}

			var inArr = ImmutableArray.Create(typeof(TI1));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TO1, TO2, TO3, TO4>(string name, Func<TI1, (TO1, TO2, TO3, TO4)> execute)		    
		    where TI1 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4) = execute((TI1) args[0]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4);
			}

			var inArr = ImmutableArray.Create(typeof(TI1));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TO1, TO2, TO3, TO4, TO5>(string name, Func<TI1, (TO1, TO2, TO3, TO4, TO5)> execute)		    
		    where TI1 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5) = execute((TI1) args[0]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5);
			}

			var inArr = ImmutableArray.Create(typeof(TI1));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TO1, TO2, TO3, TO4, TO5, TO6>(string name, Func<TI1, (TO1, TO2, TO3, TO4, TO5, TO6)> execute)		    
		    where TI1 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6) = execute((TI1) args[0]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6);
			}

			var inArr = ImmutableArray.Create(typeof(TI1));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TO1, TO2, TO3, TO4, TO5, TO6, TO7>(string name, Func<TI1, (TO1, TO2, TO3, TO4, TO5, TO6, TO7)> execute)		    
		    where TI1 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		    where TO7 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6, r7) = execute((TI1) args[0]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6, r7);
			}

			var inArr = ImmutableArray.Create(typeof(TI1));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6), typeof(TO7));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TO1>(string name, Func<TI1, TI2, TO1> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TO1 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                object r1 = execute((TI1) args[0], (TI2) args[1]);
                
                return ImmutableArray.Create<object>(r1);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2));
			var outArr = ImmutableArray.Create(typeof(TO1));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TO1, TO2>(string name, Func<TI1, TI2, (TO1, TO2)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2) = execute((TI1) args[0], (TI2) args[1]);
                
                return ImmutableArray.Create<object>(r1, r2);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TO1, TO2, TO3>(string name, Func<TI1, TI2, (TO1, TO2, TO3)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3) = execute((TI1) args[0], (TI2) args[1]);
                
                return ImmutableArray.Create<object>(r1, r2, r3);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TO1, TO2, TO3, TO4>(string name, Func<TI1, TI2, (TO1, TO2, TO3, TO4)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4) = execute((TI1) args[0], (TI2) args[1]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TO1, TO2, TO3, TO4, TO5>(string name, Func<TI1, TI2, (TO1, TO2, TO3, TO4, TO5)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5) = execute((TI1) args[0], (TI2) args[1]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TO1, TO2, TO3, TO4, TO5, TO6>(string name, Func<TI1, TI2, (TO1, TO2, TO3, TO4, TO5, TO6)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6) = execute((TI1) args[0], (TI2) args[1]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TO1, TO2, TO3, TO4, TO5, TO6, TO7>(string name, Func<TI1, TI2, (TO1, TO2, TO3, TO4, TO5, TO6, TO7)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		    where TO7 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6, r7) = execute((TI1) args[0], (TI2) args[1]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6, r7);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6), typeof(TO7));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TO1>(string name, Func<TI1, TI2, TI3, TO1> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TO1 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                object r1 = execute((TI1) args[0], (TI2) args[1], (TI3) args[2]);
                
                return ImmutableArray.Create<object>(r1);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3));
			var outArr = ImmutableArray.Create(typeof(TO1));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TO1, TO2>(string name, Func<TI1, TI2, TI3, (TO1, TO2)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2]);
                
                return ImmutableArray.Create<object>(r1, r2);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TO1, TO2, TO3>(string name, Func<TI1, TI2, TI3, (TO1, TO2, TO3)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2]);
                
                return ImmutableArray.Create<object>(r1, r2, r3);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TO1, TO2, TO3, TO4>(string name, Func<TI1, TI2, TI3, (TO1, TO2, TO3, TO4)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TO1, TO2, TO3, TO4, TO5>(string name, Func<TI1, TI2, TI3, (TO1, TO2, TO3, TO4, TO5)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TO1, TO2, TO3, TO4, TO5, TO6>(string name, Func<TI1, TI2, TI3, (TO1, TO2, TO3, TO4, TO5, TO6)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TO1, TO2, TO3, TO4, TO5, TO6, TO7>(string name, Func<TI1, TI2, TI3, (TO1, TO2, TO3, TO4, TO5, TO6, TO7)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		    where TO7 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6, r7) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6, r7);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6), typeof(TO7));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TO1>(string name, Func<TI1, TI2, TI3, TI4, TO1> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TO1 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                object r1 = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3]);
                
                return ImmutableArray.Create<object>(r1);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4));
			var outArr = ImmutableArray.Create(typeof(TO1));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TO1, TO2>(string name, Func<TI1, TI2, TI3, TI4, (TO1, TO2)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3]);
                
                return ImmutableArray.Create<object>(r1, r2);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TO1, TO2, TO3>(string name, Func<TI1, TI2, TI3, TI4, (TO1, TO2, TO3)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3]);
                
                return ImmutableArray.Create<object>(r1, r2, r3);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TO1, TO2, TO3, TO4>(string name, Func<TI1, TI2, TI3, TI4, (TO1, TO2, TO3, TO4)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TO1, TO2, TO3, TO4, TO5>(string name, Func<TI1, TI2, TI3, TI4, (TO1, TO2, TO3, TO4, TO5)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TO1, TO2, TO3, TO4, TO5, TO6>(string name, Func<TI1, TI2, TI3, TI4, (TO1, TO2, TO3, TO4, TO5, TO6)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TO1, TO2, TO3, TO4, TO5, TO6, TO7>(string name, Func<TI1, TI2, TI3, TI4, (TO1, TO2, TO3, TO4, TO5, TO6, TO7)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		    where TO7 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6, r7) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6, r7);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6), typeof(TO7));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TO1>(string name, Func<TI1, TI2, TI3, TI4, TI5, TO1> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TO1 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                object r1 = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4]);
                
                return ImmutableArray.Create<object>(r1);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5));
			var outArr = ImmutableArray.Create(typeof(TO1));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TO1, TO2>(string name, Func<TI1, TI2, TI3, TI4, TI5, (TO1, TO2)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4]);
                
                return ImmutableArray.Create<object>(r1, r2);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TO1, TO2, TO3>(string name, Func<TI1, TI2, TI3, TI4, TI5, (TO1, TO2, TO3)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4]);
                
                return ImmutableArray.Create<object>(r1, r2, r3);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TO1, TO2, TO3, TO4>(string name, Func<TI1, TI2, TI3, TI4, TI5, (TO1, TO2, TO3, TO4)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TO1, TO2, TO3, TO4, TO5>(string name, Func<TI1, TI2, TI3, TI4, TI5, (TO1, TO2, TO3, TO4, TO5)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TO1, TO2, TO3, TO4, TO5, TO6>(string name, Func<TI1, TI2, TI3, TI4, TI5, (TO1, TO2, TO3, TO4, TO5, TO6)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TO1, TO2, TO3, TO4, TO5, TO6, TO7>(string name, Func<TI1, TI2, TI3, TI4, TI5, (TO1, TO2, TO3, TO4, TO5, TO6, TO7)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		    where TO7 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6, r7) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6, r7);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6), typeof(TO7));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TO1>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, TO1> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TO1 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                object r1 = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5]);
                
                return ImmutableArray.Create<object>(r1);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6));
			var outArr = ImmutableArray.Create(typeof(TO1));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TO1, TO2>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, (TO1, TO2)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5]);
                
                return ImmutableArray.Create<object>(r1, r2);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TO1, TO2, TO3>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, (TO1, TO2, TO3)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5]);
                
                return ImmutableArray.Create<object>(r1, r2, r3);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TO1, TO2, TO3, TO4>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, (TO1, TO2, TO3, TO4)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TO1, TO2, TO3, TO4, TO5>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, (TO1, TO2, TO3, TO4, TO5)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TO1, TO2, TO3, TO4, TO5, TO6>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, (TO1, TO2, TO3, TO4, TO5, TO6)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TO1, TO2, TO3, TO4, TO5, TO6, TO7>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, (TO1, TO2, TO3, TO4, TO5, TO6, TO7)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		    where TO7 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6, r7) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6, r7);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6), typeof(TO7));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TI7, TO1>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, TI7, TO1> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TI7 : T		    
		    where TO1 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                object r1 = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5], (TI7) args[6]);
                
                return ImmutableArray.Create<object>(r1);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6), typeof(TI7));
			var outArr = ImmutableArray.Create(typeof(TO1));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TI7, TO1, TO2>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, TI7, (TO1, TO2)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TI7 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5], (TI7) args[6]);
                
                return ImmutableArray.Create<object>(r1, r2);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6), typeof(TI7));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TI7, TO1, TO2, TO3>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, TI7, (TO1, TO2, TO3)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TI7 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5], (TI7) args[6]);
                
                return ImmutableArray.Create<object>(r1, r2, r3);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6), typeof(TI7));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TI7, TO1, TO2, TO3, TO4>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, TI7, (TO1, TO2, TO3, TO4)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TI7 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5], (TI7) args[6]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6), typeof(TI7));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TI7, TO1, TO2, TO3, TO4, TO5>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, TI7, (TO1, TO2, TO3, TO4, TO5)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TI7 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5], (TI7) args[6]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6), typeof(TI7));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TI7, TO1, TO2, TO3, TO4, TO5, TO6>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, TI7, (TO1, TO2, TO3, TO4, TO5, TO6)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TI7 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5], (TI7) args[6]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6), typeof(TI7));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		/// <summary>
		/// Create law from function
		/// </summary>
		public static Law<T> Create<TI1, TI2, TI3, TI4, TI5, TI6, TI7, TO1, TO2, TO3, TO4, TO5, TO6, TO7>(string name, Func<TI1, TI2, TI3, TI4, TI5, TI6, TI7, (TO1, TO2, TO3, TO4, TO5, TO6, TO7)> execute)		    
		    where TI1 : T		    
		    where TI2 : T		    
		    where TI3 : T		    
		    where TI4 : T		    
		    where TI5 : T		    
		    where TI6 : T		    
		    where TI7 : T		    
		    where TO1 : T		    
		    where TO2 : T		    
		    where TO3 : T		    
		    where TO4 : T		    
		    where TO5 : T		    
		    where TO6 : T		    
		    where TO7 : T		    
		{
			ImmutableArray<object> Calc(ILogger logger, ImmutableArray<object> args)
            {
                var (r1, r2, r3, r4, r5, r6, r7) = execute((TI1) args[0], (TI2) args[1], (TI3) args[2], (TI4) args[3], (TI5) args[4], (TI6) args[5], (TI7) args[6]);
                
                return ImmutableArray.Create<object>(r1, r2, r3, r4, r5, r6, r7);
			}

			var inArr = ImmutableArray.Create(typeof(TI1), typeof(TI2), typeof(TI3), typeof(TI4), typeof(TI5), typeof(TI6), typeof(TI7));
			var outArr = ImmutableArray.Create(typeof(TO1), typeof(TO2), typeof(TO3), typeof(TO4), typeof(TO5), typeof(TO6), typeof(TO7));

			return new Law<T>(name, inArr, outArr, Calc);
		}

		
	}
}
