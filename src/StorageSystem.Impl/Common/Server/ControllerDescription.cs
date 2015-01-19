﻿namespace Qoollo.Impl.Common.Server
{
    public class ControllerDescription:ServerId
    {
        public ControllerDescription(string host,  int port)
            : base(host,  port)
        {
            IsAvailable = true;
            IsServerRestored = true;
        }

        public ControllerDescription(ServerId server) : this(server.RemoteHost, server.Port)
        {
        }

        public bool IsAvailable { get; private set; }
        public bool IsServerRestored { get; private set; }

        public void NotAvailable()
        {
            IsAvailable = false;
            IsServerRestored = false;
        }

        public void Available()
        {
            IsAvailable = true;
        }

        public void Restored()
        {
            IsServerRestored = true;
        }
    }
}
