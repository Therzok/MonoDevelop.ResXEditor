using System;
using System.Collections.Generic;

namespace MonoDevelop.ResXEditor
{
    public abstract class ResXEditorBinding
    {
        public abstract IEnumerable<Type> TypesHandled { get; }
        public abstract ResXEditorViewContent CreateViewContent(ResXData data);
    }

    public abstract class ResXEditorBinding<T> : ResXEditorBinding where T:ResXEditorViewContent,new()
    {
        public sealed override ResXEditorViewContent CreateViewContent(ResXData data) => new T().Initialize(data);
    }

    class ResXStringDisplayBinding : ResXEditorBinding<ResXEditorStringsViewContent>
    {
        public override IEnumerable<Type> TypesHandled => new[] { typeof(string) };
    }

    class ResXImageDisplayBinding : ResXEditorBinding<ResXEditorImageViewContent>
    {
        public override IEnumerable<Type> TypesHandled => new[] { typeof(System.Drawing.Bitmap), typeof(System.Drawing.Icon) };
    }

    class ResXOtherDisplayBinding : ResXEditorBinding<ResXEditorOtherViewContent>
    {
        public override IEnumerable<Type> TypesHandled => Array.Empty<Type>();
    }
}
