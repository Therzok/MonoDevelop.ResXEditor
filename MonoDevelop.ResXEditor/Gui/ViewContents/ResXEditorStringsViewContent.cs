using System;
namespace MonoDevelop.ResXEditor
{
	public class ResXEditorStringsViewContent : ResXEditorViewContent
	{
		Gtk.HBox box;
		public ResXEditorStringsViewContent(ResXData data) : base(data)
		{
			box = new Gtk.HBox();
			box.Add(new Gtk.Label("Hello world"));
			box.ShowAll();
		}

		public override string TabPageLabel
		{
			get
			{
				return "Strings";
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
