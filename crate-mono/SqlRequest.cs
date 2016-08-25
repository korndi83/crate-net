using System;

namespace Crate.Client
{
	public class SqlRequest
	{
		public string Stmt { get; set; }
		public object[] Args { get; set; }

		public SqlRequest () {}

		public SqlRequest(string statement, params object[] args) {
			Stmt = statement;
			Args = args;
		}
	}
}
