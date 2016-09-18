using System;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.ResXEditor
{
	public abstract class ResXEditorViewContent : BaseViewContent
	{
		CompactScrolledWindow sw;
		protected ResXEditorViewContent(ResXData data)
		{
			Data = data;
		}

		public ResXData Data
		{
			get;
			private set;
		}

		protected abstract Control CreateContent();

		public override sealed Control Control
		{
			get
			{
				if (sw == null)
				{
					sw = new CompactScrolledWindow();
					sw.Add(CreateContent());
					sw.ShowAll();
				}
				return sw;
			}
		}
	}
}
