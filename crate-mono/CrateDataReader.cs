using System;
using System.Data;

namespace Crate.Client
{
	public class CrateDataReader : IDataReader
	{
		private readonly SqlResponse _sqlResponse;
		private int _currentRow = -1;
	    private readonly DateTime _unixDt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public CrateDataReader (SqlResponse sqlResponse)
		{
			_sqlResponse = sqlResponse;
		}

		#region IDataReader implementation

		public void Close ()
		{
			IsClosed = true;
		}

		public DataTable GetSchemaTable ()
		{
			throw new NotImplementedException ();
		}

		public bool NextResult ()
		{
			return _sqlResponse.Rows.Length > _currentRow;
		}

		public bool Read ()
		{
			_currentRow++;
			return NextResult();
		}

		public int Depth {
			get {
				throw new NotImplementedException ();
			}
		}

		public bool IsClosed { get; private set; }

	    public int RecordsAffected => _sqlResponse.RowCount;

	    #endregion

		#region IDataRecord implementation

		public bool GetBoolean (int i)
		{
			return (bool)_sqlResponse.Rows[_currentRow][i];
		}

		public byte GetByte (int i)
		{
			return (byte)_sqlResponse.Rows[_currentRow][i];
		}

		public long GetBytes (int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException ();
		}

		public char GetChar (int i)
		{
			return (char)_sqlResponse.Rows[_currentRow][i];
		}

		public long GetChars (int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException ();
		}

		public IDataReader GetData (int i)
		{
			throw new NotImplementedException ();
		}

		public string GetDataTypeName (int i)
		{
			throw new NotImplementedException ();
		}

		public DateTime GetDateTime (int i)
		{
			return _unixDt.AddMilliseconds(GetInt64(i));
		}

		public decimal GetDecimal (int i)
		{
			return (decimal)_sqlResponse.Rows[_currentRow][i];
		}

		public double GetDouble (int i)
		{
			return (double)_sqlResponse.Rows[_currentRow][i];
		}

		public Type GetFieldType (int i)
		{
			throw new NotImplementedException ();
		}

		public float GetFloat (int i)
		{
			return (float)_sqlResponse.Rows[_currentRow][i];
		}

		public Guid GetGuid (int i)
		{
			return Guid.Parse((string)_sqlResponse.Rows[_currentRow][i]);
		}

		public short GetInt16 (int i)
		{
			return (short)_sqlResponse.Rows[_currentRow][i];
		}

		public int GetInt32 (int i)
		{
			return (int)_sqlResponse.Rows[_currentRow][i];
		}

		public long GetInt64 (int i)
		{
			return (long)_sqlResponse.Rows[_currentRow][i];
		}

		public string GetName (int i)
		{
			return _sqlResponse.Cols[i];
		}

		public int GetOrdinal (string name)
		{
			return Array.BinarySearch(_sqlResponse.Cols, name);
		}

		public string GetString (int i)
		{
			return (string)_sqlResponse.Rows[_currentRow][i];
		}

		public object GetValue (int i)
		{
			return _sqlResponse.Rows[_currentRow][i];
		}

		public int GetValues (object[] values)
		{
			var i = 0;
			var j = 0;
			for (; i < values.Length && j < _sqlResponse.Cols.Length; i++, j++) {
				values[i] = _sqlResponse.Rows[_currentRow][j];
			}
			return i;
		}

		public bool IsDBNull (int i)
		{
			return _sqlResponse.Rows[_currentRow][i] == null;
		}

		public int FieldCount => _sqlResponse.Cols.Length;

	    public object this [string name] => _sqlResponse.Rows[_currentRow][GetOrdinal(name)];

	    public object this [int index] => _sqlResponse.Rows[_currentRow][index];

	    #endregion

		#region IDisposable implementation

		public void Dispose ()
		{
		}

		#endregion
	}
}

