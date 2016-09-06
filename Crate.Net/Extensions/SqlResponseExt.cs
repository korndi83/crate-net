using System;
using Crate.Net.Client.Models;

namespace Crate.Net.Client.Extensions
{
    public static class SqlResponseExt
    {
        public static SqlResponse ToSqlResponse(this Exception ex)
        {
            var otherErrorResponse = new SqlResponse
            {
                Error = new SqlError
                {
                    Code = -1,
                    Message = ex.Message
                }
            };

            return otherErrorResponse;
        }

        public static SqlResponse ToSqlResponse(this string errorMessage)
        {
            var otherErrorResponse = new SqlResponse
            {
                Error = new SqlError
                {
                    Code = -1,
                    Message = errorMessage
                }
            };

            return otherErrorResponse;
        }
    }
}
