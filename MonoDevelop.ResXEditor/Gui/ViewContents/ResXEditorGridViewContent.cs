using System;
using MonoDevelop.Components;

namespace MonoDevelop.ResXEditor
{
    public abstract class ResXEditorGridViewContent : ResXEditorViewContent
    {
        protected override void OnInitialize(ResXData data)
        {
        }

        protected override bool HasToolbar => true;
    }
}
