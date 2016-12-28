using System;
using System.Collections.Generic;
using Crate.Net.Client.Helper;
using Newtonsoft.Json;

namespace Crate.Net.Client.Models
{
	[Serializable]
	public class SqlResponse
	{
		[JsonProperty("cols")]
		public string[] Cols { get; set; }

		[JsonProperty("col_types")]
		[JsonConverter(typeof(SqlColumnTypesConverter))]
		public List<SqlColumnType> ColumnTypes { get; set; }

		[JsonProperty("duration")]
		public double Duration { get; set; }

		[JsonProperty("rows")]
		public object[][] Rows { get; set; }

		[JsonProperty("rowcount")]
		public int RowCount { get; set; }

		[JsonProperty("error")]
		public SqlError Error { get; set; }

		public SqlResponse()
		{
		}
	}
}

