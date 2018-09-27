using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.API.DataAccess
{
	public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Create a new instance of type T.
        /// </summary>
        /// <returns></returns>
        T CreateInstance();

        /// <summary>
        /// Return all instances of type T.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Return all instances of type T that match the expression exp.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        IQueryable<T> GetAll(Func<T, bool> exp);

        /// <summary>
        /// Return a single instance of type T that match the expression exp.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        T Get(Func<T, bool> exp);

          /// <summary>
        /// Return a single instance of type T that match the expression exp, or default if not exists.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        T GetOrDefault(Func<T, bool> exp);

        /// <summary>
        /// Deletes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void Delete(T obj);

        /// <summary>
        /// Inserts the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void Insert(T obj);

        /// <summary>
        /// Saves this instance.
        /// </summary>
        void Save(bool mergeChanges = false);

		/// <summary>
		/// Saves the data or merges if the row has changed.
		/// </summary>
		void SaveOrMerge();
    }


    public interface IRepositoryAsync<T> where T : class
    {
        /// <summary>
        /// Create a new instance of type T.
        /// </summary>
        /// <returns></returns>
        Task<T> CreateInstanceAsync();

        /// <summary>
        /// Return all instances of type T.
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<T>> GetAllAsync();

        /// <summary>
        /// Return all instances of type T that match the expression exp.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<IQueryable<T>> GetAllAsync(Func<T, bool> exp);

        /// <summary>
        /// Return a single instance of type T that match the expression exp.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<T> GetAsync(Func<T, bool> exp);

        /// <summary>
        /// Return a single instance of type T that match the expression exp, or default if not exists.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<T> GetOrDefaultAsync(Func<T, bool> exp);

        /// <summary>
        /// Deletes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        Task DeleteAsync(T obj);

        /// <summary>
        /// Inserts the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        Task InsertAsync(T obj);

        /// <summary>
        /// Saves this instance.
        /// </summary>
        Task SaveAsync(bool mergeChanges = false);

		/// <summary>
		/// Saves the data or merges changes if the row has changed.
		/// </summary>
		Task SaveOrMergeAsync();
    }
}
