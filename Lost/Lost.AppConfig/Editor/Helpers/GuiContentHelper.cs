//-----------------------------------------------------------------------
// <copyright file="GuiContentHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System;
    using UnityEngine;

    public class GuiContentHelper : IDisposable
    {
        private Color oldColor;

        public GuiContentHelper(Color newColor)
        {
            this.oldColor = GUI.contentColor;
            GUI.contentColor = newColor;
        }

        public void Dispose()
        {
            GUI.contentColor = oldColor;
        }
    }
}
