//-----------------------------------------------------------------------
// <copyright file="BeginGridHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System;

    public class BeginGridHelper : IDisposable
    {
        private EditorGrid grid;

        public BeginGridHelper(EditorGrid grid)
        {
            this.grid = grid;
            this.grid.BeginGrid();
        }

        public void Dispose()
        {
            this.grid.EndGrid();
        }
    }
}
