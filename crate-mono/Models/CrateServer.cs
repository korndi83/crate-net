using Crate.Client.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crate.Client.Models
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
}
