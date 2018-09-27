using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.Linq;
using Cryptopia.API.DataAccess;

namespace Cryptopia.Common.DataAccess
{
	/// <summary>
	/// MS SQL data access layer
	/// </summary>
	public class MsSqlDataAccess : IDataAccess
    {
        /// <summary>
        /// The connection string
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// The Linq 2 Sql data context
        /// </summary>
        private DataContext _dataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlDataAccess"/> class.
        /// </summary>
        public MsSqlDataAccess()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            _dataContext = new DataContext(_connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlDataAccess"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MsSqlDataAccess(string connectionString)
        {
            _connectionString = connectionString;
            _dataContext = new DataContext(_connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlDataAccess"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public MsSqlDataAccess(Connection connection)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connection.ToString()].ConnectionString;
            _dataContext = new DataContext(_connectionString);
        }

        /// <summary>
        /// Gets the data context for Linq 2 SQL support.
        /// </summary>
        public DataContext DataContext
        {
            get { return _dataContext; }
        }

        /// <summary>
        /// Executes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType"></param>
        /// <returns>
        /// the rows affected
        /// </returns>
        public int Execute(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = null)
        {
            using (var connection = new SqlConnection(_connectionString))
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
        public async Task<int> ExecuteAsync(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = null)
        {
            return await Task.Factory.StartNew<int>( () => Execute(sql, parameters, commandType, timeout));
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters mapped by property name.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>
        /// the results of the query as a dynamic object
        /// </returns>
        public IEnumerable<dynamic> ExecuteQuery(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query(sql, parameters, commandType: commandType, commandTimeout: timeout);
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters mapped by property name.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>
        /// the results of the query as type T
        /// </returns>
        public IEnumerable<T> ExecuteQuery<T>(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<T>(sql, parameters, commandType: commandType, commandTimeout: timeout);
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters mapped by property name.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>
        /// the results of the query as type T
        /// </returns>
        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object parameters, CommandType commandType = CommandType.Text, int? timeout = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<T>(sql, parameters, commandType: commandType, commandTimeout: timeout);
            }
        }

        public IPagedQueryResponse<T> ExecutePagedQuery<T>(string sql, object parameters, PagedQueryRequest pageData, int? timeout = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Did you know you have to call Open expicitly if you use a Dapper multi query
                // cause I fucken didn't and spent 9 fucking hours trying to get this to work
                connection.Open();

                var sqlparameters = new DynamicParameters(parameters);
                sqlparameters.AddDynamicParams(pageData);
                int totalCount = 0;

                // the dataset will trail with TotalCount so we can split on that usining a multiquery and set a varuable and only collect T
                var data = connection.Query<T, int, T>(sql, (a, b) => { totalCount = b; return a; }, sqlparameters, commandType: CommandType.StoredProcedure, splitOn: "TotalCount");
                if (data.Any())
                {
                    return new PagedQueryResponse<T> { ItemsPerPage = pageData.ItemsPerPage, TotalItems = totalCount, PageResults = data };
                }

                return new PagedQueryResponse<T> { ItemsPerPage = pageData.ItemsPerPage, TotalItems = 0, PageResults = new List<T>() };
            }
        }

        public async Task<IPagedQueryResponse<T>> ExecutePagedQueryAsync<T>(string sql, object parameters, PagedQueryRequest pageData, int? timeout = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Did you know you have to call Open expicitly if you use a Dapper multi query
                // cause I fucken didn't and spent 9 fucking hours trying to get this to work
                await connection.OpenAsync();

                var sqlparameters = new DynamicParameters(parameters);
                sqlparameters.AddDynamicParams(pageData);
                int totalCount = 0;

                // the dataset will trail with TotalCount so we can split on that usining a multiquery and set a varuable and only collect T
                var data = await connection.QueryAsync<T, int, T>(sql, (a, b) => { totalCount = b; return a; }, sqlparameters, commandType: CommandType.StoredProcedure, splitOn: "TotalCount");
                if (data.Any())
                {
                    return new PagedQueryResponse<T> { ItemsPerPage = pageData.ItemsPerPage, TotalItems = totalCount, PageResults = data };
                }

                return new PagedQueryResponse<T> { ItemsPerPage = pageData.ItemsPerPage, TotalItems = 0, PageResults = new List<T>() };
            }
        }


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (_dataContext != null)
            {
                _dataContext.Dispose();
            }
        }



    }
}
