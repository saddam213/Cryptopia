using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.Base
{
	public static class LinqExtensions
	{
		/// <summary>
		/// Returns the maximal element of the given sequence, based on
		/// the given projection.
		/// </summary>
		/// <remarks>
		/// If more than one element has the maximal projected value, the first
		/// one encountered will be returned. This overload uses the default comparer
		/// for the projected type. This operator uses immediate execution, but
		/// only buffers a single result (the current maximal element).
		/// </remarks>
		/// <typeparam name="TSource">Type of the source sequence</typeparam>
		/// <typeparam name="TKey">Type of the projected element</typeparam>
		/// <param name="source">Source sequence</param>
		/// <param name="selector">Selector to use to pick the results to compare</param>
		/// <returns>The maximal element, according to the projection.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
		/// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
		{
			return source.MaxBy(selector, Comparer<TKey>.Default);
		}

		/// <summary>
		/// Returns the maximal element of the given sequence, based on
		/// the given projection and the specified comparer for projected values. 
		/// </summary>
		/// <remarks>
		/// If  than one element has the maximal projected value, the first
		/// one encountered will be returned. This overload uses the default comparer
		/// for the projected type. This operator uses immediate execution, but
		/// only buffers a single result (the current maximal element).
		/// </remarks>
		/// <typeparam name="TSource">Type of the source sequence</typeparam>
		/// <typeparam name="TKey">Type of the projected element</typeparam>
		/// <param name="source">Source sequence</param>
		/// <param name="selector">Selector to use to pick the results to compare</param>
		/// <param name="comparer">Comparer to use to compare projected values</param>
		/// <returns>The maximal element, according to the projection.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
		/// or <paramref name="comparer"/> is null</exception>
		/// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (selector == null) throw new ArgumentNullException("selector");
			if (comparer == null) throw new ArgumentNullException("comparer");
			using (var sourceIterator = source.GetEnumerator())
			{

				if (!sourceIterator.MoveNext())
				{
					throw new InvalidOperationException("Sequence contains no elements");
				}
				var max = sourceIterator.Current;
				var maxKey = selector(max);
				while (sourceIterator.MoveNext())
				{
					var candidate = sourceIterator.Current;
					var candidateProjected = selector(candidate);
					if (comparer.Compare(candidateProjected, maxKey) > 0)
					{
						max = candidate;
						maxKey = candidateProjected;
					}
				}
				return max;
			}
		}

		/// <summary>
		/// Batches the source sequence into sized buckets.
		/// </summary>
		/// <typeparam name="TSource">Type of elements in <paramref name="source"/> sequence.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="size">Size of buckets.</param>
		/// <returns>A sequence of equally sized buckets containing elements of the source collection.</returns>
		/// <remarks>
		/// This operator uses deferred execution and streams its results (buckets and bucket content). 
		/// It is also identical to <see cref="Partition{TSource}(System.Collections.Generic.IEnumerable{TSource},int)"/>.
		/// </remarks>
		public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size)
		{
			return Batch(source, size, x => x);
		}

		/// <summary>
		/// Batches the source sequence into sized buckets and applies a projection to each bucket.
		/// </summary>
		/// <typeparam name="TSource">Type of elements in <paramref name="source"/> sequence.</typeparam>
		/// <typeparam name="TResult">Type of result returned by <paramref name="resultSelector"/>.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="size">Size of buckets.</param>
		/// <param name="resultSelector">The projection to apply to each bucket.</param>
		/// <returns>A sequence of projections on equally sized buckets containing elements of the source collection.</returns>
		/// <remarks>
		/// This operator uses deferred execution and streams its results (buckets and bucket content).
		/// It is also identical to <see cref="Partition{TSource}(System.Collections.Generic.IEnumerable{TSource},int)"/>.
		/// </remarks>
		public static IEnumerable<TResult> Batch<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<IEnumerable<TSource>, TResult> resultSelector)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (size <= 0) throw new ArgumentOutOfRangeException("size");
			if (resultSelector == null) throw new ArgumentNullException("resultSelector");
			return BatchImpl(source, size, resultSelector);
		}

		private static IEnumerable<TResult> BatchImpl<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<IEnumerable<TSource>, TResult> resultSelector)
		{
			//Debug.Assert(source != null);
			//Debug.Assert(size > 0);
			//Debug.Assert(resultSelector != null);

			TSource[] bucket = null;
			var count = 0;
			foreach (var item in source)
			{
				if (bucket == null)
				{
					bucket = new TSource[size];
				}

				bucket[count++] = item;

				// The bucket is fully buffered before it's yielded
				if (count != size)
				{
					continue;
				}

				// Select is necessary so bucket contents are streamed too
				yield return resultSelector(bucket.Select(x => x));
				bucket = null;
				count = 0;
			}

			// Return the last bucket with all remaining elements
			if (bucket != null && count > 0)
			{
				yield return resultSelector(bucket.Take(count));
			}
		}


		public static bool IsNullOrEmpty<TResult>(this IEnumerable<TResult> items)
		{
			if (items == null)
			{
				return true;
			}
			return !items.Any();
		}

		/// <summary>
		/// Returns all distinct elements of the given source, where "distinctness"
		/// is determined via a projection and the default equality comparer for the projected type.
		/// </summary>
		/// <remarks>
		/// This operator uses deferred execution and streams the results, although
		/// a set of already-seen keys is retained. If a key is seen multiple times,
		/// only the first element with that key is returned.
		/// </remarks>
		/// <typeparam name="TSource">Type of the source sequence</typeparam>
		/// <typeparam name="TKey">Type of the projected element</typeparam>
		/// <param name="source">Source sequence</param>
		/// <param name="keySelector">Projection for determining "distinctness"</param>
		/// <returns>A sequence consisting of distinct elements from the source sequence,
		/// comparing them by the specified key projection.</returns>

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector)
		{
			return source.DistinctBy(keySelector, null);
		}

		/// <summary>
		/// Returns all distinct elements of the given source, where "distinctness"
		/// is determined via a projection and the specified comparer for the projected type.
		/// </summary>
		/// <remarks>
		/// This operator uses deferred execution and streams the results, although
		/// a set of already-seen keys is retained. If a key is seen multiple times,
		/// only the first element with that key is returned.
		/// </remarks>
		/// <typeparam name="TSource">Type of the source sequence</typeparam>
		/// <typeparam name="TKey">Type of the projected element</typeparam>
		/// <param name="source">Source sequence</param>
		/// <param name="keySelector">Projection for determining "distinctness"</param>
		/// <param name="comparer">The equality comparer to use to determine whether or not keys are equal.
		/// If null, the default equality comparer for <c>TSource</c> is used.</param>
		/// <returns>A sequence consisting of distinct elements from the source sequence,
		/// comparing them by the specified key projection.</returns>

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (keySelector == null) throw new ArgumentNullException("keySelector");
			return DistinctByImpl(source, keySelector, comparer);
		}

		private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
#if !NO_HASHSET
			var knownKeys = new HashSet<TKey>(comparer);
			foreach (var element in source)
			{
				if (knownKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}
#else
            //
            // On platforms where LINQ is available but no HashSet<T>
            // (like on Silverlight), implement this operator using 
            // existing LINQ operators. Using GroupBy is slightly less
            // efficient since it has do all the grouping work before
            // it can start to yield any one element from the source.
            //

            return source.GroupBy(keySelector, comparer).Select(g => g.First());
#endif
		}

		public static Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
		{
			return Task.WhenAll(enumerable.Select(item => action(item)));
		}
	}
}
