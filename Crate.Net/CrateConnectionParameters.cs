using Crate.Client.Constants;
using System;
using System.Collections.Generic;

namespace Crate.Client
{
    /// <summary>
    /// Crate connection parameters
    /// </summary>
    public class CrateConnectionParameters
    {
        public IDictionary<string, int> Nodes { get; set; }
    }
}
