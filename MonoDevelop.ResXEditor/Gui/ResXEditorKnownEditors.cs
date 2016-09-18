using System;
using System.Collections.Generic;

namespace MonoDevelop.ResXEditor
{
	public static class ResXEditorKnownEditors
	{
		static readonly Dictionary<Type, int> knownTypes = new Dictionary<Type, int>();

		public static bool IsKnownType(Type t)
		{
			return knownTypes.ContainsKey(t);
		}

		internal static void RegisterKnownTypes(IEnumerable<Type> types)
		{
			foreach (var type in types)
			{
				int count;
				knownTypes.TryGetValue(type, out count);
				knownTypes[type] = count++;
			}
		}

		internal static void UnregisterKnownTypes(IEnumerable<Type> types)
		{
			foreach (var type in types)
			{
				int count;
				knownTypes.TryGetValue(type, out count);
				count--;
				if (count == 0)
					knownTypes.Remove(type);
				else
					knownTypes[type] = count;
			}
		}
	}
}
