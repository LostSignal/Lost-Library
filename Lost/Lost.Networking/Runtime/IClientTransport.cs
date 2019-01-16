//-----------------------------------------------------------------------
// <copyright file="IClientTransportLayer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public enum ClientEventType
    {
        ConnectionOpened,
        ConnectionClosed,
        ConnectionLost,
        ReceivedData,
    }

    public struct ClientEvent
    {
        public ClientEventType EventType;
        public byte[] Data;
    }

    public interface IClientTransportLayer
    {
        bool IsConnecting { get; }

        bool IsConnected { get; }

        void Connect(string connectionString);

        void SendData(byte[] data, uint offset, uint length);

        void Shutdown();

        bool TryDequeueClientEvent(out ClientEvent clientEvent);
    }
}
