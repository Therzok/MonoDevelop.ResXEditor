using System;
using System.Collections.Generic;
using MonoDevelop.Components;

namespace MonoDevelop.ResXEditor
{
	public abstract class ResXEditorListViewContent : ResXEditorViewContent
	{
		const string ResXDataKey = "source";
		protected readonly Gtk.ListStore store;
		protected readonly Gtk.TreeView treeView;

		protected readonly Gtk.CellRendererText crt;
		Gtk.CellRendererText crtName, crtValue, crtComment;
		HashSet<string> names = new HashSet<string>();
		ResXNode placeholder;
		protected ResXEditorListViewContent(ResXData data) : base(data)
		{
			store = new Gtk.ListStore(typeof(ResXNode));
			store.Data[ResXDataKey] = data;
			treeView = new Gtk.TreeView(store)
			{
				EnableGridLines = Gtk.TreeViewGridLines.Both,
			};
			crt = new Gtk.CellRendererText();

			treeView.ButtonPressEvent += OnTreeViewButtonPress;
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

		[GLib.ConnectBefore]
		static void OnTreeViewButtonPress(object o, Gtk.ButtonPressEventArgs args)
		{
			args.RetVal = false;
			if (args.Event.Type != Gdk.EventType.ButtonPress || args.Event.Button != 3)
				return;

			var treeView = (Gtk.TreeView)o;
			var selection = treeView.Selection;
			Gtk.TreePath path;
			if (treeView.GetPathAtPos((int)args.Event.X, (int)args.Event.Y, out path))
				selection.SelectPath(path);
			else
				return;
			
			args.RetVal = true;
			var menu = new ContextMenu();
			var mi = new ContextMenuItem("Remove");
			mi.Context = selection;
			mi.Clicked += OnPopupMenuActivated;
			menu.Add(mi);

			// FIXME: Coordinates
			menu.Show(treeView, (int)args.Event.X, (int)args.Event.Y);
		}

		static void OnPopupMenuActivated(object o, ContextMenuItemClickedEventArgs args)
		{
			var selection = (Gtk.TreeSelection)args.Context;
			var model = (Gtk.ListStore)selection.TreeView.Model;
			var data = (ResXData)model.Data[ResXDataKey];

			foreach (var path in selection.GetSelectedRows())
			{
				Gtk.TreeIter iter;
				if (!model.GetIter(out iter, path))
					continue;

				var node = (ResXNode)model.GetValue(iter, 0);
				if (node.Name == string.Empty)
				{
					node.Value = string.Empty;
					node.Comment = null;
					continue;
				}

				if (!model.Remove(ref iter))
					continue;

				data.Nodes.Remove(node);
			}
			data.WriteToFile();
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
			treeView.ColumnsAutosize();
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

		protected override Components.Control CreateContent()
		{
			return treeView;
		}

		public override void Dispose()
		{
			store.Data[ResXDataKey] = null;
			base.Dispose();
		}
	}
}
