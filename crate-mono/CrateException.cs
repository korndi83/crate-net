using System;
using System.Data.Common;

namespace Crate.Client
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

