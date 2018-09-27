using System.Threading.Tasks;

namespace Cryptopia.Base.Queueing
{
	/// <summary>
	/// Class to encapsulate the item information to be proccesed and its completion task
	/// </summary>
	/// <typeparam name="T">The itme containing the information to be used in the Process function</typeparam>
	/// <typeparam name="U">Th return type of the Process function</typeparam>
	public class ProcessorQueueItem<T, U>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessorQueueItem"/> class.
		/// </summary>
		/// <param name="item">The request.</param>
		public ProcessorQueueItem(T item)
		{
			Item = item;
			CompletionSource = new TaskCompletionSource<U>();
		}

		/// <summary>
		/// Gets or sets the object to add to the process call containing the information needed to the Process function.
		/// </summary>
		public T Item { get; set; }

		/// <summary>
		/// Gets or sets the completion source to return the result from the Process function.
		/// </summary>
		public TaskCompletionSource<U> CompletionSource { get; set; }
	}
}
