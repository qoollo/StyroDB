namespace BricksDb.RedisInterface.RedisOperations
{
    class RedisSet : RedisOperation
    {
        private readonly IDataAdapter _dataAdapter;

        public RedisSet(IDataAdapter dataAdapter, string operationName)
            : base(operationName)
        {
            _dataAdapter = dataAdapter;
        }

        public override string PerformOperation(string[] parameterArray)
        {            
            //var key = parameterArray[0];
            //var value = parameterArray[1];
            //var responseBriks = _dataAdapter.Create(key, value);
            //if (responseBriks.IsError)
            //    Fail();
            //else
            //    Success();
            return "+OK\r\n";
        }

    }
}
