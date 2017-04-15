using System;
namespace MonoDevelop.ResXEditor
{
	class ResXEditorImageViewContent : ResXEditorGridViewContent
	{
        Xwt.HBox box;
		public ResXEditorImageViewContent()
		{
			box = new Xwt.HBox();
            box.PackStart(new Xwt.Label("Hello world"));
            box.Show();
		}

        public override string TabPageLabel => "Images";
        protected override Xwt.Widget CreateContent() => box;
	}
}
