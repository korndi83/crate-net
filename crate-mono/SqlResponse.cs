using Newtonsoft.Json;
using System;

namespace Crate.Client
{
    [Serializable]
	public class SqlResponse
	{
        [JsonProperty("cols")]
		public string[] Cols { get; set; }

        [JsonProperty("rows")]
        public object[][] Rows { get; set; }

        [JsonProperty("rowcount")]
        public int RowCount { get; set; }

        [JsonProperty("error")]
        public SqlError Error { get; set; }

        public SqlResponse ()
		{
		}
	}
}

