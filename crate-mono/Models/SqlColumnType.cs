using Crate.Client.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crate.Client.Models
{
    [Serializable]
    public class SqlColumnType
    {
        [JsonConverter(typeof(SingleOrArrayConverter<int>))]
        public List<int> Types { get; set; }
    }
}
