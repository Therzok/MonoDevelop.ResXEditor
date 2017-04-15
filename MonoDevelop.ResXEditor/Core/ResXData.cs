using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;

namespace MonoDevelop.ResXEditor
{
    public class ResXData
    {
        public IList<ResXNode> Nodes { get; private set; }
        public IList<ResXNode> Metadata { get; private set; }

        public string Path { get; }
		ResXData(string path) => Path = path;

		public T GetValue<T>(ResXNode node)
		{
            return (T)GetValue(node);
		}

        public object GetValue(ResXNode node)
        {
            var fileRef = node.ObjectValue as ResXFileRef;
            if (fileRef != null)
            {
                var absolutePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName (Path), fileRef.FileName);
                var absoluteRef = new ResXFileRef(absolutePath, fileRef.TypeName, fileRef.TextFileEncoding);
                var newNode = new ResXDataNode(node.Name, absoluteRef).GetValue(Constants.DefaultResolutionService);
                return newNode;
             }
            return node.ObjectValue;
        }

        /*public object SetValue(ResXNode node)
        {
            if (objValue.GetType() != value.GetType())
                throw new ArgumentException(string.Format("Type should be {0}, but was {1}", objValue.GetType(), value.GetType()), nameof(value));

            objValue = value;
        }*/

        public void WriteToFile()
        {
            using (var writer = new ResXResourceWriter(Path))
            {
                foreach (var item in Nodes)
                    writer.AddResource(item);

                foreach (var item in Metadata)
                    writer.AddMetadata(item.Name, GetValue(item));

                writer.Generate();
            }
        }

        public static ResXData FromFile(string path)
        {
            List<ResXNode> nodes, metadata;
            using (var reader = new ResXResourceReader(path) { UseResXDataNodes = true, })
            {
                nodes = reader.Cast<DictionaryEntry>().Select(x => (ResXNode)(ResXDataNode)x.Value).ToList();
                metadata = new List<ResXNode>();

                var enumerator = reader.GetMetadataEnumerator();
                while (enumerator.MoveNext())
                {
                    metadata.Add((ResXDataNode)enumerator.Value);
                }
            }

            return new ResXData(path)
            {
                Nodes = nodes,
                Metadata = metadata,
            };
        }
    }
}
