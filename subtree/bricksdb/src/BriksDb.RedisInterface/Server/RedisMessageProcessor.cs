using System;
using System.Collections.Generic;
using System.Threading;
using BricksDb.RedisInterface.RedisOperations;

namespace BricksDb.RedisInterface.Server
{
    class RedisMessageProcessor
    {
        private readonly Dictionary<char, Func<string, string>> _processOnDataType;
        private readonly Dictionary<string, RedisOperation> _executeCommand;
        private Timer _timer;

        public RedisMessageProcessor()
        {
            _processOnDataType = new Dictionary<char, Func<string, string>>()
            {
                //{'+', ProcessStrings},
                //{'-', ProcessErrors},
                //{':', ProcessIntegers},
                //{'$', ProcessBulk},
                {'*', ProcessArrays}
            };
            _executeCommand = new Dictionary<string, RedisOperation>();
        }

        public void AddOperation(string name, RedisOperation operation)
        {
            _executeCommand.Add(name, operation);
        }

        public void Start()
        {
            _timer = new Timer(Callback, null, 1000, 2000);
        }

        public void Stop()
        {
            _timer.Dispose();
        }

        private void Callback(object state)
        {
            Console.WriteLine("--------------------------------------");
            foreach (var redisOperation in _executeCommand)
            {
                redisOperation.Value.WritePerformanceToConsole();
            }
        }

        #region Not implemented

        

        public string ProcessStrings(string data)
        {
            return "";
        }

        public string ProcessErrors(string data)
        {
            return "";
        }

        public string ProcessIntegers(string data)
        {
            return "";
        }

        public string ProcessBulk(string data)
        {
            return "";
        }

        #endregion

        public string ProcessMessage(string message)
        {
            return ProcessArrays(message);
            //return _processOnDataType[message[0]](message);;
        }

        public string ProcessArrays(string data)
        {
            return "*1\r\n$4\r\npong\r\n";
            var array = data.Substring(1).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var numParams = Convert.ToInt32(array[0]) - 1;            
            var parameters = new string[numParams];
            for (int i = 0; i < numParams; i++)
            {
                parameters[i] = array[4 + i * 2];
            }            
            return _executeCommand[array[2]].PerformOperation(parameters);
        }


    }
}
