using System.Collections.Generic;

namespace Crate.Net.Client
{
    /// <summary>
    /// Crate connection parameters
    /// </summary>
    public class CrateConnectionParameters
    {
        public IDictionary<string, int> Nodes { get; set; }
    }
}
