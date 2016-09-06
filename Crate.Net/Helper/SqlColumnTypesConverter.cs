using System;
using System.Collections.Generic;
using System.Linq;
using Crate.Net.Client.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Crate.Net.Client.Helper
{
    public class SqlColumnTypesConverter : JsonConverter
    {
        private SingleOrArrayConverter<int> valueConverter = new SingleOrArrayConverter<int>();

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<SqlColumnType>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            if (token.Type != JTokenType.Array)
                return null;

            var result = new List<SqlColumnType>();

            foreach (var val in token)
            {
                var colType = new SqlColumnType { Types = new List<int>() };
                
                if (val.Type == JTokenType.Array)
                {
                    colType.Types.AddRange(val.Select(v => v.ToObject<int>()));
                }
                else
                    colType.Types.Add(val.ToObject<int>());

                result.Add(colType);
            }

            return result;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
