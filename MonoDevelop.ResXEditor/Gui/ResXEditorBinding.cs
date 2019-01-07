using System;
using System.Collections.Generic;

namespace MonoDevelop.ResXEditor
{
    public abstract class ResXEditorBinding
    {
        protected internal abstract IEnumerable<Type> TypesHandled { get; }
        protected internal abstract ResXEditorViewContent CreateViewContent(List<ResXData> data, ResXData mainResx);
    }

    public abstract class ResXEditorBinding<T> : ResXEditorBinding where T:ResXEditorViewContent,new()
    {
        protected internal sealed override ResXEditorViewContent CreateViewContent(List<ResXData> data, ResXData mainResx) => new T().Initialize(data, mainResx);
    }

    class ResXStringDisplayBinding : ResXEditorBinding<ResXEditorStringsViewContent>
    {
        protected internal override IEnumerable<Type> TypesHandled => new[] { typeof(string) };
    }

    class ResXImageDisplayBinding : ResXEditorBinding<ResXEditorImageViewContent>
    {
        protected internal override IEnumerable<Type> TypesHandled => new[] { typeof(System.Drawing.Bitmap), typeof(System.Drawing.Icon) };
    }

    class ResXOtherDisplayBinding : ResXEditorBinding<ResXEditorOtherViewContent>
    {
        protected internal override IEnumerable<Type> TypesHandled => Array.Empty<Type>();
    }
}
