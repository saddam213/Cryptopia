using Cryptopia.Common.DataAccess;
using System.Linq;

namespace Cryptopia.Common.Webservice
{
	public static class WebserviceExtensions
    {
        /// <summary>
        /// Creates the error response.
        /// </summary>
        /// <typeparam name="T">the type of response to create</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="error">The error.</param>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static T CreateErrorResponse<T>(this IRequest request, string error, params object[] param) where T : IResponse, new()
        {
            return new T { Error = string.Format(error, param) };
        }

        /// <summary>
        /// Creates the response.
        /// </summary>
        /// <typeparam name="T">the type of response to create</typeparam>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static T CreateResponse<T>(this IRequest request) where T : IResponse, new()
        {
            return new T();
        }

        public static T CreatePagedResponse<T, U>(this IPagedRequest request, IPagedQueryResponse<U> response) where T : IPagedResponse<U>, new()
        {
            return new T
            {
                Page = request.Page,
                TotalCount = response.TotalItems,
                TotalPages = response.TotalItems / request.ItemsPerPage,
                PageResults = response.PageResults.ToList()
            };
        }
    }
}
