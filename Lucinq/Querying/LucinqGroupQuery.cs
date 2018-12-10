using System.Collections.Generic;

namespace Lucinq.Core.Querying
{
    public abstract class LucinqGroupQuery : LucinqQuery
    {
        public IList<LucinqQuery> Queries { get; set; }

        protected LucinqGroupQuery()
        {
            Queries = new List<LucinqQuery>();
        }
    }
}
