namespace MonoDevelop.ResXEditor
{
    public abstract class ResXEditorTableViewContent : ResXEditorViewContent
    {
#if TABLE
		Xwt.Table container;
#else
		Xwt.HBox container;
#endif

		protected sealed override void OnInitialize()
        {
#if TABLE
            container = new Xwt.Table();
#else
			container = new Xwt.HBox();
#endif
			container.CanGetFocus = true;
		}

		protected override void OnToolbarSet()
		{
			// TODO: need to add the Remove button
			base.OnToolbarSet();
		}

		protected override void OnDataChanged (ResXData data)
        {
            base.OnDataChanged (data);

			container.Clear();

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

#if TABLE
				container.Add(CreateItem(node.Name, image), col++, row);
                if (col == 3) {
                    col = 0;
                    row++;
                }
#else
				container.PackStart(CreateItem(node.Name, image));
#endif
			}
        }

        Xwt.Widget CreateItem (string title, Xwt.Drawing.Image image)
        {
            if (image.Size.Width > 150 || image.Size.Height > 150)
            {
                image = image.WithBoxSize(150);
            }

            var vbox = new Xwt.VBox();
			RegisterFocusHandlers(vbox);

            vbox.PackStart(new Xwt.Label(title)
            {
                HorizontalPlacement = Xwt.WidgetPlacement.Center,
                TextAlignment = Xwt.Alignment.Center,
				CanGetFocus = false,
            });

			vbox.PackStart(new Xwt.ImageView(image)
			{
				HorizontalPlacement = Xwt.WidgetPlacement.Center,
				CanGetFocus = false,
			});

			return vbox;
        }

		void RegisterFocusHandlers (Xwt.Widget widget)
		{
			// FIXME: No background color selection
			widget.CanGetFocus = true;

			widget.GotFocus += (sender, e) =>
			{
				var w = (Xwt.Widget)sender;
				w.BackgroundColor = Ide.Gui.Styles.BaseSelectionBackgroundColor;
			};

			widget.LostFocus += (sender, e) =>
			{
				var w = (Xwt.Widget)sender;
				w.BackgroundColor = Ide.Gui.Styles.BackgroundColor;
			};
		}

		protected sealed override Xwt.Widget CreateContent() => container;
        protected abstract Xwt.Drawing.Image GetImage(ResXNode node);
        protected abstract bool SkipNode(ResXNode node);
    }
}
