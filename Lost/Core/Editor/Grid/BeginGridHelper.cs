//-----------------------------------------------------------------------
// <copyright file="BeginGridHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;

    public class BeginGridHelper : IDisposable
    {
        private Grid grid;

        public BeginGridHelper(Grid grid)
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
