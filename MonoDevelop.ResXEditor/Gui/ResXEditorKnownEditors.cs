using System;
using System.Collections.Generic;

namespace MonoDevelop.ResXEditor
{
    public static class ResXEditorKnownEditors
    {
        static readonly Dictionary<string, int> knownTypes = new Dictionary<string, int>();
        public static bool IsKnownType(string t) => knownTypes.ContainsKey(t);

        internal static void RegisterKnownTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                knownTypes.TryGetValue(type.AssemblyQualifiedName, out int count);
                knownTypes[type.AssemblyQualifiedName] = ++count;
            }
        }

        internal static void UnregisterKnownTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (!knownTypes.TryGetValue(type.AssemblyQualifiedName, out int count))
                    continue;

                if (--count == 0)
                    knownTypes.Remove(type.AssemblyQualifiedName);
                else
                    knownTypes[type.AssemblyQualifiedName] = count;
            }
        }
    }
}
