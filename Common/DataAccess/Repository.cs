using System;
using System.Configuration;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.API.DataAccess
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class Repository<T> : IDisposable, IRepository<T>, IRepositoryAsync<T> where T : class
	{
		#region Fields

		private readonly DataContext _dataContext;

		#endregion

		#region Constructor

		public Repository()
		{
			string defaultConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
			_dataContext = new DataContext(defaultConnectionString);
		}

		public Repository(Connection connection)
		{
			string defaultConnectionString = ConfigurationManager.ConnectionStrings[connection.ToString()].ConnectionString;
			_dataContext = new DataContext(defaultConnectionString);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of the primary key.
		/// </summary>
		private string PrimaryKeyName
		{
			get { return TableMetadata.RowType.IdentityMembers[0].Name; }
		}

		/// <summary>
		/// Gets the get table.
		/// </summary>
		private Table<T> GetTable
		{
			get { return _dataContext.GetTable<T>(); }
		}

		//private Table<T> GetTable
		//{
		//    get { return _dataContext.GetTable<T>(); }
		//}

		/// <summary>
		/// Gets the table metadata.
		/// </summary>
		private MetaTable TableMetadata
		{
			get { return _dataContext.Mapping.GetTable(typeof(T)); }
		}

		/// <summary>
		/// Gets the class metadata.
		/// </summary>
		private MetaType ClassMetadata
		{
			get { return _dataContext.Mapping.GetMetaType(typeof(T)); }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Create a new instance of type T.
		/// </summary>
		/// <returns></returns>
		public T CreateInstance()
		{
			var entity = Activator.CreateInstance<T>();
			GetTable.InsertOnSubmit(entity);
			return entity;
		}

		/// <summary>
		/// Create a new instance of type T async.
		/// </summary>
		public async Task<T> CreateInstanceAsync()
		{
			return await Task.Run<T>(() => CreateInstance());
		}

		/// <summary>
		/// Return all instances of type T.
		/// </summary>
		public IQueryable<T> GetAll()
		{
			return GetTable;
		}

		/// <summary>
		/// Return all instances of type T async.
		/// </summary>
		public async Task<IQueryable<T>> GetAllAsync()
		{
			return await Task.Run<IQueryable<T>>(() => GetAll());
		}

		/// <summary>
		/// Return all instances of type T that match the expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		public IQueryable<T> GetAll(Func<T, bool> expression)
		{
			return GetTable.Where(expression).AsQueryable();
		}

		/// <summary>
		/// Return all instances of type T that match the expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		public async Task<IQueryable<T>> GetAllAsync(Func<T, bool> expression)
		{
			return await Task.Run<IQueryable<T>>(() => GetAll(expression));
		}

		/// <summary>
		/// Gets a single item based on the specified exp.
		/// </summary>
		/// <param name="expression">The expression.</param>
		public T Get(Func<T, bool> exp)
		{
			return GetTable.First(exp);
		}

		/// <summary>
		/// Gets a single item based on the specified exp.
		/// </summary>
		/// <param name="expression">The expression.</param>
		public async Task<T> GetAsync(Func<T, bool> expression)
		{
			return await Task.Run<T>(() => Get(expression));
		}

		/// <summary>
		/// Gets a single item based on the specified exp.
		/// </summary>
		/// <param name="exp">The exp.</param>
		/// <returns></returns>
		public T GetOrDefault(Func<T, bool> exp)
		{
			return GetTable.FirstOrDefault(exp);
		}

		/// <summary>
		/// Gets a single item based on the specified exp.
		/// </summary>
		/// <param name="expression">The expression.</param>
		public async Task<T> GetOrDefaultAsync(Func<T, bool> expression)
		{
			return await Task.Run<T>(() => GetOrDefault(expression));
		}

		///// <summary>
		///// Gets the object by its primary key id.
		///// </summary>
		///// <typeparam name="U">The Id type</typeparam>
		///// <param name="id">The id value.</param>
		///// <returns>The object with that id, otherwise default(T)</returns>
		//public T GetById<U>(U id) 
		//{
		//    return _dataContext.Connection.Query<T>(string.Format("select top 1 * from {0} where Id = @Id", TableMetadata.TableName), new { Id = id }, commandType: CommandType.Text).FirstOrDefault();
		//}

		///// <summary>
		///// Executes the stored procedure.
		///// </summary>
		///// <typeparam name="U"></typeparam>
		///// <param name="storedProc">The stored proc.</param>
		///// <param name="parameters">The parameters.</param>
		///// <returns></returns>
		//public IEnumerable<U> ExecuteStoredProcedure<U>(string storedProc, object parameters = null)
		//{
		//    return _dataContext.Connection.Query<U>(storedProc, parameters, commandType: CommandType.StoredProcedure);
		//}

		/// <summary>
		/// Deletes the specified obj.
		/// </summary>
		/// <param name="obj">The obj.</param>
		public void Delete(T obj)
		{
			_dataContext.GetTable<T>().DeleteOnSubmit(obj);
		}

		/// <summary>
		/// Deletes the specified obj async.
		/// </summary>
		/// <param name="obj">The obj.</param>
		public async Task DeleteAsync(T obj)
		{
			await Task.Run(() => Delete(obj));
		}

		/// <summary>
		/// Inserts the specified obj.
		/// </summary>
		/// <param name="obj">The obj.</param>
		public void Insert(T obj)
		{
			GetTable.InsertOnSubmit(obj);
		}

		/// <summary>
		/// Inserts the specified obj async.
		/// </summary>
		/// <param name="obj">The obj.</param>
		public async Task InsertAsync(T obj)
		{
			await Task.Run(() => Insert(obj));
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public void Save(bool mergeChanges = false)
		{
			if (mergeChanges)
			{
				_dataContext.Refresh(RefreshMode.KeepChanges, GetTable.ToArray());
			}
			_dataContext.SubmitChanges();
		}

		/// <summary>
		/// Saves this instance async.
		/// </summary>
		public async Task SaveAsync(bool mergeChanges = false)
		{
			await Task.Run(() => Save(mergeChanges));
		}

		/// <summary>
		/// Saves this instance async.
		/// </summary>
		public async Task SaveAsync()
		{
			await Task.Run(() => Save());
		}

		public void SaveOrMerge()
		{
			try
			{
				Save();
				return;
			}
			catch { }
			Save(true);
		}

		public async Task SaveOrMergeAsync()
		{
			try
			{
				await SaveAsync();
				return;
			}
			catch { }
			await SaveAsync(true);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (_dataContext != null)
			{
				_dataContext.Dispose();
			}
		}

		#endregion


		
	}
}
