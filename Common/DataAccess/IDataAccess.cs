using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Cryptopia.Common.DataAccess
{
	public interface IDataAccess : IDisposable
    {
        /// <summary>
        /// Executes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the rows affected</returns>
        int Execute(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = null);

        /// <summary>
        /// Executes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the rows affected</returns>
        Task<int> ExecuteAsync(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = null);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters mapped by property name.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>the results of the query as a dynamic object</returns>
        IEnumerable<dynamic> ExecuteQuery(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = null);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters mapped by property name.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>the results of the query as type T</returns>
        IEnumerable<T> ExecuteQuery<T>(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = null);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters mapped by property name.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>the results of the query as type T</returns>
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = null);


        /// <summary>
        /// Executes the paged query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        IPagedQueryResponse<T> ExecutePagedQuery<T>(string sql, object parameters, PagedQueryRequest pagedata, int? timeout = null);
    }
}
