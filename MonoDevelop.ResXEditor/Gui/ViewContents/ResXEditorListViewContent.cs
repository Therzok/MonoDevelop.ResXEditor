using System;
using System.Collections.Generic;
using MonoDevelop.Components;
using MonoDevelop.Core;

namespace MonoDevelop.ResXEditor
{
    public abstract class ResXEditorListViewContent : ResXEditorViewContent
    {
        Xwt.ListStore store;
        Xwt.ListView listView;

        HashSet<string> names = new HashSet<string>();
        Xwt.DataField<string> countField = new Xwt.DataField<string>();
        Xwt.DataField<string> nameField = new Xwt.DataField<string>();
        Xwt.DataField<string> valueField = new Xwt.DataField<string>();
        Xwt.DataField<string> commentField = new Xwt.DataField<string>();
        Xwt.DataField<string> typeField = new Xwt.DataField<string>();
        Xwt.DataField<ResXNode> nodeField = new Xwt.DataField<ResXNode>();

        protected override void OnInitialize(ResXData data)
        {
            store = OnCreateListStore();

            listView = new Xwt.ListView(store)
            {
                GridLinesVisible = Xwt.GridLines.Both,
                SelectionMode = Xwt.SelectionMode.Multiple,
            };
            listView.ButtonPressed += OnButtonPress;
            listView.Show();

            AddListViewColumns(listView.Columns);

            foreach (var node in data.Nodes)
            {
                names.Add(node.Name);
                if (SkipNode(node))
                    continue;

                var row = store.AddRow();
                OnAddValues(store, row, node);
            }

            AddPlaceholder();
        }

        protected virtual Xwt.ListStore OnCreateListStore()
        {
            return new Xwt.ListStore(countField, nameField, valueField, commentField, typeField, nodeField);
        }

        protected virtual void AddListViewColumns(Xwt.ListViewColumnCollection collection)
        {
            collection.Add(" ", new Xwt.TextCellView(countField));
            collection.Add("Name", MakeEditableTextCell(nameField));
            collection.Add("Value", MakeEditableTextCell(valueField));
            collection.Add("Comment", MakeEditableTextCell(commentField, ellipsize: true));
        }

        protected virtual void OnAddValues(Xwt.ListStore store, int row, ResXNode node)
        {
            store.SetValues(row,
                            countField, row == store.RowCount - 1 ? "*" : string.Empty,
                            nameField, node.Name,
                            valueField, node.ObjectValue.ToString(),
                            commentField, node.Comment,
                            typeField, node.TypeName,
                            nodeField, node);
        }

        protected abstract bool SkipNode(ResXNode node);
        protected virtual ResXNode GetPlaceholder()
        {
            return null;
        }

        void AddPlaceholder()
        {
            var placeholder = GetPlaceholder();
            if (placeholder != null)
            {
                var row = store.AddRow();
                OnAddValues(store, row, placeholder);
            }
        }

        Xwt.TextCellView MakeEditableTextCell(Xwt.IDataField field, bool ellipsize = false)
        {
            var etc = new Xwt.TextCellView(field)
            {
                Editable = true,
                TextField = field,
            };
            etc.TextChanged += TextChanged;
            return etc;
        }

        void OnButtonPress(object o, Xwt.ButtonEventArgs args)
        {
            if (args.Button != Xwt.PointerButton.Right)
                return;

            var selection = listView.SelectedRows;

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
                var name = dataSource.GetValue(row, nameField);
                if (row == dataSource.RowCount - 1)
                {
                    dataSource.SetValues(row,
                                         valueField, string.Empty,
                                         commentField, null);
                    continue;
                }

                dataSource.RemoveRow(row - removed++);
            }
            // FIXME: Serialize
            //Data.WriteToFile();
            //FileService.NotifyFileChanged(Data.Path);
        }


        void TextChanged(object o, Xwt.WidgetEventArgs args)
        {
            var etc = (Xwt.TextCellView)o;

            var row = listView.CurrentEventRow;
            var node = store.GetValue(row, nodeField);
            var name = store.GetValue(row, nameField);
            if (name == string.Empty)
            {
                if (store.GetValue(row, valueField) != string.Empty)
                    store.SetValue(row, countField, "!");
                else
                    store.SetValue(row, countField, "*");
            }
            else
            {
                store.SetValue(row, countField, string.Empty);
                AddPlaceholder();
            }

            // FIXME: Need Xwt with NewText in args.
            string newText = etc.Text; // args.NewText;
            if (etc.TextField == nameField)
            {
                // If we already have a key with that name, revert to the old text, otherwise remove it from the set.
                if (names.Contains(newText) || newText == string.Empty)
                    args.Handled = false;
                else
                {
                    names.Remove(etc.Text);
                    names.Add(newText);
                }
                node.Name = newText;
            }
            else if (etc.TextField == valueField)
            {
                try
                {
                    // Check FileRef support.
                    node.ObjectValue = Convert.ChangeType(newText, Data.GetValue(node).GetType());
                }
                catch
                {
                    args.Handled = false;
                    return;
                }
            }
            else if (etc.TextField == commentField)
            {
                node.Comment = newText;
            }

            if (listView.CurrentEventRow == store.RowCount - 1)
            {
                if (name == string.Empty)
                    return;
            }

            // TODO: Maybe only do it on user save?
            //listView.ColumnsAutosize();
            //Data.WriteToFile();
        }

        protected sealed override Xwt.Widget CreateContent()
        {
            return listView;
        }
    }
}
