using System;
namespace MonoDevelop.ResXEditor
{
	public class ResXEditorImageViewContent : ResXEditorViewContent
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

		public override Components.Control Control
		{
			get
			{
				return box;
			}
		}
	}
}
