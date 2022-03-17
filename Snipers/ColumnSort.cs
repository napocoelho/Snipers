using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snipers
{
    public class ColumnSort
    {
        public SortDirection Direction { get; set; }
        public string ColumnName { get; set; }
        public bool IsDefined { get { return ColumnName != string.Empty; } }


        public ColumnSort(string columnName = null)
        {
            Direction = SortDirection.Ascendant;
            ColumnName = (columnName == null ? string.Empty : columnName);
        }
    }
}
