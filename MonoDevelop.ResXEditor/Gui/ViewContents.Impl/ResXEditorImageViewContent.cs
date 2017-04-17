namespace MonoDevelop.ResXEditor
{
    class ResXEditorImageViewContent : ResXEditorTableViewContent
	{
        protected override Xwt.Drawing.Image GetImage(ResXNode node)
        {
            var bitmap = Data.GetDrawingImage(node);
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

                using (var fs = System.IO.File.OpenWrite(path))
                    bitmap.Save(fs, bitmap.RawFormat);
                return Xwt.Drawing.Image.FromFile(path);
            }
            finally
            {
                System.IO.File.Delete(path);
            }
        }

        protected override bool SkipNode(ResXNode node) =>
            !(node.TypeName == typeof(System.Drawing.Bitmap).AssemblyQualifiedName) &&
            !(node.TypeName == typeof(System.Drawing.Icon).AssemblyQualifiedName);
        public override string TabPageLabel => "Images";
	}
}
