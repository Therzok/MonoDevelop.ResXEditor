using System;
using System.Resources;
using System.Windows.Forms;

namespace MonoDevelop.ResXEditor
{
	public class ResXNode
	{
		public ResXNode(string name, object value, string comment, string typeName)
		{
			Name = name;
			Value = value;
			Comment = comment;
			TypeName = typeName;
		}

		public string Name { get; set; }
		public object Value { get; set; }
		public string Comment { get; set; }
		public string TypeName { get; set; }

		public static implicit operator ResXDataNode(ResXNode node)
		{
			return new ResXDataNode(node.Name, node.Value)
			{
				Comment = node.Comment,
			};
		}

		public static implicit operator ResXNode(ResXDataNode node)
		{
			var resolution = default(System.ComponentModel.Design.ITypeResolutionService);
			var typeName = node.GetValueTypeName(resolution);
			var value = node.GetValue(resolution);
			return new ResXNode(node.Name, value, node.Comment, typeName);
		}
	}
}
