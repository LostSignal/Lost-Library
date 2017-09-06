//-----------------------------------------------------------------------
// <copyright file="CloudScriptException.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USE_PLAYFAB_SDK

namespace Lost
{
    using System;

    public class CloudScriptException : Exception
    {
        public CloudScriptException(string cloudScriptError)
        {
            this.CloudScriptError = cloudScriptError;
        }

        public string CloudScriptError { get; private set; }
    }
}

#endif
