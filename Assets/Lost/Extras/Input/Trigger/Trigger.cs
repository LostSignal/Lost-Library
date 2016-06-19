//-----------------------------------------------------------------------
// <copyright file="Trigger.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class Trigger
    {
        public bool IsFired { get; private set; }

        public void Fire()
        {
            this.IsFired = true;
        }

        public void Reset()
        {
            this.IsFired = false;
        }
    }
}
