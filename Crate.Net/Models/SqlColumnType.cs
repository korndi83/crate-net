using System;
using System.Collections.Generic;
using Crate.Net.Client.Helper;
using Newtonsoft.Json;

namespace Crate.Net.Client.Models
{
    [Serializable]
    public class SqlColumnType
    {
        [JsonConverter(typeof(SingleOrArrayConverter<int>))]
        public List<int> Types { get; set; }
    }
}
