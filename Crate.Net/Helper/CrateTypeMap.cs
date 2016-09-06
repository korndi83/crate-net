using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Crate.Net.Client.Types;

namespace Crate.Net.Client.Helper
{
    /// <summary>
    /// Crate columns type map (names and .NET types)
    /// </summary>
    public static class CrateTypeMap
    {
        private static readonly IDictionary<int, KeyValuePair<string, Type>> TypesDict;

        static CrateTypeMap()
        {
            TypesDict = new ConcurrentDictionary<int, KeyValuePair<string, Type>>()
            {
                [0] = new KeyValuePair<string, Type>("undefined", null),
                [1] = new KeyValuePair<string, Type>("unsupported", null),
                [2] = new KeyValuePair<string, Type>("byte", typeof(byte)),
                [3] = new KeyValuePair<string, Type>("boolean", typeof(bool)),
                [4] = new KeyValuePair<string, Type>("string", typeof(string)),
                [5] = new KeyValuePair<string, Type>("ip", typeof(string)),
                [6] = new KeyValuePair<string, Type>("double", typeof(double)),
                [7] = new KeyValuePair<string, Type>("float", typeof(float)),
                [8] = new KeyValuePair<string, Type>("short", typeof(short)),
                [9] = new KeyValuePair<string, Type>("integer", typeof(int)),
                [10] = new KeyValuePair<string, Type>("long", typeof(long)),
                [11] = new KeyValuePair<string, Type>("timestamp", typeof(DateTime)),
                [12] = new KeyValuePair<string, Type>("object", typeof(object)),
                [13] = new KeyValuePair<string, Type>("geo_point", typeof(GeoPoint)),
                [14] = new KeyValuePair<string, Type>("geo_shape", typeof(GeoShape)),
                [100] = new KeyValuePair<string, Type>("array", typeof(Array)),
                [101] = new KeyValuePair<string, Type>("set", typeof(Array)),
            };
        }

        /// <summary>
        /// Return Crate type name by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetTypeNameById(int id)
        {
            KeyValuePair<string, Type> kvp;

            if (!TypesDict.TryGetValue(id, out kvp))
                return "unsupported";

            return kvp.Key;
        }

        /// <summary>
        /// Return .NET type from Crate type ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Type GetTypeById(int id)
        {
            KeyValuePair<string, Type> kvp;

            if (!TypesDict.TryGetValue(id, out kvp))
                return null;

            return kvp.Value;
        }

        public static DbType GetDbTypeById(int id)
        {
            DbType type;

            switch (id)
            {
                case 2:
                    type = DbType.Byte;
                    break;
                case 3:
                    type = DbType.Boolean;
                    break;
                case 4:
                    type = DbType.String;
                    break;
                case 5:
                    type = DbType.String;
                    break;
                case 6:
                    type = DbType.Double;
                    break;
                case 7:
                    type = DbType.Double;
                    break;
                case 8:
                    type = DbType.Int16;
                    break;
                case 9:
                    type = DbType.Int32;
                    break;
                case 10:
                    type = DbType.Int64;
                    break;
                case 11:
                    type = DbType.DateTime2;
                    break;
                default:
                    type = DbType.Object;
                    break;
            }

            return type;
        }
    }
}
