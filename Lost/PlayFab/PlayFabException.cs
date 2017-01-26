//-----------------------------------------------------------------------
// <copyright file="PlayFabException.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USE_PLAYFAB_SDK

namespace Lost
{
    using System;
    using PlayFab;

    public class PlayFabException : Exception
    {
        public PlayFabException(PlayFabError error)
        {
            this.Error = error;
        }

        public PlayFabError Error { get; private set; }
    }
}

#endif
