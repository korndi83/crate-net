using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Crate.Net.Client
{
    public class CrateParameterCollection : List<CrateParameter>, IDataParameterCollection
    {
        public void RemoveAt(string parameterName)
        {
            RemoveAt(IndexOf(parameterName));
        }

        public int IndexOf(string parameterName)
        {
            return FindIndex(x => x.ParameterName == parameterName);
        }

        public bool Contains(string parameterName)
        {
            return this.Any(x => x.ParameterName == parameterName);
        }

        public object this[string parameterName]
        {
            get
            {
                return this.FirstOrDefault(x => x.ParameterName == parameterName);
            }
            set
            {
                this[IndexOf(parameterName)] = (CrateParameter)value;
            }
        }
    }
}
