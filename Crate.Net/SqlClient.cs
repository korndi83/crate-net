using System;
using System.Net;
using System.Threading.Tasks;
using Crate.Net.Client.Extensions;
using Crate.Net.Client.Models;
using Newtonsoft.Json;

namespace Crate.Net.Client {

    public static class SqlClient
    {
        public static async Task<SqlResponse> ExecuteAsync(string sqlUri, SqlRequest request)
        {
            using (var client = new WebClient())
            {
                var data = JsonConvert.SerializeObject(request);

                try
                {
                    var resp = await client.UploadStringTaskAsync(sqlUri, data);

                    return JsonConvert.DeserializeObject<SqlResponse>(resp);
                }
                catch (WebException ex)
                {
                    if (ex.Status != WebExceptionStatus.ProtocolError)
                        return ex.ToSqlResponse();

                    var response = ex.Response as HttpWebResponse;

                    if (response == null)
                        return ex.ToSqlResponse();

                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        var responseData = await reader.ReadToEndAsync();

                        var errorResponse = JsonConvert.DeserializeObject<SqlResponse>(responseData);

                        return errorResponse;
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToSqlResponse();
                }
            }
        }
    }
}
