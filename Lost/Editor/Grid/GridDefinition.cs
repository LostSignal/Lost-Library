//-----------------------------------------------------------------------
// <copyright file="GridDefinition.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;

    public class GridDefinition
    {
        private List<Column> columns = new List<Column>();
        
        public GridDefinition()
        {
            this.DrawHeader = true;
        }

        public int ColumnCount
        {
            get { return this.columns.Count; }
        }

        public bool DrawHeader { get; set; }

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

            public int Width { get; private set; }

            public string Tooltip { get; private set; }
        }
    }
}
