using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Crate.Net.Client.Extensions;
using Crate.Net.Client.Models;
using Newtonsoft.Json;

namespace Crate.Net.Client
{

	public static class SqlClient
	{
		// as discussed here
		// https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
		private static readonly HttpClient _client = new HttpClient();

		public static async Task<SqlResponse> ExecuteAsync(string sqlUri, SqlRequest request, CancellationToken cancellationToken)
		{
			var data = JsonConvert.SerializeObject(request);

			try
			{
				var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");

				var resp = await _client.PostAsync(sqlUri, content, cancellationToken);
				var responseContent = await resp.Content.ReadAsStringAsync();

				return JsonConvert.DeserializeObject<SqlResponse>(responseContent);
			}
			catch(WebException ex)
			{
				if(ex.Status != WebExceptionStatus.ProtocolError)
					return ex.ToSqlResponse();

				var response = ex.Response as HttpWebResponse;

				if(response == null)
					return ex.ToSqlResponse();

				using(var reader = new System.IO.StreamReader(response.GetResponseStream()))
				{
					var responseData = await reader.ReadToEndAsync();

					var errorResponse = JsonConvert.DeserializeObject<SqlResponse>(responseData);

					return errorResponse;
				}
			}
			catch(Exception ex)
			{
				return ex.ToSqlResponse();
			}
		}
	}
}
