//-----------------------------------------------------------------------
// <copyright file="GuiBackgroundHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    public class GuiBackgroundHelper : IDisposable
    {
        private Color oldColor;

        public GuiBackgroundHelper(Color newColor)
        {
            this.oldColor = GUI.backgroundColor;
            GUI.backgroundColor = newColor;
        }

        public void Dispose()
        {
            GUI.backgroundColor = oldColor;
        }
    }
}
