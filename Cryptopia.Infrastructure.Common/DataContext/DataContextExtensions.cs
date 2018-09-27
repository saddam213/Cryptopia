using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

namespace Cryptopia.Infrastructure.Common.DataContext
{
	public static class DataContextExtensions
	{
		#region ToList NoLock

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

		public static List<T> ToListNoLock<T>(this IOrderedQueryable<T> query)
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

		public static async Task<List<T>> ToListNoLockAsync<T>(this IOrderedQueryable<T> query)
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

		public static List<T> ToListNoLock<T>(this DbRawSqlQuery<T> query)
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

		public static async Task<List<T>> ToListNoLockAsync<T>(this DbRawSqlQuery<T> query)
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

		#endregion

		#region ToDictionary NoLock

		public static async Task<Dictionary<TKey, T>> ToDictionaryNoLockAsync<TKey, T>(this IQueryable<T> query, Func<T, TKey> keySelector)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted },
					TransactionScopeAsyncFlowOption.Enabled))
			{
				Dictionary<TKey, T> toReturn = await query.ToDictionaryAsync(keySelector);
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<Dictionary<TKey, TElement>> ToDictionaryNoLockAsync<T, TKey, TElement>(this IQueryable<T> query, Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted },
					TransactionScopeAsyncFlowOption.Enabled))
			{
				Dictionary<TKey, TElement> toReturn = await query.ToDictionaryAsync(keySelector, elementSelector);
				scope.Complete();
				return toReturn;
			}
		}

		#endregion

		#region Count NoLock

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

		public static int CountNoLock<T>(this IOrderedQueryable<T> query)
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

		public static async Task<int> CountNoLockAsync<T>(this IOrderedQueryable<T> query)
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

		#endregion

		#region First NoLock
		public static async Task<T> FirstNoLockAsync<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted },
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.FirstAsync();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<T> FirstNoLockAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted },
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.FirstAsync(predicate);
				scope.Complete();
				return toReturn;
			}
		}
		#endregion

		#region FirstOrDefault NoLock

		public static T FirstOrDefaultNoLock<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.FirstOrDefault();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<T> FirstOrDefaultNoLockAsync<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.FirstOrDefaultAsync();
				scope.Complete();
				return toReturn;
			}
		}

		public static T FirstOrDefaultNoLock<T>(this IOrderedQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.FirstOrDefault();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<T> FirstOrDefaultNoLockAsync<T>(this IOrderedQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.FirstOrDefaultAsync();
				scope.Complete();
				return toReturn;
			}
		}

		public static T FirstOrDefaultNoLock<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.FirstOrDefault(predicate);
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<T> FirstOrDefaultNoLockAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.FirstOrDefaultAsync(predicate);
				scope.Complete();
				return toReturn;
			}
		}

		public static T FirstOrDefaultNoLock<T>(this IOrderedQueryable<T> query, Expression<Func<T, bool>> predicate)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.FirstOrDefault(predicate);
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<T> FirstOrDefaultNoLockAsync<T>(this IOrderedQueryable<T> query,
			Expression<Func<T, bool>> predicate)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.FirstOrDefaultAsync(predicate);
				scope.Complete();
				return toReturn;
			}
		}

		public static T FirstOrDefaultNoLock<T>(this DbRawSqlQuery<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.FirstOrDefault();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<T> FirstOrDefaultNoLockAsync<T>(this DbRawSqlQuery<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.FirstOrDefaultAsync();
				scope.Complete();
				return toReturn;
			}
		}

		#endregion

		#region Any NoLock

		public static bool AnyNoLock<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.Any(predicate);
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<bool> AnyNoLockAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.AnyAsync(predicate);
				scope.Complete();
				return toReturn;
			}
		}

		public static bool AnyNoLock<T>(this IOrderedQueryable<T> query, Expression<Func<T, bool>> predicate)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.Any(predicate);
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<bool> AnyNoLockAsync<T>(this IOrderedQueryable<T> query, Expression<Func<T, bool>> predicate)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.AnyAsync(predicate);
				scope.Complete();
				return toReturn;
			}
		}

		public static bool AnyNoLock<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.Any();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<bool> AnyNoLockAsync<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.AnyAsync();
				scope.Complete();
				return toReturn;
			}
		}

		public static bool AnyNoLock<T>(this IOrderedQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.Any();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<bool> AnyNoLockAsync<T>(this IOrderedQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.AnyAsync();
				scope.Complete();
				return toReturn;
			}
		}

		#endregion

		#region Max NoLock
		public static async Task<T> MaxNoLockAsync<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted },
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.MaxAsync();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<TResult> MaxNoLockAsync<T, TResult>(this IQueryable<T> query, Expression<Func<T, TResult>> selector)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted },
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.MaxAsync(selector);
				scope.Complete();
				return toReturn;
			}
		}
        #endregion

        #region Bulk Insert

        public static async Task BulkInsertAsync<T>(this Database database, IEnumerable<T> records)
        {
            if (!records.Any())
                return;

            var sqlConn = database.Connection as SqlConnection;
            if (sqlConn == null)
                throw new NotSupportedException("Cannot cast DbConnection as SqlConnection");

            if (sqlConn.State != ConnectionState.Open)
                await sqlConn.OpenAsync();

            string tableName = typeof(T).Name;

            using (var bulkInsert = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.Default, null))
            {
                using (var reader = FastMember.ObjectReader.Create(records))
                {
                    foreach (var propertyInfo in typeof(T).GetProperties())
                    {
                        if (propertyInfo.GetGetMethod().IsVirtual)
                            continue;

                        bulkInsert.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    }

                    bulkInsert.DestinationTableName = $"dbo.{tableName}";
                    bulkInsert.BulkCopyTimeout = 2400;
                    bulkInsert.BatchSize = 5000;

					// if this throws...make sure it blows up proper like..
                    await bulkInsert.WriteToServerAsync(reader);
                }
            }

            if (sqlConn.State != ConnectionState.Closed)
                sqlConn.Close();
        }

        #endregion
    }
}