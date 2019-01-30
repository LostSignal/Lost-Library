//-----------------------------------------------------------------------
// <copyright file="AppVersionRequest.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    [System.Serializable]
    public class AppVersionRequest
    {
        public string CurrentVersion { get; set; }
        public string LastLoginVersion { get; set; }
        public string Language { get; set; }
        public bool IsNewVersionForUser { get; set; }
    }
}
