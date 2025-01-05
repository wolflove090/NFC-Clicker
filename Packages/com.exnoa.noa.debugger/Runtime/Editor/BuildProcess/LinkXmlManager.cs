#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;

namespace NoaDebugger.DebugCommand
{
    sealed class LinkXmlManager
    {
        static readonly string LinkXmlOutputDir = $"{EditorDefine.SettingsDataPath}";
        static readonly string LinkXmlPath = $"{LinkXmlManager.LinkXmlOutputDir}/link.xml";

        static readonly string[] ExcludedAssemblyNames = new[] {"Noa.NoaDebugger.Runtime", "Noa.NoaDebugger.Editor", "Noa.NoaDebugger.RuntimeTest", "Noa.NoaDebugger.EditorTests"}; 
        
        
        public static void Export()
        {
            var instance = new LinkXmlManager();
            instance._Export();
        }
        
        public static void Remove()
        {
            var instance = new LinkXmlManager();
            instance._RemoveLinkXml();
        }

        
        void _Export()
        {
            var targets = _GetTargetClasses();
            if (targets.Count <= 0)
            {
                return;
            }
            
            var sb = _CreateLinkXmlStringBuilder(targets);
            _WriteLinkXml(sb);
        }

        Dictionary<Assembly, List<Type>> _GetTargetClasses()
        {
            var result = new Dictionary<Assembly, List<Type>>();
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (ExcludedAssemblyNames.Contains(assembly.GetName().Name))
                {
                    continue;
                }
                
                foreach(var type in assembly.GetTypes())
                {
                    if (type.BaseType == typeof(DebugCategoryBase) || type.BaseType == typeof(NoaCustomMenuBase))
                    {
                        if (!result.ContainsKey(assembly))
                        {
                            result.Add(assembly, new List<Type>(){type});
                        }
                        else
                        {
                            result[assembly].Add(type);
                        }
                    }
                }
            }

            return result;
        }

        const string SOFT_TAB = "    "; 
        StringBuilder _CreateLinkXmlStringBuilder(Dictionary<Assembly, List<Type>> targets)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<linker>");
            
            foreach (var kv in targets)
            {
                string assemblyName = kv.Key.GetName().Name;
                sb.AppendLine($"{SOFT_TAB}<assembly fullname=\"{assemblyName}\">");
                foreach (var target in kv.Value)
                {
                    sb.AppendLine($"{SOFT_TAB}{SOFT_TAB}<type fullname=\"{target.FullName}\" preserve=\"all\"/>");
                }
                sb.AppendLine($"{SOFT_TAB}</assembly>");
            }
            
            sb.Append("</linker>");

            return sb;
        }

        void _WriteLinkXml(StringBuilder sb)
        {
            if (!Directory.Exists(LinkXmlManager.LinkXmlOutputDir))
            {
                Directory.CreateDirectory(LinkXmlManager.LinkXmlOutputDir);
            }
            File.WriteAllText(LinkXmlPath, sb.ToString());
            
            AssetDatabase.Refresh();
        }
        

        void _RemoveLinkXml()
        {
            if (!File.Exists(LinkXmlPath))
            {
                return;
            }
            
            File.Delete(LinkXmlPath);
            AssetDatabase.Refresh();
        }
    }
}
#endif
