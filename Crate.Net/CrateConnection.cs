using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Crate.Net.Client.Extensions;
using Crate.Net.Client.Models;

namespace Crate.Net.Client
{
    public class CrateConnection : IDbConnection
    {
        private readonly IList<CrateServer> _allServers;
        private int _currentServer = 0;
        private readonly object _lockObj1 = new object();
        private readonly object _lockObj2 = new object();

        private readonly CrateConnectionParameters _parameters;

        public IList<CrateServer> ActiveServers { get; private set; }

        public CrateConnection()
            : this("Server=localhost;Port=4200") { }

        public CrateConnection(string connectionString)
        {
            _parameters = connectionString.ToConnectionParameters();

            _allServers = new List<CrateServer>();

            foreach (var node in _parameters.Nodes)
            {
                _allServers.Add(new CrateServer(node));
            }

            ActiveServers = _allServers;

            ConnectionString = connectionString;
            State = ConnectionState.Closed;
        }

        public CrateServer NextServer()
        {
            lock (_lockObj1)
            {
                var server = ActiveServers[_currentServer];
                _currentServer++;
                if (_currentServer >= ActiveServers.Count)
                {
                    _currentServer = 0;
                }
                return server;
            }
        }

        public void MarkAsFailed(CrateServer server)
        {
            lock (_lockObj1)
            {
                if (ActiveServers.Count == 1)
                {
                    ActiveServers = _allServers;
                }
                ActiveServers.Remove(server);
                Task.Delay(TimeSpan.FromMinutes(3)).ContinueWith(x => AddServer(server));
                _currentServer = 0;
            }
        }

        private void AddServer(CrateServer server)
        {
            lock (_lockObj1)
            {
                if (!ActiveServers.Contains(server))
                {
                    ActiveServers.Add(server);
                }
            }
        }

        #region IDbConnection implementation

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            lock (_lockObj2)
            {
                State = ConnectionState.Closed;
            }
        }

        public IDbCommand CreateCommand()
        {
            return new CrateCommand(null, this);
        }

        public void Open()
        {
            lock (_lockObj2)
            {
                State = ConnectionState.Connecting;

                using (var cmd = CreateCommand())
                {
                    cmd.CommandText = "select id from sys.cluster";
                    var reader = cmd.ExecuteReader();
                    reader.Read();
                }

                State = ConnectionState.Open;
            }
        }

        public string ConnectionString { get; set; }

        public int ConnectionTimeout
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Database
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ConnectionState State { get; private set; }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}

