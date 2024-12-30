using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NoaDebugger
{
    interface IKeyValueParser
    {
        string ToJsonString();
    }

    sealed class KeyValueSerializer
    {
        string _key;
        IKeyValueParser[] _parsers;

        public KeyValueSerializer(string key, IKeyValueParser[] parsers)
        {
            _key = key;
            _parsers = parsers;
        }

        string SerializeToJson()
        {
            var builder = new StringBuilder();

            string[] appendStrings = new string[_parsers.Length];
            for (int i = 0; i < appendStrings.Length; i++)
            {
                appendStrings[i] = _parsers[i].ToJsonString();
            }
            builder = KeyValueSerializer.AppendAndJoinJson(builder, appendStrings);

            builder = KeyValueSerializer.InsertTabs(builder, $"\"{_key}\":{{", "}");

            return builder.ToString();
        }

        public static string SerializeToJson(KeyValueSerializer[] targetDatas)
        {
            var builder = new StringBuilder();

            string[] appendStrings = new string[targetDatas.Length];
            for(int i = 0; i < targetDatas.Length; i++)
            {
                string dataJson = targetDatas[i].SerializeToJson();
                appendStrings[i] = dataJson;
            }
            builder = KeyValueSerializer.AppendAndJoinJson(builder, appendStrings);

            builder = KeyValueSerializer.InsertTabs(builder, "{", "}");

            return builder.ToString();
        }

        public static StringBuilder InsertTabs(StringBuilder builder, string prefix = "", string suffix = "")
        {
            var lines = builder.ToString().Split(Environment.NewLine);
            builder.Clear();
            foreach (var line in lines)
            {
                builder.AppendLine($"\t{line}");
            }

            if (!string.IsNullOrEmpty(prefix))
            {
                builder.Insert(0, Environment.NewLine);
                builder.Insert(0, prefix);
            }

            if (!string.IsNullOrEmpty(suffix))
            {
                builder.Append(suffix);
            }

            return builder;
        }

        public static StringBuilder AppendAndJoinJson(StringBuilder builder, string[] targetStrings)
        {
            return builder.Append(String.Join($",{Environment.NewLine}", targetStrings));
        }

        public static KeyValueSerializer CreateSubData(string label, IKeyValueParser[] addValues = null)
        {
            var subDataValues = new List<IKeyValueParser>();

            var labelData = new KeyValueParser("_label", label);
            subDataValues.Add(labelData);
            if (addValues != null)
            {
                subDataValues.AddRange(addValues);
            }

            return new KeyValueSerializer("SubData", subDataValues.ToArray());
        }

        public static KeyValueSerializer CreateFromClass<T>(T viewInfo, string name) where T : class
        {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            KeyValueParser[] parsers = new KeyValueParser[fields.Length];

            for(int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var value = field.GetValue(viewInfo);
                parsers[i] = new KeyValueParser(field.Name, value.ToString());
            }

            return new KeyValueSerializer(name, parsers);
        }
    }
}
