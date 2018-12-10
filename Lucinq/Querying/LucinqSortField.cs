using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucinq.Core.Querying
{
    public class LucinqSortField
    {
        public string FieldName { get; }

        public int SortType { get; }

        public bool SortDescending { get; }

        public LucinqSortField(string fieldName, int sortType, bool sortDescending)
        {
            this.FieldName = fieldName;
            this.SortType = sortType;
            this.SortDescending = sortDescending;
        }
    }
}
