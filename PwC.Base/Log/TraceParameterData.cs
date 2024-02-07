namespace PwC.Base.Log
{
    public class TraceParameterData
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public TraceParameterData(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            string nullName = "Null";
            return $"{Name}: {Value ?? nullName}";
        }
    }
}