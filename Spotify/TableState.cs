using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Spotify
{
    internal class TableState : IDisposable
    {
        public DataTable Table
        {
            get;
            private set;
        }

        public string Display
        {
            get;
            private set;
        }

        public GroupingType GroupingType
        {
            get;
            private set;
        }

        public TableState(string display, DataTable table, GroupingType groupingType)
        {
            this.Table = table;
            this.Display = display;
            this.GroupingType = groupingType;
            this.StateGUID = Guid.NewGuid();
        }

        public void Dispose()
        {
            if (Table != null)
            {
                Utility.DisposeTableDisposables(Table);
                Table.Dispose();
            }
        }

        public int Position
        {
            get;
            set;
        }

        public Guid StateGUID
        {
            get;
            private set;
        }

        public string ImageID
        {
            get;
            set;
        }
    }
}
