//-----------------------------------------------------------------------
// <copyright file="SimpleServer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Match;

    public class SimpleServer : NetworkServerSimple
    {
        public delegate void OnConnectErrorDelegate(int connectionId, byte error);
        public delegate void OnDataErrorDelegate(NetworkConnection conn, byte error);
        public delegate void OnDisconnectErrorDelegate(NetworkConnection conn, byte error);
        public delegate void OnConnectedDelegate(NetworkConnection conn);
        public delegate void OnDisconnectedDelegate(NetworkConnection conn);

        public event OnConnectErrorDelegate OnConnectErrorEvent;
        public event OnDataErrorDelegate OnDataErrorEvent;
        public event OnDisconnectErrorDelegate OnDisconnectErrorEvent;
        public event OnConnectedDelegate OnConnectedEvent;
        public event OnDisconnectedDelegate OnDisconnectedEvent;


        public void ListenMatchInfo(MatchInfo matchInfo)
        {
            if (!matchInfo.usingRelay)
            {
                Debug.LogError("Trying to ListenRelay on matchInfo that is not using relay.");
                return;
            }

            this.ListenRelay(matchInfo.address, matchInfo.port, matchInfo.networkId, Utility.GetSourceID(), matchInfo.nodeId);
        }

        public override void OnConnectError(int connectionId, byte error)
        {
            //// NOTE [bgish]: Not calling base because it only prints an error to the console
            //// base.OnConnectError(connectionId, error);

            if (this.OnConnectErrorEvent != null)
            {
                this.OnConnectErrorEvent(connectionId, error);
            }
        }

        public override void OnDataError(NetworkConnection conn, byte error)
        {
            //// NOTE [bgish]: Not calling base because it only prints an error to the console
            //// base.OnDataError(conn, error);

            if (this.OnDataErrorEvent != null)
            {
                this.OnDataErrorEvent(conn, error);
            }
        }

        public override void OnDisconnectError(NetworkConnection conn, byte error)
        {
            //// NOTE [bgish]: Not calling base because it only prints an error to the console
            //// base.OnDisconnectError(conn, error);

            if (this.OnDisconnectErrorEvent != null)
            {
                this.OnDisconnectErrorEvent(conn, error);
            }
        }

        public override void OnConnected(NetworkConnection conn)
        {
            base.OnConnected(conn);

            if (this.OnConnectedEvent != null)
            {
                this.OnConnectedEvent(conn);
            }
        }

        public override void OnDisconnected(NetworkConnection conn)
        {
            base.OnDisconnected(conn);

            if (this.OnDisconnectedEvent != null)
            {
                this.OnDisconnectedEvent(conn);
            }
        }

        public bool SendMessage(short messageType, MessageBase message)
        {
            bool result = true;

            // this list holds all connections (local and remote)
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection connection = connections[i];
                if (connection != null)
                {
                    result &= connection.Send(messageType, message);
                }
            }

            return result;
        }

        public bool SendMessageUnreliable(short messageType, MessageBase message)
        {
            bool result = true;

            // this list holds all connections (local and remote)
            for (int i = 0; i < connections.Count; i++)
            {
                NetworkConnection connection = connections[i];
                if (connection != null)
                {
                    result &= connection.SendUnreliable(messageType, message);
                }
            }

            return result;
        }
    }
}
