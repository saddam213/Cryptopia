using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace Cryptopia.Common.DataAccess
{
	/// <summary>
	/// MySql data simple access layer
	/// </summary>
	public class MySqlDataAccess : IDataAccess
    {
        private string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDataAccess"/> class.
        /// </summary>
        public MySqlDataAccess()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDataAccess"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MySqlDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Executes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        public int Execute(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = 120)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Execute(sql, parameters, commandType: commandType, commandTimeout: timeout);
            }
        }

        /// <summary>
        /// Executes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType"></param>
        /// <param name="timeout"></param>
        /// <returns>
        /// the rows affected
        /// </returns>
        public Task<int> ExecuteAsync(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = 120)
        {
            return Task.Factory.StartNew<int>(() => Execute(sql, parameters, commandType, timeout));
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        public IEnumerable<dynamic> ExecuteQuery(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = 120)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query(sql, parameters, commandType: commandType, commandTimeout: timeout);
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteQuery<T>(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = 120)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<T>(sql, parameters, commandType: commandType, commandTimeout: timeout);
            }
        }

        /// <summary>
        /// Executes the query asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = 120)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryAsync<T>(sql, parameters, commandType: commandType, commandTimeout: timeout);
            }
        }


        /// <summary>
        /// Executes the paged query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="pagedata">the paging parameters</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IPagedQueryResponse<T> ExecutePagedQuery<T>(string sql, object parameters, PagedQueryRequest pagedata, int? timeout = 120)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var sqlparameters = new DynamicParameters(parameters);
                sqlparameters.AddDynamicParams(pagedata);
                int totalCount = 0;

                var data = connection.Query<T, int, T>(sql, (a, b) => { totalCount = b; return a; }, parameters, commandType: CommandType.StoredProcedure, splitOn: "TotalCount");
                if (data.Any())
                {
                    return new PagedQueryResponse<T> { ItemsPerPage = pagedata.ItemsPerPage, TotalItems = totalCount, PageResults = data };
                }

                return new PagedQueryResponse<T> { ItemsPerPage = pagedata.ItemsPerPage, TotalItems = 0, PageResults = new List<T>() };
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
