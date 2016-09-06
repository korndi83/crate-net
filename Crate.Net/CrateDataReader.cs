using System;
using System.Data;
using Crate.Net.Client.Constants;
using Crate.Net.Client.Helper;
using Crate.Net.Client.Models;

namespace Crate.Net.Client
{
    public class CrateDataReader : IDataReader
    {
        private readonly SqlResponse _sqlResponse;
        private int _currentRow = -1;
        private readonly DateTime _unixDt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private DataTable _schemaTable;

        public CrateDataReader(SqlResponse sqlResponse)
        {
            _sqlResponse = sqlResponse;
        }

        #region IDataReader implementation

        public void Close()
        {
            IsClosed = true;
        }

        private DataTable GenerateSchemaTable()
        {
            // schema table always must be named as "SchemaTable"
            var table = new DataTable("SchemaTable");

            // create table schema columns
            table.Columns.Add(CrateSchemaTableColumns.ColumnName, typeof(string));
            table.Columns.Add(CrateSchemaTableColumns.ColumnOrdinal, typeof(int));
            table.Columns.Add(CrateSchemaTableColumns.ColumnSize, typeof(int));
            table.Columns.Add(CrateSchemaTableColumns.NumericPrecision, typeof(int));
            table.Columns.Add(CrateSchemaTableColumns.NumericScale, typeof(int));
            table.Columns.Add(CrateSchemaTableColumns.DataType, typeof(Type));
            table.Columns.Add(CrateSchemaTableColumns.ProviderType, typeof(int));
            table.Columns.Add(CrateSchemaTableColumns.ProviderSpecificDataType, typeof(DbType));
            table.Columns.Add(CrateSchemaTableColumns.IsLong, typeof(bool));
            table.Columns.Add(CrateSchemaTableColumns.AllowDbNull, typeof(bool));
            table.Columns.Add(CrateSchemaTableColumns.IsReadOnly, typeof(bool));
            table.Columns.Add(CrateSchemaTableColumns.IsRowVersion, typeof(bool));
            table.Columns.Add(CrateSchemaTableColumns.IsUnique, typeof(bool));
            table.Columns.Add(CrateSchemaTableColumns.IsKey, typeof(bool));
            table.Columns.Add(CrateSchemaTableColumns.IsAutoincrement, typeof(bool));
            table.Columns.Add(CrateSchemaTableColumns.BaseSchemaName, typeof(string));
            table.Columns.Add(CrateSchemaTableColumns.BaseCatalogName, typeof(string));
            table.Columns.Add(CrateSchemaTableColumns.BaseTableName, typeof(string));
            table.Columns.Add(CrateSchemaTableColumns.BaseColumnName, typeof(string));
            table.Columns.Add(CrateSchemaTableColumns.DataTypeName, typeof(string));

            // fill schema table
            for (var fieldIndex = 0; fieldIndex < FieldCount; fieldIndex++)
            {
                // get column name
                var columnName = GetName(fieldIndex);

                // get system type
                var systemType = GetFieldType(fieldIndex);
                var providerType = CrateTypeMap.GetDbTypeById(_sqlResponse.ColumnTypes[fieldIndex].Types[0]);
                var providerTypeName = GetDataTypeName(fieldIndex);

                table.Rows.Add(
                        columnName,
                        fieldIndex,
                        0,  // size
                        0,  // precision
                        0,  // scale
                        systemType, // system type
                        _sqlResponse.ColumnTypes[fieldIndex].Types[0], // provider type id
                        providerType, // provider type
                        false,  // is blob
                        true,   // nullable
                        true,   // is readonly
                        false,  // is row version
                        false,  // is unique
                        false,  // is key
                        false,  // is autoincrement
                        null,   // base schema name
                        null,   // catalog name
                        null,   // base table name
                        columnName, // base column name
                        providerTypeName // provider type name
                    );
            }

            return table;
        }

        public DataTable GetSchemaTable()
        {
            // Generate schema table when requested,
            // cause building schema table is so expensive operation
            return _schemaTable ?? (_schemaTable = GenerateSchemaTable());
        }

        public bool NextResult()
        {
            return _sqlResponse.Rows.Length > _currentRow;
        }

        public bool Read()
        {
            _currentRow++;
            return NextResult();
        }

        public int Depth { get; private set; }

        public bool IsClosed { get; private set; }

        public int RecordsAffected => _sqlResponse.RowCount;

        #endregion

        #region IDataRecord implementation

        public bool GetBoolean(int i)
        {
            return (bool)_sqlResponse.Rows[_currentRow][i];
        }

        public byte GetByte(int i)
        {
            return (byte)_sqlResponse.Rows[_currentRow][i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return 0;
        }

        public char GetChar(int i)
        {
            return (char)_sqlResponse.Rows[_currentRow][i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return 0;
        }

        public IDataReader GetData(int i)
        {
            return null;
        }

        public string GetDataTypeName(int i)
        {
            return CrateTypeMap.GetTypeNameById(_sqlResponse.ColumnTypes[i].Types[0]);
        }

        public DateTime GetDateTime(int i)
        {
            return _unixDt.AddMilliseconds(GetInt64(i));
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)_sqlResponse.Rows[_currentRow][i];
        }

        public double GetDouble(int i)
        {
            return (double)_sqlResponse.Rows[_currentRow][i];
        }

        public Type GetFieldType(int i)
        {
            return CrateTypeMap.GetTypeById(_sqlResponse.ColumnTypes[i].Types[0]);
        }

        public float GetFloat(int i)
        {
            return (float)_sqlResponse.Rows[_currentRow][i];
        }

        public Guid GetGuid(int i)
        {
            return Guid.Parse((string)_sqlResponse.Rows[_currentRow][i]);
        }

        public short GetInt16(int i)
        {
            return (short)_sqlResponse.Rows[_currentRow][i];
        }

        public int GetInt32(int i)
        {
            return (int)_sqlResponse.Rows[_currentRow][i];
        }

        public long GetInt64(int i)
        {
            return (long)_sqlResponse.Rows[_currentRow][i];
        }

        public string GetName(int i)
        {
            return _sqlResponse.Cols[i];
        }

        public int GetOrdinal(string name)
        {
            for (var index = 0; index < _sqlResponse.Cols.Length; index++)
            {
                if (_sqlResponse.Cols[index].Equals(name, StringComparison.Ordinal))
                    return index;
            }

            return -1;
        }

        public string GetString(int i)
        {
            return (string)_sqlResponse.Rows[_currentRow][i];
        }

        public object GetValue(int i)
        {
            return _sqlResponse.Rows[_currentRow][i];
        }

        public int GetValues(object[] values)
        {
            var i = 0;
            var j = 0;
            for (; i < values.Length && j < _sqlResponse.Cols.Length; i++, j++)
            {
                values[i] = _sqlResponse.Rows[_currentRow][j];
            }
            return i;
        }

        public bool IsDBNull(int i)
        {
            return _sqlResponse.Rows[_currentRow][i] == null;
        }

        public int FieldCount => _sqlResponse.Cols.Length;

        public object this[string name] => _sqlResponse.Rows[_currentRow][GetOrdinal(name)];

        public object this[int index] => _sqlResponse.Rows[_currentRow][index];

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
        }

        #endregion
    }
}

