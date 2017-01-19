using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Crate.Net.Benchmarks.Scenarios;
using Crate.Net.Client;
using Crate.Net.Testing;

namespace Crate.Net.Benchmarks
{
	public class Program
	{
		private static CrateCluster _cluster;

		public static void Main(string[] args)
		{
			//_cluster = new CrateCluster("crate-testing", "1.0.2");
			//_cluster.Start();

			// sleep for 10 seconds, wait until cluster starts
			//Thread.Sleep(10000);

			var summary = BenchmarkRunner.Run<SimpleScenario>();

			//_cluster.Stop();
		}

		//public static async Task InitSchema()
		//{
		//	using(var conn = new CrateConnection())
		//	{
		//		await conn.OpenAsync();

		//		conn.Close();
		//	}
		//}
	}
}
