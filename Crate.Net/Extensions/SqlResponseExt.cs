using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crate.Client.Extensions
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
