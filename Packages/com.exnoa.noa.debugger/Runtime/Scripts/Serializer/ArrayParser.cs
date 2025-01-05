using System.Text;

namespace NoaDebugger
{
    sealed class ArrayParser: IKeyValueParser
    {
        string _key;
        string[] _values;

        public ArrayParser(string key, string[] values)
        {
            _key = key;
            _values = values;
        }
        
        public string ToJsonString()
        {
            var builder = new StringBuilder();

            string[] values = new string[_values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = $"\"{_values[i]?.Replace("\"", "\\\"")}\"";
            }
        
            builder = KeyValueSerializer.AppendAndJoinJson(builder, values);
            builder = KeyValueSerializer.InsertTabs(builder, $"\"{_key}\":[", "]");
            return builder.ToString();
        }
    }
}
