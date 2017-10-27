//-----------------------------------------------------------------------
// <copyright file="MessageRoom.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Match;

    public class RegisterMessage
    {
        public short MessageId { get; set; }
        public NetworkMessageDelegate Delegate { get; set; }
        public bool RelayToAllClients { get; set; }
        public bool Reliable { get; set; }

        public RegisterMessage()
        {
            this.RelayToAllClients = true;
            this.Reliable = true;
        }
    }

    public class RoomInfo
    {
        public Options RoomOptions { get; set; }
        public Options ReconnectRoomOptions { get; set; }
        public int MaxConnections { get; set; }
        public int Elo { get; set; }
        
        public RoomInfo()
        {
            this.RoomOptions = new Options();
            this.ReconnectRoomOptions = null;
            this.MaxConnections = 2;
            this.Elo = 0;
        }

        public class Options
        {
            public string RoomName { get; set; }
            public string RoomPassword { get; set; }
            public bool Advertise { get; set; }

            public Options()
            {
                this.RoomName = string.Empty;
                this.RoomPassword = null;
                this.Advertise = true;
            }
        }
    }

    public abstract class MessageRoom : MonoBehaviour
    {
        public enum State
        {
            Waiting,
            Searching,
            Joining,
            Creating,
            
            ConnectedServer,
            ConnectedClient,

            ReconnectingToOriginalRoom,  // happens when a client disconnects

            SearchingForFallback,
            JoiningFallback,
            CreatingFallback,  // what happens if the fallback fails?
            
            ShuttingDown,
        }

        private HashSet<short> relayToAllClientsMessageIds = new HashSet<short>();
        private HashSet<short> unreliableMessageIds = new HashSet<short>();
        private ConnectionConfig connectionConfig;
        private NetworkClient client;
        private SimpleServer server;
        //private RoomInfo roomInfo;
        private State state;

        public abstract IEnumerable<RegisterMessage> Messages { get; }


        private void SetState(State newState)
        {
            if (this.state == newState)
            {
                Debug.LogErrorFormat("MessageRoom try to go into state {0} while already in that state.", newState);
                return;
            }

            this.state = newState;

            switch (this.state)
            {
                case State.Waiting:
                case State.Searching:
                case State.Joining:
                case State.Creating:
                case State.ConnectedServer:
                case State.ConnectedClient:
                case State.ShuttingDown:
                default:
                    break;
            }
        }

        private void Update()
        {
            switch (this.state)
            {
                case State.Waiting:
                case State.Searching:
                case State.Joining:
                case State.Creating:
                    break;

                case State.ConnectedServer:
                    this.server.Update();
                    break;

                case State.ConnectedClient:
                    break;
                
                case State.ShuttingDown:

                    if (this.client != null)
                    {
                        this.client.Shutdown();
                        this.client = null;
                    }

                    if (this.server != null)
                    {
                        // TODO: unregister events?
                        this.server.Stop();
                        this.server = null;
                    }

                    break;

                default:
                    break;
            }
        }

        public void CreateOrJoinRoom(RoomInfo roomInfo)
        {
            // this.roomInfo = roomInfo;
        }

        protected virtual void Awake()
        {
            foreach (var message in this.Messages)
            {
                if (message.RelayToAllClients)
                {
                    this.relayToAllClientsMessageIds.Add(message.MessageId);
                }

                if (message.Reliable == false)
                {
                    this.unreliableMessageIds.Add(message.MessageId);
                }
            }
        }

        protected virtual ConnectionConfig CreateConnectionConfig()
        {
            var config = new ConnectionConfig();
            config.AddChannel(QosType.Reliable);
            config.AddChannel(QosType.Unreliable);

            return config;
        }

        private void OnConnected()
        {
             if (this.connectionConfig == null)
             {
                this.connectionConfig = this.CreateConnectionConfig();
             }

            // If Client

            // -------------- Client -----------------
            if (this.client != null)
            {
                // this.client = new NetworkClient();
                // this.client.Configure(this.connectionConfig);

                foreach (var message in this.Messages)
                {
                    this.client.RegisterHandler(message.MessageId, message.Delegate);
                }

                // this.client.Connect(the MatchInfo result of the join room call);
            }
            
            // ---------------- Server ----------------
            // this.matches.Add(createMatch.Value);
            // 
            // if (this.server == null)
            // {
            //     this.server = new SimpleServer();
            //     this.server.OnConnectedEvent += Server_OnConnectedEvent;
            //     this.server.OnConnectErrorEvent += Server_OnConnectErrorEvent;
            //     this.server.OnDataErrorEvent += Server_OnDataErrorEvent;
            //     this.server.OnDisconnectErrorEvent += Server_OnDisconnectErrorEvent;
            //     this.server.OnDisconnectedEvent += Server_OnDisconnectedEvent;
            // }
            // 
            // this.server.Configure(config, int.Parse(createMatchSize.text));
            // this.server.RegisterHandler((short)NetworkTestType.Text, this.OnTextMessageReceived);
            // this.server.ListenMatchInfo(the MatchInfo result from the create room call);

        }

        #region Simple Server Callbacks

        private void Server_OnDisconnectErrorEvent(UnityEngine.Networking.NetworkConnection conn, byte error)
        {
            Debug.LogError("Server_OnDisconnectErrorEvent");
        }

        private void Server_OnDataErrorEvent(UnityEngine.Networking.NetworkConnection conn, byte error)
        {
            Debug.LogError("Server_OnDataErrorEvent");
        }

        private void Server_OnConnectErrorEvent(int connectionId, byte error)
        {
            Debug.LogError("Server_OnConnectErrorEvent");
        }

        private void Server_OnConnectedEvent(UnityEngine.Networking.NetworkConnection conn)
        {
            Debug.Log("Server_OnConnectedEvent");
        }

        private void Server_OnDisconnectedEvent(NetworkConnection conn)
        {
            Debug.Log("Server_OnDisconnectedEvent");
        }

        #endregion
        
        //private SimpleServer server;
        //private NetworkClient client;

        // public enum MessageType
        // {
        //     MessageType1 = 1001,
        // }
        // 
        // public class MessageType1 : NetworkMessage
        // {
        // 
        // }
        // 
        // public SimpleServer()
        // {
        //     this.Listen("127.0.0.1", 4567);  // or this.ListenRelay(...)
        //     this.RegisterHandler(MessageType.MessageType1, this.OnReceivedMessageType1);
        // }
        // 
        // private void OnReceivedMessageType1(NetworkMessage networkMessage)
        // {
        //     var messageType1 = networkMessage.ReadMessage<MessageType1>();
        //     Debug.Log(messageType1.ToString());
        // }

        // This gets called when a client disconnects
        // If we have a valid connection here drop the client in the matchmaker before shutting down below
        // if (matchMaker != null && matchInfo != null && matchInfo.networkId != NetworkID.Invalid && matchInfo.nodeId != NodeID.Invalid)
        // {
        //     matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, matchInfo.domain, OnDropConnection);
        // }

        // ABCDEFGHIJKLMNPQRSTUVWXYZ123456789  - 35 characters

        //void OnServerReadyToBeginMessage(NetworkMessage netMsg)
        //{
        //    var beginMessage = netMsg.ReadMessage<IntegerMessage>();
        //    Debug.Log("received OnServerReadyToBeginMessage " + beginMessage.value);
        //}

        private void Start()
        {
            NetworkManager.singleton.StartMatchMaker();
        }
        
        //call this method to request a match to be created on the server
        public void CreateInternetMatch(string matchName)
        {
            NetworkManager.singleton.matchMaker.CreateMatch(matchName, 4, true, "", "", "", 0, 0, OnInternetMatchCreate);
        }

        // this method is called when your request for creating a match is returned
        private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                //Debug.Log("Create match succeeded");

                MatchInfo hostInfo = matchInfo;
                NetworkServer.Listen(hostInfo, 9000);

                NetworkManager.singleton.StartHost(hostInfo);
            }
            else
            {
                Debug.LogError("Create match failed");
            }
        }

        // call this method to find a match through the matchmaker
        public void FindInternetMatch(string matchName)
        {
            NetworkManager.singleton.matchMaker.ListMatches(0, 10, matchName, true, 0, 0, OnInternetMatchList);
        }

        // this method is called when a list of matches is returned
        private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            if (success)
            {
                if (matches.Count != 0)
                {
                    //Debug.Log("A list of matches was returned");

                    //join the last server (just in case there are two...)
                    NetworkManager.singleton.matchMaker.JoinMatch(matches[matches.Count - 1].networkId, "", "", "", 0, 0, OnJoinInternetMatch);
                }
                else
                {
                    Debug.Log("No matches in requested room!");
                }
            }
            else
            {
                Debug.LogError("Couldn't connect to match maker");
            }
        }

        // this method is called when your request to join a match is returned
        private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                //Debug.Log("Able to join a match");

                MatchInfo hostInfo = matchInfo;
                NetworkManager.singleton.StartClient(hostInfo);
            }
            else
            {
                Debug.LogError("Join match failed");
            }
        }
    }
}

