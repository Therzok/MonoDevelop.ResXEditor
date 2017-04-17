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
                using (var ms = new System.IO.MemoryStream ())
                {
                    bitmap.Save(ms, bitmap.RawFormat);
                    ms.Position = 0;
                    return Xwt.Drawing.Image.FromStream(ms);
                }
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
