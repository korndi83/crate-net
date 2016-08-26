using Dapper;
using System.Linq;
using Crate.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Crate.Client
{
    [TestClass]
	public class IntegrationTest
	{
        //private CrateCluster _cluster = null;

        //[TestInitialize]
        //public void SetUpCrateCluster()
        //{
        //    if (_cluster != null)
        //        return;

        //    _cluster = new CrateCluster("crate-testing", "0.55.4");
        //    _cluster.Start();
        //}

        //[TestCleanup]
        //public void TearDownCrateCluster()
        //{
        //    if (_cluster == null)
        //        return;

        //    _cluster.Stop();
        //    _cluster = null;
        //}

		[TestMethod]
		public void TestSelect ()
		{
			using (var conn = new CrateConnection()) {
				conn.Open();

				using (var cmd = new CrateCommand("select name from sys.cluster", conn)) {
					var reader = cmd.ExecuteReader();
					reader.Read();
					var clusterName = reader.GetString(0);
					Assert.AreEqual(clusterName, "crate");
				}
			}
		}

        [TestMethod]
        public void TestSelectThrowsCrateException()
        {
            try
            {
                using (var conn = new CrateConnection())
                {
                    conn.Open();

                    using (var cmd = new CrateCommand("invalid sql statement", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                            reader.Read();
                    }
                }
            }
            catch (CrateException)
            {
                // ignore
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
		public void TestSelectServerRoundrobin()
		{
			using (var conn = new CrateConnection("Server=localhost,localhost;Port=9999,4200")) {
				conn.Open();

				for (var i = 0; i < 10; i++) {
					var clusterName = conn.Query<string>("select name from sys.cluster").First();
					Assert.AreEqual("crate", clusterName);
				}
			}
		}

		[TestMethod]
		public void TestWithDapper()
		{
			using (var conn = new CrateConnection()) {
				conn.Open();
				var clusterName = conn.Query<string>("select name from sys.cluster").First();
				Assert.AreEqual(clusterName, "crate");

				clusterName = conn.Query<string>(
					"select name from sys.cluster where name = ?", new { Name = "crate" }).First();
				Assert.AreEqual(clusterName, "crate");

				conn.Execute(
					"create table foo (id int primary key, name string) with (number_of_replicas='0-1')");
				Assert.AreEqual(1,
					conn.Execute("insert into foo (id, name) values (?, ?)", new { id = 1, name = "foo"}));

				var rowsAffected = conn.Execute(
					"insert into foo (id, name) values (?, ?), (?, ?)",
					new { id1 = 2, name1 = "zwei", id2 = 3, name2 = "drei"}
				);
				Assert.AreEqual(2, rowsAffected);
				conn.Execute("drop table foo");
			}
		}
	}
}