//public static void CreateOrJoinRoom(string matchName, int eloScore, int playerCount)
//{
    // if CreateMatch worked, then run this
    // MatchInfo hostInfo = matchInfo;
    // NetworkServer.Listen(hostInfo, 9000);
    // NetworkManager.singleton.StartHost(hostInfo);

    // if list match successful call 
    // NetworkManager.singleton.matchMaker.JoinMatch(matches[matches.Count - 1].networkId, "", "", "", 0, 0, OnJoinInternetMatch);

    // if join match success, call
    // MatchInfo hostInfo = matchInfo;
    // NetworkManager.singleton.StartClient(hostInfo);

    // SearchingForMatches
    //   * Try Listing Matches
    //     * If None Found, Go To CreatingMatch 
    //     * If Found, Go To JoiningMatch
    // 
    // CreatingMatch
    //   * Try Creating Match
    //     * FAIL: Go To SearchingForMatches
    //     * PASS: Create SimpleServer, Go To ServerWaitingForPlayers
    //
    // JoiningMatch 
    //   * Try To Connect To MatchInfo
    //     * FAIL: Go To SearchingForMatches
    //     * PASS: ClientConnectToServer
    //
    // ClientConnectToServer
    //   * Try Connecting To Server MatchInfo (Creates NetworkClient?)
    //     * FAIL: Go To SearchingForMatches
    //     * PASS: Go To ClientWaitingForPlayers
    // 
    // ServerWaitingForPlayers
    //   * Waiting for all players to join
    //   * When New Player connects send reconnect info
    //   * If player sends back Ready, then mark them ready
    //   * When you get Ready from all players, then go to ServerReady
    //     * Close down matchmaker?
    // 
    // ClientWaitingForPlayers
    //   * 
    // 
    // ClientConnectToServer
    // ClientSendPlayerInfo
    // ClientWaitingForReconnectInfo
    // ClientReady
    // 
    //
    // 
    // 
    //
    // StopWaitingForPlayers()
    //
    // Server Has Dictionary<Connect, PlayerInfo>
    // 	PlayerInfo { Connect, PlayerId, HasSentReconnect, HasReceivedReady, IsServer }
    // Events
    //   PlayerConnected
    //   PlayerDropped (if server, then start reconnect)
