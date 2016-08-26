using Newtonsoft.Json;
using System;

namespace Crate.Client
{
    [Serializable]
    public class SqlError
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        public override string ToString()
        {
            return $"CRATE-{Code}: {Message}";
        }
    }
}
