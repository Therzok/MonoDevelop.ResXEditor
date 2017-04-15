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
        protected DocumentToolbar Toolbar { get; private set; }

        internal ResXEditorViewContent Initialize(ResXData data)
		{
            Data = data;
            OnInitialize(data);
            return this;
        }

        protected abstract void OnInitialize(ResXData data);

        protected override void OnWorkbenchWindowChanged()
        {
            base.OnWorkbenchWindowChanged();

            if (WorkbenchWindow != null)
            {
                Toolbar = WorkbenchWindow.GetToolbar(this);
            }
        }

        protected abstract Xwt.Widget CreateContent();

		public override sealed Control Control
		{
			get
			{
				if (sw == null)
				{
					sw = new CompactScrolledWindow();
                    sw.AddWithViewport(CreateContent().ToGtkWidget());
					sw.ShowAll();
				}
				return sw;
			}
		}
	}
}