//}

// should you ever call "NetworkMatch.DestroyMatch"???
// 
// public static void Leave()
// {
//     // NetworkMatch.DropConnection
// }
// 
//
// State (Searching, Join, ConnectingAsClient, Connected)
//
// When first Initializing the user gives params (RoomName, Elo, Stage, Etc)
//   When everyone is connected, create reconnect params, and if lose connection
//   then use the reconnect info.
//
// List Matches
// If don't find one, then go into server mode CreateMatch, if successful, make NetworkServerSimple
// Else, go to JoinMatch, if successful, make NetworkClient
// Register messages
// List Players;
// event PlayersReady  (before sending this, make sure the server sends reconnect information)
//
//


//// --------------------------------------------------------------------------------
//// # Message Room
//// --------------------------------------------------------------------------------
//// 
//// #### Lan
//// ---------------------
////   Host
////     NetworkDiscovery Server Port: 4001  // sends out { IP, Room Name, Room GUID, Depth, ConnectionCount }  (Depth 0 = Host, Depth 1 = 1st Slave)
////     NetworkServer Listen Port: 4000
////     
////   Client
////     Find Rooms
////       Sorts by depth, then by connection count, as long as there are less than 10, it connects
//// 
//// 
////     Starts Up NetworkDiscovery Server on port localhost: 4001  // sends out { IP, Room Name, Room GUID, Depth, ConnectionCount }  Only does this once connected to a 
////     Starts A NetworkServerSimple(Client) on HOST_IP:4001
////     Starts NetworkServerSimple(Server) Port localhost:4000    // any connects will be 
////     
////     When receive message from server, send along to all clients
//// 
////  - FindMatch(if non, then StartMatch RoomName = Guid.New();)
////  
////  - Once server gets all 4 people in a room, creates a secret and sends it to all players, then tells card server to create a game with RoomName and Secret and all the player ids
////  
////  - Server then sends out to players the ready command and they all get the game state from the card server
////    * Card server uses playfab to authenticate before give the player their cards?
////    
////  - On Server Disconnect, hit up MatchMaker with RoomName and Secret and wait for all clients to join to resume
////    * If room name already exists, then you're no longer the server, try to join
////    
////  - On Client Disconnect, try to hit up MatchMaker with previous connection info, if that won't connect, try to make the room again with password, if it already exists, then connect to that room
////  
////  - BIG QUESITONS: When does the matchmaking room stop working? Automatically shuts down when full?
//// 
////     public class MyNetworkClient : NetworkClient
//// {
////     public MyNetworkClient()
////     {
////         // Connect(MatchInfo matchInfo) or Connect(string serverIp, int serverPort)
//// 
////     }
//// 
////     public override void Disconnect()
////     {
////         base.Disconnect();
////     }
//// 
////     // Send(short msgType, MessageBase msg)
////     // SendUnreliable(short msgType, MessageBase msg)
////     // RegisterHandler(short msgType, NetworkMessageDelegate handler)
////     // UnregisterHandler(short msgType)
////     // public bool isConnected { get { return m_AsyncConnect == ConnectState.Connected; }}
//// 
////     // I have zero clue how you know when the connection finally succeds
////     // Overloading update will be hard too since it's internal "internal virtual void Update()"
//// }
//// 
//// 
//// // or instead of inheriting, just use it like the NetworkManager does
//// /*NetworkClient client = new NetworkClient();
//// client.Configure(m_ConnectionConfig, m_MaxConnections);  // not sure how config get set!?!?
////     client.Connect(...);
////     client.RegisterHandler(MsgType.Connect, OnClientConnectInternal);
////     client.RegisterHandler(MsgType.Disconnect, OnClientDisconnectInternal);
////     client.RegisterHandler(MsgType.NotReady, OnClientNotReadyMessageInternal);
////     client.RegisterHandler(MsgType.Error, OnClientErrorInternal);
////     client.RegisterHandler(MsgType.Scene, OnClientSceneInternal);
////     // register all other handlers you like
////  
////     client.Disconnect();
////     client.Shutdown();
//// 
////     public class MessageType
//// {
////     public const short MessageType1 = 100;
//// }
//// 
