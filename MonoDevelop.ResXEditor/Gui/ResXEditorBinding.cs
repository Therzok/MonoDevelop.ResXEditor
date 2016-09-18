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
		public override ResXEditorViewContent CreateViewContent(ResXData data)
		{
			return new ResXEditorStringsViewContent(data);
		}

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
		public override ResXEditorViewContent CreateViewContent(ResXData data)
		{
			return new ResXEditorImageViewContent(data);
		}

		public override IEnumerable<Type> TypesHandled
		{
			get
			{
				yield break;
			}
		}
	}

	class ResXOtherDisplayBinding : ResXEditorBinding
	{
		public override ResXEditorViewContent CreateViewContent(ResXData data)
		{
			return new ResXEditorOtherViewContent(data);
		}

		public override IEnumerable<Type> TypesHandled
		{
			get
			{
				yield break;
			}
		}
	}
}
