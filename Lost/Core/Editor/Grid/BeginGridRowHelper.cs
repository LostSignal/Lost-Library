//-----------------------------------------------------------------------
// <copyright file="BeginGridRowHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;

    public class BeginGridRowHelper : IDisposable
    {
        private Grid grid;

        public BeginGridRowHelper(Grid grid)
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
