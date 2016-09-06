using System.Data.Common;
using Crate.Net.Client.Models;

namespace Crate.Net.Client
{
	public class CrateException : DbException
	{
		public CrateException (string message) 
            : base(message)
		{
		}

        public CrateException(SqlError error)
            : base(error.ToString())
        {
        }
	}
}

