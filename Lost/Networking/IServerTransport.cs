//-----------------------------------------------------------------------
// <copyright file="IServerTransportLayer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public enum ServerEventType
    {
        ConnectionOpened,
        ConnectionClosed,
        ConnectionLost,
        ReceivedData,
    }

    public struct ServerEvent
    {
        public ServerEventType EventType;
        public long ConnectionId;
        public byte[] Data;
    }

    public interface IServerTransportLayer
    {
        bool IsStarting { get; }

        bool IsRunning { get; }

        void Start(int port);

        void SendData(long connectionId, byte[] data, uint offset, uint length);

        void Shutdown();

        bool TryDequeueServerEvent(out ServerEvent serverEvent);
    }
}
