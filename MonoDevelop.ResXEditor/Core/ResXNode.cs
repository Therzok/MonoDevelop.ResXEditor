using System.Resources;

namespace MonoDevelop.ResXEditor
{
    public class ResXNode
    {
        public ResXNode(string name, object value, string comment, string typeName)
        {
            Name = name;
            ObjectValue = value;
            Comment = comment;
            TypeName = typeName;
        }

        public ResXNode(string name, ResXFileRef fileRef, string comment, string typeName)
        {
            Name = name;
            ObjectValue = fileRef;
            Comment = comment;
            TypeName = typeName;
        }

        public string Name { get; set; }
        public string Comment { get; set; }
        public string TypeName { get; set; }
        internal object ObjectValue { get; set; }

        public static implicit operator ResXDataNode(ResXNode node)
        {
            var fileRef = node.ObjectValue as ResXFileRef;
            ResXDataNode resxNode;

            if (fileRef != null)
                resxNode = new ResXDataNode(node.Name, fileRef);
            else
                resxNode = new ResXDataNode(node.Name, node.ObjectValue);
            resxNode.Comment = node.Comment;
            return resxNode;
        }

        public static implicit operator ResXNode(ResXDataNode node)
        {
            return new ResXNode(
                node.Name,
                node.FileRef ?? node.GetValue(Constants.DefaultResolutionService),
                node.Comment,
                node.GetValueTypeName(Constants.DefaultResolutionService)
            );
        }
    }
}
