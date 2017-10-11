//-----------------------------------------------------------------------
// <copyright file="MatchMakingException.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class MatchMakingException : System.Exception
    {
        public string ExtendedInfoResult { get; private set; }

        public MatchMakingException(string extendedInfoResult) : base(extendedInfoResult)
        {
            this.ExtendedInfoResult = extendedInfoResult;
        }
    }
}
