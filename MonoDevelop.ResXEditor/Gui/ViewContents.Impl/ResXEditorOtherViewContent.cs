namespace MonoDevelop.ResXEditor
{
    class ResXEditorOtherViewContent : ResXEditorListViewContent
    {
        protected override bool SkipNode(ResXNode node) => ResXEditorKnownEditors.IsKnownType(node.TypeName);
        public override string TabPageLabel => "Other";

        protected override void AddListViewColumns(Xwt.ListViewColumnCollection collection)
        {
            collection.Add("Name", MakeEditableTextCell(nameField));
            collection.Add("Value", MakeEditableTextCell(valueField));
            collection.Add("Type", typeField);
            collection.Add("Comment", MakeEditableTextCell(commentField));
        }
    }
}
