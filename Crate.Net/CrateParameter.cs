using System;
using System.Data;
using System.Data.Common;

namespace Crate.Net.Client
{
	public class CrateParameter : DbParameter
	{
		public CrateParameter()
		{
			Direction = ParameterDirection.Input;
			SourceVersion = DataRowVersion.Current;
		}

		public CrateParameter(string parameterName, DbType type)
		{
			ParameterName = parameterName;
			DbType = type;
			Direction = ParameterDirection.Input;
			SourceVersion = DataRowVersion.Current;
		}

		public CrateParameter(string parameterName, object value)
		{
			ParameterName = parameterName;
			Direction = ParameterDirection.Input;
			SourceVersion = DataRowVersion.Current;
			DbType = InferType(value);
			Value = value;
		}

		public byte Precision { get; set; }
		public byte Scale { get; set; }
		public override bool IsNullable { get; set; }
		public override DbType DbType { get; set; }
		public override ParameterDirection Direction { get; set; }
		public override string ParameterName { get; set; }
		public override int Size { get; set; }
		public override string SourceColumn { get; set; }
		public override bool SourceColumnNullMapping { get; set; }
		public override DataRowVersion SourceVersion { get; set; }
		public override object Value { get; set; }

		private DbType InferType(object value)
		{
			switch(Type.GetTypeCode(value.GetType()))
			{
				case TypeCode.Empty:
					throw new ArgumentException();
				case TypeCode.Object:
				case TypeCode.DBNull:
					return DbType.Object;
				case TypeCode.Boolean:
					return DbType.Boolean;
				case TypeCode.Char:
					return DbType.String;
				case TypeCode.SByte:
					return DbType.Byte;
				case TypeCode.Byte:
					return DbType.Byte;
				case TypeCode.Int16:
					return DbType.Int16;
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					throw new ArgumentException();
				case TypeCode.Int32:
					return DbType.Int32;
				case TypeCode.Int64:
					return DbType.Int64;
				case TypeCode.Single:
					throw new ArgumentException();
				case TypeCode.Double:
					return DbType.Double;
				case TypeCode.Decimal:
					return DbType.Decimal;
				case TypeCode.DateTime:
					return DbType.DateTime;
				case TypeCode.String:
					return DbType.String;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void ResetDbType()
		{
			throw new NotImplementedException();
		}
	}
}