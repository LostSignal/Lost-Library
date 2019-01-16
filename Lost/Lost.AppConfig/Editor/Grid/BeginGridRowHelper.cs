//-----------------------------------------------------------------------
// <copyright file="BeginGridRowHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System;

    public class BeginGridRowHelper : IDisposable
    {
        private EditorGrid grid;

        public BeginGridRowHelper(EditorGrid grid)
        {
            this.grid = grid;
            this.grid.BeginRow();
        }

        public void Dispose()
        {
            this.grid.EndRow();
        }
    }
}
