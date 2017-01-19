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
				task.ConfigureAwait(false);

				return task.Result;
			}
			catch(AggregateException aggrEx)
			{
				aggrEx = aggrEx.Flatten();

				throw aggrEx.InnerExceptions.First();
			}
		}

		protected async Task<SqlResponse> ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			// set the max numbers of active servers to 1 - looking into performance problems
			var server = _connection.ActiveServers[0];
			try
			{
				return await SqlClient.ExecuteAsync(
						server.SqlUri(),
						new SqlRequest(CommandText, _parameters.Select(x => x.Value).ToArray()),
						cancellationToken);
			}
			catch(WebException)
			{
				// the original retry implementation is too trivial
				// either use a good circuit breaking mechanism or fail fast and hard
				return "Connection could not be established".ToSqlResponse();
			}
		}

		public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
		{
			return CrateExecuteNonQueryAsync(cancellationToken);
		}

		private async Task<int> CrateExecuteNonQueryAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var resp = await ExecuteAsync(cancellationToken);

			if(resp.Error == null)
				return resp.RowCount;

			throw new CrateException(resp.Error);
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return CrateExecuteReader(behavior);
		}

		protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
		{
			return CrateExecuteReaderAsync(behavior, cancellationToken);
		}

		public DbDataReader CrateExecuteReader(CommandBehavior behavior = CommandBehavior.Default)
		{
			var task = CrateExecuteReaderAsync(behavior);
			task.ConfigureAwait(false);

			try
			{
				return task.Result;
			}
			catch(AggregateException aggrEx)
			{
				aggrEx = aggrEx.Flatten();

				throw aggrEx.InnerExceptions.First();
			}
		}

		public async Task<DbDataReader> CrateExecuteReaderAsync(CommandBehavior behavior = CommandBehavior.Default, CancellationToken cancellationToken = default(CancellationToken))
		{
			// TODO: I'm doing nothing with the CommandBehavior atm - not best practise

			var resp = await ExecuteAsync(cancellationToken);

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