using System;
using System.Linq;

namespace MonoDevelop.ResXEditor
{
	class ResXEditorStringsViewContent : ResXEditorListViewContent
	{
        protected override bool SkipNode(ResXNode node) => !(node.TypeName == "System.String");
        protected override ResXNode GetPlaceholder() => new ResXNode(string.Empty, string.Empty, null, null);
        public override string TabPageLabel => "Strings";
	}
}
