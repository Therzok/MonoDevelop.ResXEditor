namespace MonoDevelop.ResXEditor
{
    public abstract class ResXEditorTableViewContent : ResXEditorViewContent
    {
        Xwt.Table table;
        protected sealed override void OnInitialize()
        {
            table = new Xwt.Table();
        }

        protected override void OnDataChanged (ResXData data)
        {
            base.OnDataChanged (data);

            table.Clear ();

            int row = 0;
            int col = 0;

            // smart resizing
            foreach (var node in data.Nodes)
            {
                names.Add(node.Name);
                if (SkipNode(node))
                    continue;

                var image = GetImage(node);
                if (image == null)
                    continue;

                table.Add(CreateItem(node.Name, image), col++, row);
                if (col == 3) {
                    col = 0;
                    row++;
                }
            }
        }

        Xwt.Widget CreateItem (string title, Xwt.Drawing.Image image)
        {
            if (image.Size.Width > 150 || image.Size.Height > 150)
            {
                image = image.WithBoxSize(150);
            }

            var vbox = new Xwt.VBox();
            vbox.PackStart(new Xwt.Label(title)
            {
                HorizontalPlacement = Xwt.WidgetPlacement.Center,
                TextAlignment = Xwt.Alignment.Center,
            });
            vbox.PackStart(new Xwt.ImageView(image)
            {
                HorizontalPlacement = Xwt.WidgetPlacement.Center,
            });
            return vbox;
        }

        protected sealed override Xwt.Widget CreateContent() => table;
        protected abstract Xwt.Drawing.Image GetImage(ResXNode node);
        protected abstract bool SkipNode(ResXNode node);
    }
}
