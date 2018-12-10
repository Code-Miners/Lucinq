namespace Lucinq.Core.Querying
{
    public class LucinqTerm
    {
        public string Field { get; }

        public string Value { get; }

        public LucinqTerm(string field, string value)
        {
            Field = field;
            Value = value;
        }
    }
}
