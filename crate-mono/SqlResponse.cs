using System;

namespace Crate.Client
{
	public class SqlResponse
	{
		public string[] Cols { get; set; }
		public object[][] Rows { get; set; }
		public int Rowcount { get; set; }

		public SqlResponse ()
		{
		}
	}
}

