using System;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading.Tasks;
using Crate.Client.Constants;

namespace Crate.Client
{
    [DebuggerDisplay("<CrateServer {Hostname}:{Port}>")]
    public class CrateServer
    {
        private readonly Regex _serverRex = new Regex(@"^(https?)?(://)?([^:]*):?(\d*)$");

        public string Scheme { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }

        public CrateServer()
        {
            Hostname = CrateConstants.DefaultServer;
            Scheme = CrateConstants.DefaultTransport;
            Port = CrateConstants.DefaultPort;
        }

        public CrateServer(KeyValuePair<string, int> serverAndPort)
            : this()
        {
            Hostname = serverAndPort.Key;
            Port = serverAndPort.Value;
        }

        public CrateServer(string server)
            : this()
        {
            if (server == null)
                return;

            var m = _serverRex.Match(server);

            if (!m.Success)
                return;

            Scheme = string.IsNullOrEmpty(m.Groups[1].Value) ? CrateConstants.DefaultTransport : m.Groups[1].Value;
            Hostname = string.IsNullOrEmpty(m.Groups[3].Value) ? CrateConstants.DefaultServer : m.Groups[3].Value;
            Port = string.IsNullOrEmpty(m.Groups[4].Value) ? CrateConstants.DefaultPort : int.Parse(m.Groups[4].Value);
        }

        public string SqlUri()
        {
            return string.Format("{0}://{1}:{2}/_sql", Scheme, Hostname, Port);
        }
    }

    public class CrateConnection : IDbConnection
    {
        private readonly IList<CrateServer> _allServers;
        private int _currentServer = 0;
        private readonly object _lockObj = new object();

        private readonly CrateConnectionParameters _parameters;

        public IList<CrateServer> ActiveServers { get; private set; }

        public CrateConnection()
            : this("Server=localhost;Port=4200") { }

        public CrateConnection(string connectionString)
        {
            _parameters = CrateConnectionParameters.FromConnectionString(connectionString);

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
            lock (_lockObj)
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
            lock (_lockObj)
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
            lock (_lockObj)
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
            lock (_lockObj)
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
            lock (_lockObj)
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

