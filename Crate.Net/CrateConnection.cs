using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Crate.Net.Client.Extensions;
using Crate.Net.Client.Models;

namespace Crate.Net.Client
{
	public class CrateConnection : DbConnection
	{
		private readonly IList<CrateServer> _allServers;
		private readonly CrateConnectionParameters _parameters;

		private string _connectionString;
		private ConnectionState _state;

		public IList<CrateServer> ActiveServers { get; private set; }

		public CrateConnection()
			: this("Server=localhost;Port=4200") { }

		public CrateConnection(string connectionString)
		{
			_parameters = connectionString.ToConnectionParameters();

			_allServers = new List<CrateServer>();

			foreach(var node in _parameters.Nodes)
			{
				_allServers.Add(new CrateServer(node));
			}

			ActiveServers = _allServers;

			ConnectionString = connectionString;
			_state = ConnectionState.Closed;
		}

		public override void ChangeDatabase(string databaseName)
		{
			throw new NotImplementedException();
		}

		public override void Close()
		{
			_state = ConnectionState.Closed;
		}

		protected override DbCommand CreateDbCommand()
		{
			return new CrateCommand(null, this);
		}

		public override void Open()
		{
			// this would be a good place in case we have a strategy for circuit breaking
			_state = ConnectionState.Open;
		}

		public override int ConnectionTimeout
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string Database
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string ConnectionString
		{
			get
			{
				return _connectionString;
			}

			set
			{
				_connectionString = value;
			}
		}

		public override string DataSource
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string ServerVersion
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override ConnectionState State
		{
			get
			{
				return _state;
			}
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			throw new NotImplementedException();
		}
	}
}