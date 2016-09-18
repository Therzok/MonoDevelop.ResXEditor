using System;
namespace MonoDevelop.ResXEditor
{
	public abstract class ResXEditorListViewContent : ResXEditorViewContent
	{
		protected readonly Gtk.ListStore store;
		protected readonly Gtk.TreeView treeView;

		Gtk.CellRendererText crtName, crtValue, crtComment;
		protected ResXEditorListViewContent(ResXData data) : base(data)
		{
			store = new Gtk.ListStore(typeof(ResXNode));
			treeView = new Gtk.TreeView(store);


			foreach (var node in data.Nodes)
			{
				if (SkipNode(node))
					continue;

				store.InsertWithValues(-1, node);
			}

			AddTreeViewColumns();
		}

		protected virtual void AddTreeViewColumns()
		{
			crtName = MakeEditableCellRenderer();
			crtValue = MakeEditableCellRenderer();
			crtComment = MakeEditableCellRenderer();
			treeView.AppendColumn("Name", crtName, new Gtk.TreeCellDataFunc(NameDataFunc));
			treeView.AppendColumn("Value", crtValue, new Gtk.TreeCellDataFunc(ValueDataFunc));
			treeView.AppendColumn("Comment", crtComment, new Gtk.TreeCellDataFunc(CommentDataFunc));
		}

		protected abstract bool SkipNode(ResXNode node);

		Gtk.CellRendererText MakeEditableCellRenderer()
		{
			var crt = new Gtk.CellRendererText { Editable = true };
			crt.Edited += CellEdited;
			return crt;
		}

		void CellEdited(object o, Gtk.EditedArgs args)
		{
			if (args.NewText == string.Empty)
				return;

			Gtk.TreeIter iter;
			if (!store.GetIterFromString(out iter, args.Path))
				return;

			var node = (ResXNode)store.GetValue(iter, 0);
			if (o == crtName)
			{
				node.Name = args.NewText;
			}
			else if (o == crtValue)
			{
				try
				{
					node.Value = Convert.ChangeType(args.NewText, node.Value.GetType());
				}
				catch
				{
					return;
				}
			}
			else {
				node.Comment = args.NewText;
			}

			// TODO: Maybe only do it on user save?
			Data.WriteToFile();
		}

		protected static void NameDataFunc(Gtk.CellLayout cell_layout, Gtk.CellRenderer cell, Gtk.TreeModel tree_model, Gtk.TreeIter iter)
		{
			var dataNode = (ResXNode)tree_model.GetValue(iter, 0);
			var crt = (Gtk.CellRendererText)cell;

			crt.Text = dataNode.Name;
		}

		protected static void ValueDataFunc(Gtk.CellLayout cell_layout, Gtk.CellRenderer cell, Gtk.TreeModel tree_model, Gtk.TreeIter iter)
		{
			var dataNode = (ResXNode)tree_model.GetValue(iter, 0);
			var crt = (Gtk.CellRendererText)cell;

			crt.Text = Convert.ToString(dataNode.Value);
		}

		protected static void CommentDataFunc(Gtk.CellLayout cell_layout, Gtk.CellRenderer cell, Gtk.TreeModel tree_model, Gtk.TreeIter iter)
		{
			var dataNode = (ResXNode)tree_model.GetValue(iter, 0);
			var crt = (Gtk.CellRendererText)cell;

			crt.Text = dataNode.Comment;
		}

		public override Components.Control Control
		{
			get
			{
				return treeView;
			}
		}
	}
}
