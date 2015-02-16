using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.Adapter
{
    class StyroDataReader: IDataReader
    {
        class Property
        {
            public Property(string name, int ordinal, Type type, Object value)
            {
                Name = name;
                Ordinal = ordinal;
                TypeValue = type;
                Value = value;
            }

            public readonly string Name;
            public readonly int Ordinal;
            public readonly Type TypeValue;
            public readonly Object Value;
        }

        private IEnumerable<Property> _currentValue; 
        private IEnumerator<Object> _values;

        private IEnumerable<Property> GetValueProperies(Object value)
        {
            var order = 0;
            var propList = new List<Property>();
            foreach (var prop in value.GetType().GetProperties())
            {
                propList.Add(new Property(prop.Name, order, prop.PropertyType, prop.GetConstantValue()));
                order++;
            }
            return propList;    
        }

        public void Dispose()
        {
            _currentValue = null;
            _values = null;
        }

        public string GetName(int i)
        {
            if (_currentValue != null)
            {
                var prop = _currentValue.FirstOrDefault(val => val.Ordinal == i);
                if (prop != null)
                {
                    return prop.Name;
                }
            }
            return null;
        }

        public Type GetFieldType(int i)
        {
            if (_currentValue != null)
            {
                var prop = _currentValue.FirstOrDefault(val => val.Ordinal == i);
                if (prop != null)
                {
                    return prop.TypeValue;
                }
            }
            return null;
        }

        public object GetValue(int i)
        {
            if (_currentValue != null)
            {
                var prop = _currentValue.FirstOrDefault(val => val.Ordinal == i);
                if (prop != null)
                {
                    return prop.Value;
                }
            }
            return null;
        }

        public object GetValue(string name)
        {
            if (_currentValue != null)
            {
                var prop = _currentValue.FirstOrDefault(val => val.Name == name);
                if (prop != null)
                {
                    return prop.Value;
                }
            }
            return null;
        }
        
        public int GetOrdinal(string name)
        {
            if (_currentValue != null)
            {
                var prop = _currentValue.FirstOrDefault(val => val.Name == name);
                if (prop != null)
                {
                    return prop.Ordinal;
                }
            }
            return -1;
        }

        object IDataRecord.this[int i]
        {
            get { return GetValue(i); }
        }

        object IDataRecord.this[string name]
        {
            get { return GetValue(name); }
        }

        public void Close()
        {
            IsClosed = true;
        }
        
        public bool Read()
        {
            var result = _values.MoveNext();
            if (result)
            {
                _currentValue = GetValueProperies(_values.Current);
            }
            return result;
        }

        public int FieldCount
        {
            get { return _currentValue.Count(); }
        }

        public bool IsClosed { get; private set; }


        #region NotImplemented

        public int RecordsAffected
        {
            get{throw new NotImplementedException();}
        }

        public int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
