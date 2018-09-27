using System.Collections.Generic;
using System.Linq;
using Cryptopia.Datatables.Models;
using Cryptopia.Datatables.Reflection;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Transactions;
using System;

namespace Cryptopia.Datatables
{
	/// <summary>
	/// Model binder for datatables.js parameters a la http://geeksprogramando.blogspot.com/2011/02/jquery-datatables-plug-in-with-asp-mvc.html
	/// </summary>
	public class DataTablesParam
	{
		public int iDisplayStart { get; set; }
		public int iDisplayLength { get; set; }
		public int iColumns { get; set; }
		public string sSearch { get; set; }
		public bool bEscapeRegex { get; set; }
		public int iSortingCols { get; set; }
		public int sEcho { get; set; }
		public List<string> sColumnNames { get; set; }
		public List<bool> bSortable { get; set; }
		public List<bool> bSearchable { get; set; }
		public List<string> sSearchValues { get; set; }
		public List<int> iSortCol { get; set; }
		public List<string> sSortDir { get; set; }
		public List<bool> bEscapeRegexColumns { get; set; }

		public DataTablesParam()
		{
			sColumnNames = new List<string>();
			bSortable = new List<bool>();
			bSearchable = new List<bool>();
			sSearchValues = new List<string>();
			iSortCol = new List<int>();
			sSortDir = new List<string>();
			bEscapeRegexColumns = new List<bool>();
		}

		public DataTablesParam(int iColumns)
		{
			this.iColumns = iColumns;
			sColumnNames = new List<string>(iColumns);
			bSortable = new List<bool>(iColumns);
			bSearchable = new List<bool>(iColumns);
			sSearchValues = new List<string>(iColumns);
			iSortCol = new List<int>(iColumns);
			sSortDir = new List<string>(iColumns);
			bEscapeRegexColumns = new List<bool>(iColumns);
		}

		public DataTablesResponseData GetDataTablesResponse<TSource>(IEnumerable<TSource> data, bool disablePaging = false)
		{
			var filters = new DataTablesFiltering();
			var outputProperties = DataTablesTypeInfo<TSource>.Properties;

			TSource[] queryResult;
			var totalDisplayRecords = 0;
			var filteredData = filters.ApplyFiltersAndSort(this, data.AsQueryable(), outputProperties);
			if (!disablePaging)
			{
				totalDisplayRecords = filteredData.Count();
				var skipped = filteredData.Skip(this.iDisplayStart);
				queryResult = (this.iDisplayLength <= 0 ? skipped : skipped.Take(this.iDisplayLength)).ToArray();
			}
			else
			{
				queryResult = filteredData.ToArray();
				totalDisplayRecords = queryResult.Count();
			}

			var result = new DataTablesResponseData()
			{
				iTotalRecords = totalDisplayRecords,
				iTotalDisplayRecords = totalDisplayRecords,
				sEcho = sEcho,
				aaData = queryResult.Cast<object>().ToArray(),
			};

			return result;
		}

		public DataTablesResponseData GetDataTablesResponse<TSource>(IQueryable<TSource> data, bool disablePaging = false)
		{
			var filters = new DataTablesFiltering();
			var outputProperties = DataTablesTypeInfo<TSource>.Properties;

			TSource[] queryResult;
			var totalDisplayRecords = 0;
			var filteredData = filters.ApplyFiltersAndSort(this, data, outputProperties);
			if (!disablePaging)
			{
				totalDisplayRecords = filteredData.Count();
				var skipped = filteredData.Skip(this.iDisplayStart);
				queryResult = (this.iDisplayLength <= 0 ? skipped : skipped.Take(this.iDisplayLength)).ToArray();
			}
			else 
			{
				queryResult = filteredData.ToArray();
				totalDisplayRecords = queryResult.Count();
			}
				
			var result = new DataTablesResponseData()
			{
				iTotalRecords = totalDisplayRecords,
				iTotalDisplayRecords = totalDisplayRecords,
				sEcho = sEcho,
				aaData = queryResult.Cast<object>().ToArray(),
			};

			return result;
		}

		public DataTablesResponseData GetDataTablesResponseNoLock<TSource>(IQueryable<TSource> data, bool disablePaging = false)
		{
			var filters = new DataTablesFiltering();
			var outputProperties = DataTablesTypeInfo<TSource>.Properties;

			TSource[] queryResult;
			var totalDisplayRecords = 0;
			var filteredData = filters.ApplyFiltersAndSort(this, data, outputProperties);
			if (!disablePaging)
			{
				totalDisplayRecords = filteredData.Count();
				var skipped = filteredData.Skip(this.iDisplayStart);
				queryResult = (this.iDisplayLength <= 0 ? skipped : skipped.Take(this.iDisplayLength)).ToArray();
			}
			else
			{
				queryResult = filteredData.ToArray();
				totalDisplayRecords = queryResult.Count();
			}

			var result = new DataTablesResponseData()
			{
				iTotalRecords = totalDisplayRecords,
				iTotalDisplayRecords = totalDisplayRecords,
				sEcho = this.sEcho,
				aaData = queryResult.Cast<object>().ToArray(),
			};

			return result;
		}

