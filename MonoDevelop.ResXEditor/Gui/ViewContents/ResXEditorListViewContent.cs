using System;
using System.Collections.Generic;

namespace MonoDevelop.ResXEditor
{
	public abstract class ResXEditorListViewContent : ResXEditorViewContent
	{
		protected readonly Gtk.ListStore store;
		protected readonly Gtk.TreeView treeView;

		protected readonly Gtk.CellRendererText crt;
		Gtk.CellRendererText crtName, crtValue, crtComment;
		HashSet<string> names = new HashSet<string>();
		ResXNode placeholder;
		protected ResXEditorListViewContent(ResXData data) : base(data)
		{
			store = new Gtk.ListStore(typeof(ResXNode));
			treeView = new Gtk.TreeView(store)
			{
				EnableGridLines = Gtk.TreeViewGridLines.Both,
			};
			crt = new Gtk.CellRendererText();

			foreach (var node in data.Nodes)
			{
				names.Add(node.Name);
				if (SkipNode(node))
					continue;

				store.InsertWithValues(-1, node);
			}

			AddPlaceholder();

			AddTreeViewColumns();
		}

		void AddPlaceholder()
		{
			placeholder = GetPlaceholder();
			if (placeholder != null)
				store.InsertWithValues(-1, placeholder);
		}

		protected virtual ResXNode GetPlaceholder()
		{
			return null;
		}

		protected virtual void AddTreeViewColumns()
		{
			crtName = MakeEditableCellRenderer();
			crtValue = MakeEditableCellRenderer();
			crtComment = MakeEditableCellRenderer();
			treeView.AppendColumn("", crt, new Gtk.TreeCellDataFunc(CountDataFunc));
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
			Gtk.TreeIter iter;
			if (!store.GetIterFromString(out iter, args.Path))
				return;

			var node = (ResXNode)store.GetValue(iter, 0);
			if (o == crtName)
			{
				// We can't remove a node's name, nor can we duplicate it
				if (args.NewText == string.Empty || names.Contains(args.NewText))
					return;

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
			else if (o == crtComment)
			{
				node.Comment = args.NewText;
			}

			if (node == placeholder)
			{
				if (node.Name == string.Empty)
					return;

				Data.Nodes.Add(node);
				AddPlaceholder();
			}

			// TODO: Maybe only do it on user save?
			Data.WriteToFile();
		}

		protected static void CountDataFunc(Gtk.CellLayout cell_layout, Gtk.CellRenderer cell, Gtk.TreeModel tree_model, Gtk.TreeIter iter)
		{
			var dataNode = (ResXNode)tree_model.GetValue(iter, 0);
			var crt = (Gtk.CellRendererText)cell;
			if (dataNode.Name == string.Empty)
			{
				if (dataNode.Comment != null || (string)dataNode.Value != string.Empty)
					crt.Text = "!";
				else
					crt.Text = "*";
			}
			else
			{
				crt.Text = string.Empty;
			}
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
