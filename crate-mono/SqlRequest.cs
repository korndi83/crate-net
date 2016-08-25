using System;
using Newtonsoft.Json;

namespace Crate.Client
{
    [Serializable]
	public class SqlRequest
	{
        [JsonProperty("stmt")]
		public string Stmt { get; set; }

        [JsonProperty("args")]
        public object[] Args { get; set; }

		public SqlRequest () {}

		public SqlRequest(string statement, params object[] args) {
			Stmt = statement;
			Args = args;
		}
	}
}
