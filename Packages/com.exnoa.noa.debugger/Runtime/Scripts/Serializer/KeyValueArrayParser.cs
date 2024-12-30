using System.Text;

namespace NoaDebugger
{
    sealed class KeyValueArrayParser : IKeyValueParser
    {
        string _key;
        ObjectParser[] _values;

        public KeyValueArrayParser(string key, ObjectParser[] parser)
        {
            _key = key;
            _values = parser;
        }

        public string ToJsonString()
        {
            var builder = new StringBuilder();
            
            string[] appendStrings = new string[_values.Length];
            for (int i = 0; i < appendStrings.Length; i++)
            {
                appendStrings[i] = _values[i].ToJsonString();
            }
            builder = KeyValueSerializer.AppendAndJoinJson(builder, appendStrings);

            builder = KeyValueSerializer.InsertTabs(builder, $"\"{_key}\":[", "]");
            
            return builder.ToString();
        }
        
        public sealed class ObjectParser
        {
            IKeyValueParser[] _values;
        
            public ObjectParser(IKeyValueParser[] values)
            {
                _values = values;
            }
        
            public string ToJsonString()
            {
                var builder = new StringBuilder();
            
                string[] appendStrings = new string[_values.Length];
                for (int i = 0; i < appendStrings.Length; i++)
                {
                    appendStrings[i] = _values[i].ToJsonString();
                }
                builder = KeyValueSerializer.AppendAndJoinJson(builder, appendStrings);

                builder = KeyValueSerializer.InsertTabs(builder, "{", "}");
            
                return builder.ToString();
            }
        }
    }    
}
