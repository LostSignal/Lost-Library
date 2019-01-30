//-----------------------------------------------------------------------
// <copyright file="BeginGridRowScope.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.EditorGrid
{
    using System;

    public class BeginGridRowScope : IDisposable
    {
        private EditorGrid grid;

        public BeginGridRowScope(EditorGrid grid)
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
