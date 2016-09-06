using System;
using System.Collections.Generic;
using Crate.Net.Client.Constants;

namespace Crate.Net.Client.Extensions
{
    public static class CrateExt
    {
        /// <summary>
        /// Build connection parameters by parsing connection string
        /// </summary>
        /// <param name="connectionString">Connection string to parse</param>
        /// <returns></returns>
        public static CrateConnectionParameters ToConnectionParameters(this string connectionString)
        {
            var parameters = new CrateConnectionParameters
            {

                Nodes = new Dictionary<string, int>()
            };

            var servers = new List<string>();
            var ports = new List<int>();

            var splitted = connectionString.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            if (splitted.Length == 0)
                return parameters;

            foreach (var entry in splitted)
            {
                var splittedKeyVal = entry.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                if (splittedKeyVal.Length == 2)
                {
                    var splittedVals = splittedKeyVal[1].Split(new[] { "," }, StringSplitOptions.None);
                    switch (splittedKeyVal[0].ToUpper())
                    {
                        case CrateConnectionParameterNames.Server:
                            foreach (var db in splittedVals)
                            {
                                if (string.IsNullOrEmpty(db))
                                    servers.Add(CrateConstants.DefaultServer);
                                else
                                    servers.Add(db);
                            }
                            break;
                        case CrateConnectionParameterNames.Port:
                            foreach (var p in splittedVals)
                            {
                                int port;
                                if (string.IsNullOrEmpty(p))
                                    port = CrateConstants.DefaultPort;
                                else
                                {
                                    if (!int.TryParse(p, out port))
                                        port = CrateConstants.DefaultPort;
                                }

                                ports.Add(port);
                            }
                            break;
                        default:
                            continue;
                    }
                }
            }

            var diff = servers.Count - ports.Count;

            if (diff > 0)
            {
                for (var i = 0; i < diff; i++)
                    ports.Add(CrateConstants.DefaultPort);
            }
            else if (diff < 0)
            {
                diff = -diff;

                for (var i = 0; i < diff; i++)
                    servers.Add(CrateConstants.DefaultServer);
            }

            // create node string
            for (var serverId = 0; serverId < servers.Count; serverId++)
            {
                var server = servers[serverId];
                var port = ports[serverId];

                parameters.Nodes[server] = port;
            }

            return parameters;
        }
    }
}