		public async Task<DataTablesResponseData> GetDataTablesResponseAsync<TSource>(IQueryable<TSource> data, bool disablePaging = false)
		{
			var filters = new DataTablesFiltering();
			var outputProperties = DataTablesTypeInfo<TSource>.Properties;

			TSource[] queryResult;
			var totalDisplayRecords = 0;
			var filteredData = filters.ApplyFiltersAndSort(this, data, outputProperties);
			if (!disablePaging)
			{
				totalDisplayRecords = await filteredData.CountNoLockAsync().ConfigureAwait(false);
				var skipped = filteredData.Skip(this.iDisplayStart);
				queryResult = await (this.iDisplayLength <= 0 ? skipped : skipped.Take(this.iDisplayLength)).ToArrayAsync();
			}
			else
			{
				queryResult = await filteredData.ToArrayAsync();
				totalDisplayRecords = queryResult.Count();
			}

			var result = new DataTablesResponseData()
			{
				iTotalRecords = totalDisplayRecords,
				iTotalDisplayRecords = totalDisplayRecords,
				sEcho = this.sEcho,
				aaData = queryResult.Cast<object>().ToArray(),
			};

			return result;
		}

		public async Task<DataTablesResponseData> GetDataTablesResponseNoLockAsync<TSource>(IQueryable<TSource> data, bool disablePaging = false)
		{
			var filters = new DataTablesFiltering();
			var outputProperties = DataTablesTypeInfo<TSource>.Properties;

			List<TSource> queryResult;
			var totalDisplayRecords = 0;
			var filteredData = filters.ApplyFiltersAndSort(this, data, outputProperties);
			if (!disablePaging)
			{
				totalDisplayRecords = await filteredData.CountNoLockAsync();
				var skipped = filteredData.Skip(this.iDisplayStart);
				queryResult = await (this.iDisplayLength <= 0 ? skipped : skipped.Take(this.iDisplayLength)).ToListNoLockAsync().ConfigureAwait(false);
			}
			else
			{
				queryResult = await filteredData.ToListNoLockAsync().ConfigureAwait(false);
				totalDisplayRecords = queryResult.Count();
			}

			var result = new DataTablesResponseData()
			{
				iTotalRecords = totalDisplayRecords,
				iTotalDisplayRecords = totalDisplayRecords,
				sEcho = this.sEcho,
				aaData = queryResult.Cast<object>().ToArray(),
			};

			return result;
		}



		public async Task<DataTablesResponseData> GetDataTablesResponseNoLockAsync<TSource>(IQueryable<TSource> data, Action<TSource> postQueryTransform, bool disablePaging = false)
		{
			var filters = new DataTablesFiltering();
			var outputProperties = DataTablesTypeInfo<TSource>.Properties;

			List<TSource> queryResult;
			var totalDisplayRecords = 0;
			var filteredData = filters.ApplyFiltersAndSort(this, data, outputProperties);
			if (!disablePaging)
			{
				totalDisplayRecords = await filteredData.CountNoLockAsync();
				var skipped = filteredData.Skip(this.iDisplayStart);
				queryResult = await (this.iDisplayLength <= 0 ? skipped : skipped.Take(this.iDisplayLength)).ToListNoLockAsync().ConfigureAwait(false);
			}
			else
			{
				queryResult = await filteredData.ToListNoLockAsync().ConfigureAwait(false);
				totalDisplayRecords = queryResult.Count();
			}

			queryResult.ForEach(postQueryTransform);

			var result = new DataTablesResponseData()
			{
				iTotalRecords = totalDisplayRecords,
				iTotalDisplayRecords = totalDisplayRecords,
				sEcho = this.sEcho,
				aaData = queryResult.Cast<object>().ToArray(),
			};

			return result;
		}

	}

	//public enum DataType
	//{
	//    tInt,
	//    tString,
	//    tnone
	//}

	public static class DataContextExtensions
	{
		public static List<T> ToListNoLock<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.ToList();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<List<T>> ToListNoLockAsync<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				List<T> toReturn = await query.ToListAsync();
				scope.Complete();
				return toReturn;
			}
		}

		public static int CountNoLock<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				int toReturn = query.Count();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<int> CountNoLockAsync<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				int toReturn = await query.CountAsync();
				scope.Complete();
				return toReturn;
			}
		}
	}
}