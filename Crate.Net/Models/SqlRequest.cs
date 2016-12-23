using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Crate.Net.Client.Models
{
	[Serializable]
	public class SqlRequest
	{

		private const string PATTERN = @"@\w+";

		private string _stmt;

		[JsonProperty("stmt")]
		public string Stmt
		{
			get
			{
				return _stmt;
			}

			set
			{
				// .Net Programmers are used to @ markers for replacements of parameters
				// the HTTP API endpoint expects $ though
				var tmpStmt = value;

				var regex = new Regex(PATTERN);

				int paramCounter = 1;
				tmpStmt = regex.Replace(tmpStmt, m => "$" + paramCounter++);

				_stmt = tmpStmt;
			}
		}

		[JsonProperty("args")]
		public object[] Args { get; set; }

		public SqlRequest() { }

		public SqlRequest(string statement, params object[] args)
		{
			Stmt = statement;
			Args = args;
		}
	}
}
