using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Crate.Net.Client
{
	public class CrateParameterCollection : DbParameterCollection
	{
		private readonly List<CrateParameter> _internalParameterList = new List<CrateParameter>();

		public override int Count
		{
			get
			{
				return _internalParameterList.Count;
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override bool IsSynchronized
		{
			get
			{
				var collection = _internalParameterList as ICollection;
				return collection.IsSynchronized;
			}
		}

		public override object SyncRoot
		{
			get
			{
				var collection = _internalParameterList as ICollection;
				return collection.SyncRoot;
			}
		}

		/// <summary>
		/// Adds parameter to the parameter collection
		/// Beware that for Crate the order of parameters is extremly important!
		/// </summary>
		/// <param name="value">CrateParameter which has to be added</param>
		/// <returns>Last index of the ParameterCollection</returns>
		public override int Add(object value)
		{
			_internalParameterList.Add(CheckAndCastObject(value));

			return _internalParameterList.Count - 1;
		}

		public override void AddRange(Array values)
		{
			foreach(var value in values)
			{
				Add(value);
			}
		}

		public override void Clear()
		{
			_internalParameterList.Clear();
		}

		public override bool Contains(string value)
		{
			return _internalParameterList.Any(x => x.ParameterName == value);
		}

		public override bool Contains(object value)
		{
			return _internalParameterList.Contains(value);
		}

		public override void CopyTo(Array array, int index)
		{
			var parameters = new List<CrateParameter>();
			foreach(var value in array)
			{
				parameters.Add(CheckAndCastObject(value));
			}

			_internalParameterList.CopyTo(parameters.ToArray(), index);
		}

		public override IEnumerator GetEnumerator()
		{
			return _internalParameterList.GetEnumerator();
		}

		public override int IndexOf(string parameterName)
		{
			return _internalParameterList.FindIndex(x => x.ParameterName == parameterName);
		}

		public override int IndexOf(object value)
		{
			return _internalParameterList.IndexOf(CheckAndCastObject(value));
		}

		public override void Insert(int index, object value)
		{
			_internalParameterList.Insert(index, CheckAndCastObject(value));
		}

		public override void Remove(object value)
		{
			_internalParameterList.Remove(CheckAndCastObject(value));
		}

		public override void RemoveAt(string parameterName)
		{
			_internalParameterList.RemoveAt(IndexOf(parameterName));
		}

		public override void RemoveAt(int index)
		{
			_internalParameterList.RemoveAt(index);
		}

		protected override DbParameter GetParameter(string parameterName)
		{
			int index = IndexOf(parameterName);
			return _internalParameterList[index];
		}

		protected override DbParameter GetParameter(int index)
		{
			return _internalParameterList[index];
		}

		protected override void SetParameter(string parameterName, DbParameter value)
		{
			int index = IndexOf(parameterName);

			if(index < 0)
			{
				_internalParameterList.Add(CheckAndCastObject(value));
			}
			else
			{
				RemoveAt(index);
				_internalParameterList.Insert(index, CheckAndCastObject(value));
			}
		}

		protected override void SetParameter(int index, DbParameter value)
		{
			RemoveAt(index);
			_internalParameterList.Insert(index, CheckAndCastObject(value));
		}

		public IEnumerable<TResult> Select<TResult>(Func<CrateParameter, TResult> selector)
		{
			return _internalParameterList.Select(selector);
		}

		private CrateParameter CheckAndCastObject(object value)
		{
			if(!(value is CrateParameter))
				throw new NotSupportedException();

			var val = value as CrateParameter;

			return val;
		}
	}
}
