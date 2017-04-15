using System;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.ResXEditor
{
    public abstract class ResXEditorViewContent : BaseViewContent
    {
        CompactScrolledWindow sw;
        protected ResXEditorViewContent()
        {
        }

        protected ResXData Data { get; private set; }

        internal ResXEditorViewContent Initialize(ResXData data)
		{
            Data = data;
            OnInitialize(data);
            return this;
        }

        protected abstract void OnInitialize(ResXData data);
        protected abstract Xwt.Widget CreateContent();

		public override sealed Control Control
		{
			get
			{
				if (sw == null)
				{
					sw = new CompactScrolledWindow();
                    sw.Add(CreateContent().ToGtkWidget());
					sw.ShowAll();
				}
				return sw;
			}
		}
	}
}
