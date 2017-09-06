//-----------------------------------------------------------------------
// <copyright file="GridDefinition.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;

    [Flags]
    public enum GridButton
    {
        None = 0,
        Delete = 1,
        MoveUp = 2,
        MoveDown = 4,
        All = 7
    }

    public class GridDefinition
    {
        private List<Column> columns = new List<Column>();

        public GridDefinition()
        {
            this.DrawHeader = true;
            this.AlternateColors = false;
            this.RowButtons = GridButton.None;
        }

        public int ColumnCount
        {
            get { return this.columns.Count; }
        }

        public bool DrawHeader { get; set; }

        public bool AlternateColors { get; set; }
        
        public GridButton RowButtons { get; set; }

        public int RowButtonCount
        {
            get
            {
                return ((this.RowButtons & GridButton.Delete) != 0 ? 1 : 0) +
                       ((this.RowButtons & GridButton.MoveDown) != 0 ? 1 : 0) +
                       ((this.RowButtons & GridButton.MoveUp) != 0 ? 1 : 0);
            }
        }

        public Column this[int index]
        {
            get { return this.columns[index]; }
        }
                
        public void AddColumn(string name, int width, string tooltip = null)
        {
            this.columns.Add(new Column(name, width, tooltip));
        }

        public class Column
        {
            internal Column(string name, int width, string tooltip)
            {
                this.Name = name;
                this.Width = width;
                this.Tooltip = tooltip;
            }
            
            public string Name { get; private set; }

            public int Width { get; set; }

            public string Tooltip { get; private set; }
        }
    }
}
