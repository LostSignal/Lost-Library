//-----------------------------------------------------------------------
// <copyright file="DialogStatus.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class DialogStatus
    {
        public enum State
        {
            Waiting,
            Running,
            Done
        }

        public Dialog DialogPrefab { get; set; }

        public Dialog DialogInstance { get; set; }

        public State CurrentState { get; set; }
    }
}
