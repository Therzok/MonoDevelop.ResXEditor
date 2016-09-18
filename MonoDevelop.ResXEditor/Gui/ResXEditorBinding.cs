using System;
using Mono.Addins;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;

namespace MonoDevelop.ResXEditor
{
	public abstract class ResXEditorBinding 
	{
		public abstract ResXEditorViewContent CreateViewContent(ResXData data);
	}

	class ResXStringDisplayBinding : ResXEditorBinding
	{
		public override ResXEditorViewContent CreateViewContent(ResXData data)
		{
			return new ResXEditorStringsViewContent(data);
		}
	}

	class ResXImageDisplayBinding : ResXEditorBinding
	{
		public override ResXEditorViewContent CreateViewContent(ResXData data)
		{
			return new ResXEditorImageViewContent(data);
		}
	}
}
