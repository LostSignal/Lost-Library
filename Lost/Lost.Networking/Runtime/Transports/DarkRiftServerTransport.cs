//-----------------------------------------------------------------------
// <copyright file="DarkRiftServerTransport.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_DARKRIFT_2

namespace Lost.Networking
{
    using System.Net;
    using DarkRift.Server;

    public class DarkRiftServerTransport : IServerTransportLayer
    {
        private DarkRiftServer server;

        bool IServerTransportLayer.IsStarting => throw new System.NotImplementedException();

        bool IServerTransportLayer.IsRunning => throw new System.NotImplementedException();

        public void Update()
        {
            this.server?.ExecuteDispatcherTasks();
        }

        void IServerTransportLayer.SendData(long connectionId, byte[] data, uint offset, uint length)
        {
            throw new System.NotImplementedException();
        }

        void IServerTransportLayer.Start(int port)
        {
            if (this.server != null)
            {
                this.Shutdown();
            }

            this.server = new DarkRiftServer(new ServerSpawnData(IPAddress.Parse("127.0.0.1"), 9999, DarkRift.IPVersion.IPv4));
            this.server.Start();
        }

        bool IServerTransportLayer.TryDequeueServerEvent(out ServerEvent serverEvent)
        {
            throw new System.NotImplementedException();
        }

        void IServerTransportLayer.Shutdown()
        {
            this.Shutdown();
        }

        private void Shutdown()
        {
            if (this.server != null)
            {
                this.server.Dispose();
                this.server = null;
            }
        }
    }
}

#endif
