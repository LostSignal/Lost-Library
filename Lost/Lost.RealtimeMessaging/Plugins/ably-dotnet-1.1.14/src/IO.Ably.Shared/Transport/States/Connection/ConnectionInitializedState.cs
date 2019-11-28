﻿using System.Threading.Tasks;
using IO.Ably;
using IO.Ably.Types;

namespace IO.Ably.Transport.States.Connection
{
    using IO.Ably.Realtime;

    internal class ConnectionInitializedState : ConnectionStateBase
    {
        public ConnectionInitializedState(IConnectionContext context, ILogger logger)
            : base(context, logger)
        { }

        public override bool CanQueue => true;

        public override ConnectionState State => Realtime.ConnectionState.Initialized;

        public override void Connect()
        {
            Context.SetState(new ConnectionConnectingState(Context, Logger));
        }

        public override void AbortTimer()
        {
        }
    }
}
