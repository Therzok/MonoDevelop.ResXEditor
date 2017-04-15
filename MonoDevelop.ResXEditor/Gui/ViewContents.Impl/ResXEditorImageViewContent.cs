using System;
namespace MonoDevelop.ResXEditor
{
	class ResXEditorImageViewContent : ResXEditorGridViewContent
	{
        Xwt.HBox box;
        Xwt.ImageView view;
		public ResXEditorImageViewContent()
		{
			box = new Xwt.HBox();
            box.PackStart(view = new Xwt.ImageView());
            box.Show();
		}

        System.Drawing.Image GetImageFromNode(ResXData data, ResXNode node)
        {
            if (node.TypeName == typeof(System.Drawing.Image).AssemblyQualifiedName)
                return (System.Drawing.Image)data.GetValue(node);

            if (node.TypeName == typeof(System.Drawing.Icon).AssemblyQualifiedName)
                return ((System.Drawing.Icon)data.GetValue(node)).ToBitmap();
            return null;
        }

        protected override void OnInitialize(ResXData data)
        {
            foreach (var node in data.Nodes)
            {
                var bitmap = GetImageFromNode(data, node);
                if (bitmap == null)
                    continue;
                // FIXME: No idea why memory stream loading does not work.
                //new System.IO.MemoryStream ())
                //            {
                //                bitmap.Save(ms, bitmap.RawFormat);
                //                view.Image = Xwt.Drawing.Image.FromStream(ms);
                //}

                var path = System.IO.Path.GetTempFileName();
                using (var ms = System.IO.File.OpenWrite(path))
                {
                    bitmap.Save(ms, bitmap.RawFormat);
                }
                view.Image = Xwt.Drawing.Image.FromFile(path);
                System.IO.File.Delete(path);
            }
            base.OnInitialize(data);
        }

        public override string TabPageLabel => "Images";
        protected override Xwt.Widget CreateContent() => box;
	}
}
