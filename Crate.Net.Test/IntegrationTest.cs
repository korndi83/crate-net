using System;
using System.Linq;
using System.Threading;
using Crate.Net.Testing;
using Dapper;
using NUnit.Framework;

namespace Crate.Net.Client.Tests
{
	[TestFixture]
	public class IntegrationTest
	{
		private static CrateCluster _cluster = null;

		[OneTimeSetUp]
		public static void SetUpCrateCluster()
		{
			if(_cluster != null)
				return;

			_cluster = new CrateCluster("crate-testing", "1.0.2");
			_cluster.Start();

			// sleep for 10 seconds, wait until cluster starts
			Thread.Sleep(10000);
		}

		[OneTimeTearDown]
		public static void TearDownCrateCluster()
		{
			_cluster?.Stop();
		}

		[Test]
		public void TestSelect()
		{
			using(var conn = new CrateConnection())
			{
				conn.Open();

				using(var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select name from sys.cluster";

					using(var reader = cmd.ExecuteReader())
					{
						reader.Read();
						var clusterName = reader.GetString(0);
						Assert.AreEqual("crate", clusterName);
					}
				}
			}
		}

		[Test]
		public void TestSelectWithParameter()
		{
			using(var conn = new CrateConnection())
			{
				conn.Open();

				using(var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select name from sys.cluster where name = @name";

					var param = cmd.CreateParameter();
					param.ParameterName = "@name";
					param.Value = "crate";

					cmd.Parameters.Add(param);

					using(var reader = cmd.ExecuteReader())
					{
						reader.Read();
						var clusterName = reader.GetString(0);
						Assert.AreEqual("crate", clusterName);
					}
				}
			}
		}

		[Test]
		public void TestSelectThrowsCrateException()
		{
			try
			{
				using(var conn = new CrateConnection())
				{
					conn.Open();

					using(var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "invalid sql statement";

						using(var reader = cmd.ExecuteReader())
							reader.Read();
					}
				}
			}
			catch(CrateException)
			{
				// ignore
			}
			catch(Exception)
			{
				Assert.Fail();
			}
		}

		[Test]
		public void TestSelectServerRoundrobin()
		{
			using(var conn = new CrateConnection("Server=localhost,localhost;Port=9999,4200"))
			{
				conn.Open();

				for(var i = 0; i < 10; i++)
				{
					var clusterName = conn.Query<string>("select name from sys.cluster").First();
					Assert.AreEqual("crate", clusterName);
				}
			}
		}

		[Test]
		public void TestWithDapper()
		{
			using(var conn = new CrateConnection())
			{
				conn.Open();
				var clusterName = conn.Query<string>("select name from sys.cluster").First();
				Assert.AreEqual(clusterName, "crate");

				clusterName = conn.Query<string>(
					"select name from sys.cluster where name = ?", new { Name = "crate" }).First();
				Assert.AreEqual(clusterName, "crate");

				conn.Execute(
					"create table foo (id int primary key, name string) with (number_of_replicas='0-1')");
				Assert.AreEqual(1,
					conn.Execute("insert into foo (id, name) values (?, ?)", new { id = 1, name = "foo" }));

				var rowsAffected = conn.Execute(
					"insert into foo (id, name) values (?, ?), (?, ?)",
					new { id1 = 2, name1 = "zwei", id2 = 3, name2 = "drei" }
				);
				Assert.AreEqual(2, rowsAffected);
				conn.Execute("drop table foo");
			}
		}

		[Test]
		public void TestSchemaTable()
		{
			using(var conn = new CrateConnection())
			{
				conn.Open();

				var query =
					@"SELECT NULL AS ""undefined""
						, TRUE AS ""boolean""
						, CAST(0 AS BYTE) AS ""byte""
						, CAST(0 AS SHORT) AS ""short""
						, CAST(0 AS INTEGER) AS ""integer""
						, CAST(0 AS LONG) AS ""long""
						, CAST(0 AS FLOAT) AS ""float""
						, CAST(0 AS DOUBLE) AS ""double""
						, 'string' AS ""string""
						, CAST('127.0.0.1' AS IP) AS ""ip""
						, CURRENT_TIMESTAMP AS ""timestamp""
						, [1, 2, 3] AS ""array""
						, CAST('POINT (5 5)' AS geo_point) AS ""geo_point""
						, CAST('POLYGON ((5 5, 10 5, 10 10, 5 10, 5 5))' AS geo_shape) AS ""geo_shape""
						, {} AS ""object""
					FROM sys.cluster";

				using(var cmd = conn.CreateCommand())
				{
					cmd.CommandText = query;
					var reader = cmd.ExecuteReader();

					var table = reader.GetSchemaTable();

					Assert.IsNotNull(table);
					Assert.IsTrue(table.Rows.Count == 15);
				}
			}
		}
	}
}
