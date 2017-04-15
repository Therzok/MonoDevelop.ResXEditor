using System;
using System.Linq;

namespace MonoDevelop.ResXEditor
{
	class ResXEditorOtherViewContent : ResXEditorListViewContent
	{
        protected override bool SkipNode(ResXNode node) =>  ResXEditorKnownEditors.IsKnownType(node.TypeName);

		protected override void AddColumns(Xwt.ListViewColumnCollection columns)
		{
            base.AddColumns(columns);
            columns.Add("Type", new Xwt.TextCellView ());
//			listView.AppendColumn("Type", crt, new Gtk.TreeCellDataFunc(TypeDataFunc));
		}

		static void TypeDataFunc(Gtk.CellLayout cell_layout, Gtk.CellRenderer cell, Gtk.TreeModel tree_model, Gtk.TreeIter iter)
		{
			var dataNode = (ResXNode)tree_model.GetValue(iter, 0);
			var crt = (Gtk.CellRendererText)cell;

			crt.Text = dataNode.TypeName;
		}

        public override string TabPageLabel => "Other";
	}
}
