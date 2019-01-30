//-----------------------------------------------------------------------
// <copyright file="BeginGridScope.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.EditorGrid
{
    using System;

    public class BeginGridScope : IDisposable
    {
        private EditorGrid grid;

        public BeginGridScope(EditorGrid grid)
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
