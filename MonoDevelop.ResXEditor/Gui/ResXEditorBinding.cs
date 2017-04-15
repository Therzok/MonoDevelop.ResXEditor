using System;
using System.Collections.Generic;

namespace MonoDevelop.ResXEditor
{
	public abstract class ResXEditorBinding
	{
		public abstract ResXEditorViewContent CreateViewContent(ResXData data);

		public abstract IEnumerable<Type> TypesHandled { get; }
	}

	class ResXStringDisplayBinding : ResXEditorBinding
	{
        public override ResXEditorViewContent CreateViewContent(ResXData data) => new ResXEditorStringsViewContent().Initialize(data);

		public override IEnumerable<Type> TypesHandled
		{
			get
			{
				yield return typeof(string);
			}
		}
	}

	class ResXImageDisplayBinding : ResXEditorBinding
	{
        public override ResXEditorViewContent CreateViewContent(ResXData data) => new ResXEditorImageViewContent().Initialize(data);

		public override IEnumerable<Type> TypesHandled
		{
			get
			{
                yield return typeof(System.Drawing.Bitmap);
                yield return typeof(System.Drawing.Icon);
			}
		}
	}

	class ResXOtherDisplayBinding : ResXEditorBinding
	{
        public override ResXEditorViewContent CreateViewContent(ResXData data) => new ResXEditorOtherViewContent().Initialize(data);

		public override IEnumerable<Type> TypesHandled
		{
			get
			{
				yield break;
			}
		}
	}
}
