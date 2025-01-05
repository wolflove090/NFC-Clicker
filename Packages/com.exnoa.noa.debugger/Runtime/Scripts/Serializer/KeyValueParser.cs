using System.Text;

namespace NoaDebugger
{
    sealed class KeyValueParser: IKeyValueParser
    {
        string _key;
        string _value;

        public KeyValueParser(string key, string value)
        {
            _key = key;
            _value = value;
        }

        public string ToJsonString()
        {
            var builder = new StringBuilder();
            builder.Append($"\"{_key}\":\"{_value?.Replace("\"", "\\\"")}\"");
            return builder.ToString();
        }
    }
}
