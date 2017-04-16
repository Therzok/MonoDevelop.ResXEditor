using System;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.ResXEditor
{
    public abstract class ResXEditorViewContent : AbstractXwtViewContent
    {
        Xwt.ScrollView sw;
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

        protected virtual void OnToolbarSet()
        {
        }

        protected abstract void OnInitialize(ResXData data);
        protected abstract bool HasToolbar { get; }
        protected abstract Xwt.Widget CreateContent();

        protected override void OnWorkbenchWindowChanged()
        {
            base.OnWorkbenchWindowChanged();

            if (WorkbenchWindow != null && HasToolbar)
            {
                Toolbar = WorkbenchWindow.GetToolbar(this);
                OnToolbarSet();
            }
        }

        public override Xwt.Widget Widget
        {
            get
            {
                if (sw == null)
                {
                    sw = new Xwt.ScrollView(CreateContent());
                    sw.Show();
                }
                return sw;
            }
        }
    }
}
