//-----------------------------------------------------------------------
// <copyright file="CursorUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public static class CursorUtil
    {
        public static bool CursorLockedAndHidden
        {
            get
            {
                return Cursor.visible == false;
            }

            set
            {
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = value == false;
            }
        }
    }
}
