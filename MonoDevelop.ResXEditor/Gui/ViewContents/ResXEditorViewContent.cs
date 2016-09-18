using System;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.ResXEditor
{
	public abstract class ResXEditorViewContent : BaseViewContent
	{
		protected ResXEditorViewContent(ResXData data)
		{
			Data = data;
		}

		public ResXData Data
		{
			get;
			private set;
		}
	}
}
