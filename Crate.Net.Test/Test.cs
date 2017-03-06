using System;
using System.Collections.Generic;
using Crate.Net.Client.Models;
using NUnit.Framework;

namespace Crate.Net.Client.Tests
{
	[TestFixture]
	public class Test
	{
		[Test]
		public void TestDefaultConnection()
		{
			var server = new CrateServer();

			Assert.AreEqual("http", server.Scheme);
			Assert.AreEqual("localhost", server.Hostname);
			Assert.AreEqual(4200, server.Port);
		}

		[Test]
		public void TestServerWithKvp()
		{
			var server = new CrateServer(new KeyValuePair<string, int>("localhost", 4200));

			Assert.AreEqual("http", server.Scheme);
			Assert.AreEqual("localhost", server.Hostname);
			Assert.AreEqual(4200, server.Port);
		}

		[Test]
		public void TestGetDateTime()
		{
			var reader = new CrateDataReader(new SqlResponse
			{
				Rows = new[] { new object[] { 1388534400000 } },
				Cols = new[] { "dt" }
			});

			reader.Read();
			var dt = new DateTime(2014, 01, 01);

			Assert.AreEqual(dt, reader.GetDateTime(0));
		}

		[Test]
		public void TestGetInt()
		{
			var reader = new CrateDataReader(new SqlResponse
			{
				Rows = new[] { new object[] { 1L } },
				Cols = new[] { "flag" },
			});

			reader.Read();

			Assert.AreEqual((byte)1, reader.GetByte(0));
			Assert.AreEqual((Int16)1, reader.GetInt16(0));
			Assert.AreEqual((Int32)1, reader.GetInt32(0));
			Assert.AreEqual((Int64)1, reader.GetInt64(0));
		}

		[Test]
		public void TestGetDouble()
		{
			var reader = new CrateDataReader(new SqlResponse
			{
				Rows = new[] { new object[] { 1.2D } },
				Cols = new[] { "flag" },
			});

			reader.Read();

			Assert.AreEqual(1.2D, reader.GetDouble(0));
			Assert.AreEqual((float)1.2, reader.GetFloat(0));
			Assert.AreEqual((decimal)1.2, reader.GetDecimal(0));
		}

		[Test]
		public void TestThatStatementParameterPlaceholderGetParsedCorrectly()
		{
			var stmt = @"select * from schema.test where name = @name and form_id = @form_id";
			var expectedResult = @"select * from schema.test where name = $1 and form_id = $2";

			var parameters = new object[] { "theName", 1 };

			var sqlRequest = new SqlRequest(stmt, parameters);

			Assert.AreEqual(expectedResult, sqlRequest.Stmt);
		}

		[Test]
		public void TestThatNullValuesHandledCorrectly()
		{
			var parameter = new CrateParameter("theName", null);

			Assert.NotNull(parameter);
			Assert.AreEqual(DBNull.Value, parameter.Value);
		}
	}
}

