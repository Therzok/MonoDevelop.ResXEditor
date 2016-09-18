using System;
using System.Linq;

namespace MonoDevelop.ResXEditor
{
	class ResXEditorOtherViewContent : ResXEditorListViewContent
	{
		public ResXEditorOtherViewContent(ResXData data) : base(data)
		{
			treeView.ShowAll();
		}

		protected override bool SkipNode(ResXNode node)
		{
			return ResXEditorKnownEditors.IsKnownType(node.Value.GetType());
		}

		protected override void AddTreeViewColumns()
		{
			base.AddTreeViewColumns();
			treeView.AppendColumn("Type", new Gtk.CellRendererText(), new Gtk.TreeCellDataFunc(TypeDataFunc));
		}

		static void TypeDataFunc(Gtk.CellLayout cell_layout, Gtk.CellRenderer cell, Gtk.TreeModel tree_model, Gtk.TreeIter iter)
		{
			var dataNode = (ResXNode)tree_model.GetValue(iter, 0);
			var crt = (Gtk.CellRendererText)cell;

			crt.Text = dataNode.TypeName;
		}

		public override string TabPageLabel
		{
			get
			{
				return "Other";
			}
		}
	}
}
