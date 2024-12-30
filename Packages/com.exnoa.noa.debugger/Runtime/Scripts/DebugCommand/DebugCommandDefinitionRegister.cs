using System.Collections.Generic;

namespace NoaDebugger.DebugCommand
{
    class DebugCommandDefinitionRegister
    {
        public Dictionary<string, HashSet<CommandDefinition>> CommandDefinitions { get; } = new();

        public void Dispose()
        {
            CommandDefinitions.Clear();
        }

        public bool Add(CommandDefinition commandDefinition)
        {
            string categoryName = commandDefinition.CategoryName;

            if (!CommandDefinitions.ContainsKey(categoryName))
            {
                CommandDefinitions[categoryName] = new HashSet<CommandDefinition>();
            }

            return CommandDefinitions[categoryName].Add(commandDefinition);
        }

        public bool Remove(CommandDefinition commandDefinition)
        {
            string categoryName = commandDefinition.CategoryName;

            if (!CommandDefinitions.ContainsKey(categoryName)
                || !CommandDefinitions[categoryName].Remove(commandDefinition))
            {
                return false;
            }

            if (CommandDefinitions[categoryName].Count == 0)
            {
                CommandDefinitions.Remove(categoryName);
            }
            return true;
        }

        public bool RemoveCategory(string categoryName)
        {
            return CommandDefinitions.Remove(categoryName);
        }
    }
}
