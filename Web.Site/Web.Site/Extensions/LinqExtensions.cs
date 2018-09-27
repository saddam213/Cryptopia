using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Site
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

		public static string Truncate(this string value, int maxLength, string truncationIndicator = "...")
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			return value.Length <= maxLength 
			? value 
			: value.Substring(0, maxLength) + truncationIndicator;
		}
	}
}