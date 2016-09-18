using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using MonoDevelop.Core;

namespace MonoDevelop.ResXEditor
{
	public class ResXData
	{
		public IList<ResXNode> Nodes { get; private set; }
		public IList<ResXNode> Metadata { get; private set; }

		FilePath path;
		ResXData(FilePath path)
		{
			this.path = path;
		}

		public void WriteToFile()
		{
			using (var writer = new ResXResourceWriter(path))
			{
				foreach (var item in Nodes)
					writer.AddResource(item);

				// FIXME:
				//foreach (var item in Metadata)
				//	writer.AddMetadata()

				writer.Generate();
			}
			FileService.NotifyFileChanged(path);
		}

		internal static ResXData FromFile(FilePath path)
		{
			List<ResXNode> nodes, metadata;
			using (var reader = new ResXResourceReader(path) { UseResXDataNodes = true, }) {
				nodes = reader.Cast<DictionaryEntry>().Select(x => (ResXNode)(ResXDataNode)x.Value).ToList();
				metadata = new List<ResXNode>();

				var enumerator = reader.GetMetadataEnumerator();
				while (enumerator.MoveNext())
				{
					metadata.Add((ResXNode)(ResXDataNode)enumerator.Value);
				}
			}

			return new ResXData (path)
			{
				Nodes = nodes,
			};
		}
	}
}
