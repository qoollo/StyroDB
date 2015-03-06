using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;

namespace StyroDB.Adapter.StyroClient
{
    public class StyroDataReader: IDataReader
    {
        class Property
        {
            public Property(string name, Object value)
            {
                Name = name;
                Value = value;
            }

            public readonly string Name;
            public readonly Object Value;
        }

        private List<Property> _currentValue; 
        private IEnumerator<Object> _values;

        internal StyroDataReader(IEnumerable<object> values )
        {
            Contract.Requires(values!=null);
            _values = values.GetEnumerator();            
        }

        private List<Property> GetValueProperies(Object value)
        {
            var propList = new List<Property>();
            var valueType = value.GetType();
            var properties = valueType.GetProperties();

            if (properties.Length == 0)
            {
                propList.Add(new Property(valueType.FullName, value));
            }
            else
            {
                foreach (var prop in properties)
                {
                    propList.Add(new Property(prop.Name, prop.GetValue(value, null)));
                }
            }
            
            return propList;    
        }

        public void Dispose()
        {
            Close();
            _currentValue = null;
            _values = null;
        }

        public string GetName(int i)
        {
            if (_currentValue != null)
            {
                try
                {
                    return _currentValue[i].Name;
                }
                catch (Exception)
                {
                    return null;
                }
                
            }
            return null;
        }

        public Type GetFieldType(int i)
        {
            if (_currentValue != null)
            {
                try
                {
                    return _currentValue[i].Value.GetType();
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public object GetValue(int i)
        {
            if (_currentValue != null)
            {
                try
                {
                    return _currentValue[i].Value;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public object GetValue(string name)
        {
            var propIndex = GetOrdinal(name);
            if (propIndex != -1)
            {
                return _currentValue[propIndex];
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
                    return _currentValue.IndexOf(prop);
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
