using System;
using System.Linq;

namespace MonoDevelop.ResXEditor
{
	class ResXEditorStringsViewContent : ResXEditorListViewContent
	{
		public ResXEditorStringsViewContent(ResXData data) : base(data)
		{
			treeView.ShowAll ();
		}

		protected override bool SkipNode(ResXNode node)
		{
			return !(node.Value is string);
		}

		public override string TabPageLabel
		{
			get
			{
				return "Strings";
			}
		}
	}
}
