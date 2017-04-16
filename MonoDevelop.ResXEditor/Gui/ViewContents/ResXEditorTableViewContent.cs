using System;
using MonoDevelop.Components;

namespace MonoDevelop.ResXEditor
{
    public abstract class ResXEditorTableViewContent : ResXEditorViewContent
    {
        Xwt.Table table;
        protected sealed override void OnInitialize(ResXData data)
        {
            table = new Xwt.Table();

            int row = 0;
            int col = 0;
            foreach (var node in data.Nodes)
            {
                names.Add(node.Name);
                if (SkipNode(node))
                    continue;

                var image = GetImage(node);
                if (image == null)
                    continue;

                if (image.Size.Width > 150 || image.Size.Height > 150) {
                    image = image.WithBoxSize(150);
                }
                var view = new Xwt.ImageView(image);
                table.Add(view, col++, row);
                if (col == 3) {
                    col = 0;
                    row++;
                }
            }
        }

        protected sealed override Xwt.Widget CreateContent() => table;
        protected abstract Xwt.Drawing.Image GetImage(ResXNode node);
        protected abstract bool SkipNode(ResXNode node);
        protected override bool HasToolbar => true;
    }
}
