using System;
namespace MonoDevelop.ResXEditor
{
	class ResXEditorImageViewContent : ResXEditorGridViewContent
	{
		Gtk.HBox box;
		public ResXEditorImageViewContent(ResXData data) : base(data)
		{
			box = new Gtk.HBox();
            box.Add(new Gtk.Label("Hello world"));
			box.ShowAll();
		}

		public override string TabPageLabel
		{
			get
			{
				return "Images";
			}
		}

		protected override Components.Control CreateContent()
		{
			return box;
		}
	}
}
