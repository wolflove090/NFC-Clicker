using System.Reflection;
using System.Text;

namespace NoaDebugger
{
    sealed class KeyObjectParser : IKeyValueParser
    {
        string _key;
        IKeyValueParser[] _elements;

        public KeyObjectParser(string key, IKeyValueParser[] elements)
        {
            _key = key;
            _elements = elements;
        }

        public string ToJsonString()
        {
            var builder = new StringBuilder();
            string[] appendStrings = new string[_elements.Length];
            for (int i = 0; i < appendStrings.Length; i++)
            {
                appendStrings[i] = _elements[i].ToJsonString();
            }
            builder = KeyValueSerializer.AppendAndJoinJson(builder, appendStrings);
            builder = KeyValueSerializer.InsertTabs(builder, $"\"{_key}\":{{", "}");
            return builder.ToString();
        }

        public static KeyObjectParser CreateFromClass<T>(T viewInfo, string name) where T : class
        {
            var type = typeof(T);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            IKeyValueParser[] parsers = new IKeyValueParser[fields.Length + properties.Length];

            int index = 0;
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(viewInfo);
                parsers[index] = new KeyValueParser(field.Name, value.ToString());
                index++;
            }
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(viewInfo);
                parsers[index] = new KeyValueParser(property.Name, value.ToString());
                index++;
            }

            return new KeyObjectParser(name, parsers);
        }
    }
}
