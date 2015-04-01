using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qoollo.Impl.Modules.Db.Impl;
using Qoollo.Turbo.ObjectPools;
using StyroDB.Adapter.StyroClient;

namespace StyroDB.Adapter.Internal
{
    internal class StyroReader:DbReader<StyroDataReader>
    {
        private readonly StyroCommand _command;
        private readonly RentedElementMonitor<StyroConnection> _connection;
        private StyroDataReader _reader;

        public StyroReader(StyroCommand command, RentedElementMonitor<StyroConnection> connection)
        {
            _command = command;
            _connection = connection;
        }

        public override StyroDataReader Reader
        {
            get { return _reader; }
        }

        public override bool IsCanRead
        {
            get { return _reader.Read(); }
        }

        protected override int CountFieldsInner()
        {
            return _reader.FieldCount;
        }

        protected override void ReadNextInner()
        {
        }

        protected override object GetValueInner(int index)
        {
            return _reader.GetValue(index);
        }

        protected override object GetValueInner(string index)
        {
            return _reader.GetValue(index);
        }

        protected override bool IsValidRead()
        {
            return _reader != null && !_reader.IsClosed;
        }

        protected override void StartInner()
        {
            _command.Connection = _connection.Element;
            _reader = _command.ExecuteReader();
        }

        protected override void Dispose(bool isUserCall)
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader.Dispose();
            }
            _connection.Dispose();
        }
    }
}
