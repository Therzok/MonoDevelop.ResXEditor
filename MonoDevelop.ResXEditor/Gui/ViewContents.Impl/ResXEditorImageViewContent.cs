using System;
namespace MonoDevelop.ResXEditor
{
	class ResXEditorImageViewContent : ResXEditorTableViewContent
	{
        System.Drawing.Image GetDrawingImage(ResXData data, ResXNode node)
        {
            if (node.TypeName == typeof(System.Drawing.Bitmap).AssemblyQualifiedName)
                return (System.Drawing.Image)data.GetValue(node);
            if (node.TypeName == typeof(System.Drawing.Icon).AssemblyQualifiedName)
                return ((System.Drawing.Icon)data.GetValue(node)).ToBitmap();
            return null;
        }

        protected override Xwt.Drawing.Image GetImage(ResXNode node)
        {
            var bitmap = GetDrawingImage(Data, node);
            if (bitmap == null)
                return null;

            var path = System.IO.Path.GetTempFileName();
            try
            {
                // FIXME: No idea why memory stream loading does not work.
                //new System.IO.MemoryStream ())
                //{
                //    bitmap.Save(ms, bitmap.RawFormat);
                //    view.Image = Xwt.Drawing.Image.FromStream(ms);
                //}

                using (var ms = System.IO.File.OpenWrite(path))
                {
                    bitmap.Save(ms, bitmap.RawFormat);
                }
                return Xwt.Drawing.Image.FromFile(path);
            }
            finally
            {
                System.IO.File.Delete(path);
            }
        }

        // Make this smarter.
        protected override bool SkipNode(ResXNode node) =>
            !(node.TypeName == typeof(System.Drawing.Bitmap).AssemblyQualifiedName) &&
            !(node.TypeName == typeof(System.Drawing.Icon).AssemblyQualifiedName);

        public override string TabPageLabel => "Images";
	}
}
