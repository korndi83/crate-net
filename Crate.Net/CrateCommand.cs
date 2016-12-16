using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Crate.Net.Client.Extensions;
using Crate.Net.Client.Models;

namespace Crate.Net.Client
{
	public class CrateCommand : DbCommand
	{
		private readonly CrateConnection _connection;
		private readonly CrateParameterCollection _parameters = new CrateParameterCollection();

		public override string CommandText { get; set; }
		public override int CommandTimeout { get; set; }

		public CrateCommand(string commandText, CrateConnection connection)
		{
			CommandText = commandText;
			_connection = connection;
		}

		public override void Cancel()
		{
			throw new NotImplementedException();
		}

		public override int ExecuteNonQuery()
		{
			try
			{
				var task = ExecuteNonQueryAsync();
				task.Wait();
				return task.Result;
			}
			catch(AggregateException aggrEx)
			{
				aggrEx = aggrEx.Flatten();

				throw aggrEx.InnerExceptions.First();
			}
		}

		protected async Task<SqlResponse> ExecuteAsync(int currentRetry = 0)
		{
			var server = _connection.NextServer();
			try
			{
				return await SqlClient.ExecuteAsync(
						server.SqlUri(),
						new SqlRequest(CommandText, _parameters.Select(x => x.Value).ToArray()));
			}
			catch(WebException)
			{
				_connection.MarkAsFailed(server);
				if(currentRetry > 3)
				{
					return "Connection retry count exceeded".ToSqlResponse();
				}

				return await ExecuteAsync(currentRetry++);
			}
		}

		public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
		{
			return CrateExecuteNonQueryAsync();
		}

		private async Task<int> CrateExecuteNonQueryAsync()
		{
			var resp = await ExecuteAsync();

			if(resp.Error == null)
				return resp.RowCount;

			throw new CrateException(resp.Error);
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return CrateExecuteReader();
		}

		protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
		{
			return CrateExecuteReaderAsync();
		}

		public DbDataReader CrateExecuteReader()
		{
			var task = CrateExecuteReaderAsync();

			try
			{
				task.Wait();
				return task.Result;
			}
			catch(AggregateException aggrEx)
			{
				aggrEx = aggrEx.Flatten();

				throw aggrEx.InnerExceptions.First();
			}
		}

		public async Task<DbDataReader> CrateExecuteReaderAsync()
		{
			var resp = await ExecuteAsync();

			if(resp.Error == null)
				return new CrateDataReader(resp);

			throw new CrateException(resp.Error);
		}

		public override object ExecuteScalar()
		{
			using(var reader = ExecuteReader())
			{
				reader.Read();
				return reader[0];
			}
		}

		public override void Prepare()
		{
		}

		protected override DbParameter CreateDbParameter()
		{
			return new CrateParameter();
		}

		public override CommandType CommandType { get; set; }

		protected override DbTransaction DbTransaction
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override UpdateRowSource UpdatedRowSource
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return _connection;
			}

			set
			{
				throw new NotSupportedException();
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				return _parameters;
			}
		}

		public override bool DesignTimeVisible
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}
	}
}