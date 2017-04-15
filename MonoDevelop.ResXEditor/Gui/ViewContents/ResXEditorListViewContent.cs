using System;
using System.Collections.Generic;
using MonoDevelop.Components;
using MonoDevelop.Core;

namespace MonoDevelop.ResXEditor
{
    public abstract class ResXEditorListViewContent : ResXEditorViewContent
    {
        readonly Xwt.ListStore store;
        readonly Xwt.ListView listView;

        HashSet<string> names = new HashSet<string>();
        ResXNode placeholder;
        Xwt.DataField<ResXNode> nodeField = new Xwt.DataField<ResXNode>();
        protected ResXEditorListViewContent()
        {
            store = new Xwt.ListStore(nodeField);
            listView = new Xwt.ListView(store)
            {
                GridLinesVisible = Xwt.GridLines.Both,
            };
            listView.ButtonPressed += OnButtonPress;
            listView.Show();
        }

        protected override void OnInitialize(ResXData data)
        {
			AddPlaceholder();

			AddColumns(listView.Columns);

			foreach (var node in data.Nodes)
			{
				names.Add(node.Name);
				if (SkipNode(node))
					continue;

				var row = store.AddRow();
				store.SetValue(row, nodeField, node);
			}
        }

        void OnButtonPress(object o, Xwt.ButtonEventArgs args)
        {
            if (args.Button != Xwt.PointerButton.Right)
                return;

            var listView = (Xwt.ListView)o;
            var selection = listView.SelectedRows;

            //listView.GetRowAtPosition(args.X, args.Y);
            //select it

            var menu = new ContextMenu();
            var mi = new ContextMenuItem("Remove Row");
            mi.Context = listView;
            mi.Clicked += OnPopupMenuActivated;
            menu.Add(mi);

            // FIXME: Coordinates
            menu.Show(listView.ToGtkWidget(), (int)args.X, (int)args.Y);
        }

        void OnPopupMenuActivated(object o, ContextMenuItemClickedEventArgs args)
        {
            var view = (Xwt.ListView)args.Context;
            var dataSource = (Xwt.ListStore)view.DataSource;

            int removed = 0;
            foreach (var row in view.SelectedRows)
            {
                var node = dataSource.GetValue<ResXNode>(row, nodeField);
                if (node.Name == string.Empty)
                {
                    //node.Value = string.Empty;
                    node.Comment = null;
                    continue;
                }

                dataSource.RemoveRow(row - removed++);
                Data.Nodes.Remove(node);
            }
            Data.WriteToFile();
            FileService.NotifyFileChanged(Data.Path);
        }

        void AddPlaceholder()
        {
            placeholder = GetPlaceholder();
            if (placeholder != null)
            {
                var row = store.AddRow();
                store.SetValue(row, nodeField, placeholder);
            }
        }

        protected virtual ResXNode GetPlaceholder()
        {
            return null;
        }

        protected virtual void AddColumns(Xwt.ListViewColumnCollection columns)
        {
            //         columns.Add("", countField);
            //columns.Add("Value", MakeEditableTextCell(valueField));
            //columns.Add("Comment", MakeEditableTextCell(CommentField));
            //columns.Add("", crt, new Gtk.TreeCellDataFunc(CountDataFunc));
        }

        protected abstract bool SkipNode(ResXNode node);

        Xwt.TextCellView MakeEditableTextCell(Xwt.IDataField field)
        {
            var etc = new Xwt.TextCellView(field)
            {
                Editable = true,
                TextField = null,
            };
            etc.TextChanged += TextChanged;
            return etc;
        }

        void TextChanged(object o, Xwt.WidgetEventArgs args)
        {
            //         var etc = (Xwt.TextCellView)o;

            //         var node = (ResXNode)store.GetValue(listView.SelectedRow, 0);
            //if (o == crtName)
            //{
            //	// We can't remove a node's name, nor can we duplicate it
            //	if (args.NewText == string.Empty || names.Contains(args.NewText))
            //		return;

            //	node.Name = args.NewText;
            //}
            //else if (o == crtValue)
            //{
            //	try
            //	{
            //		node.Value = Convert.ChangeType(args.NewText, node.Value.GetType());
            //	}
            //	catch
            //	{
            //		return;
            //	}
            //}
            //else if (o == crtComment)
            //{
            //	node.Comment = args.NewText;
            //}

            //if (node == placeholder)
            //{
            //	if (node.Name == string.Empty)
            //		return;

            //	Data.Nodes.Add(node);
            //	AddPlaceholder();
            //}

            //// TODO: Maybe only do it on user save?
            //listView.ColumnsAutosize();
            //Data.WriteToFile();
        }

        protected void CountDataFunc(Gtk.CellLayout cell_layout, Gtk.CellRenderer cell, Gtk.TreeModel tree_model, Gtk.TreeIter iter)
        {
            var dataNode = (ResXNode)tree_model.GetValue(iter, 0);
            var crt = (Gtk.CellRendererText)cell;
            if (dataNode.Name == string.Empty)
            {
                if (dataNode.Comment != null || (string)Data.GetValue(dataNode) != string.Empty)
                    crt.Text = "!";
                else
                    crt.Text = "*";
            }
            else
            {
                crt.Text = string.Empty;
            }
        }

        protected override Xwt.Widget CreateContent()
        {
            return listView;
        }
    }
}
