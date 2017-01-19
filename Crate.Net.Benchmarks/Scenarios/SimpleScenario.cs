using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using Crate.Net.Client;
using Crate.Net.Testing;

namespace Crate.Net.Benchmarks.Scenarios
{
	[ClrJob]
	[LegacyJitX64Job, RyuJitX64Job]
	public class SimpleScenario
	{
		[Benchmark]
		public async Task<string> QueryClusterName()
		{
			var stmt = @"select name from sys.cluster";

			using(var conn = new CrateConnection("Server=vsdocker01;Port=4200"))
			{
				await conn.OpenAsync();

				string result;
				using(var command = conn.CreateCommand())
				{
					command.CommandText = stmt;

					result = (string)await command.ExecuteScalarAsync();
				}

				conn.Close();

				return result;
			}
		}
	}
}
