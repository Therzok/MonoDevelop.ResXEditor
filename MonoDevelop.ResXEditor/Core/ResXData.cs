﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using MonoDevelop.Core;

namespace MonoDevelop.ResXEditor
{
    public class ResXData
    {
        // Implement notifications to handle this better.
        public IList<ResXNode> Nodes { get; private set; }
        public IList<ResXNode> Metadata { get; private set; }
        public string Path { get; }
        public Projects.ProjectFile ProjectFile { get; private set; }

        ResXData (string path)
        {
            Path = path;
        }

        public ResXNode CreateNode (FilePath absPath, Type type)
        {
            FilePath relPath = absPath.ToRelative (((FilePath)Path).ParentDirectory);
            var fileRef = new ResXFileRef (relPath, type.AssemblyQualifiedName);
            return new ResXDataNode (absPath.FileNameWithoutExtension, fileRef);
        }

        public System.Drawing.Image GetDrawingImage (ResXNode node)
        {
            if (node.TypeName == typeof (System.Drawing.Bitmap).AssemblyQualifiedName)
                return (System.Drawing.Image)GetValue (node);
            if (node.TypeName == typeof (System.Drawing.Icon).AssemblyQualifiedName)
                return ((System.Drawing.Icon)GetValue (node)).ToBitmap ();
            return null;
        }

        public T GetValue<T> (ResXNode node)
        {
            return (T)GetValue (node);
        }

        public object GetValue (ResXNode node)
        {
            if (node.ObjectValue is ResXFileRef fileRef) {
                var absolutePath = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (Path), fileRef.FileName);
                var absoluteRef = new ResXFileRef (absolutePath, fileRef.TypeName, fileRef.TextFileEncoding);
                var newNode = new ResXDataNode (node.Name, absoluteRef).GetValue (Constants.DefaultResolutionService);
                return newNode;
            }
            return node.ObjectValue;
        }

        public void SetValue (ResXNode node, object value)
        {
            if (node.ObjectValue.GetType () != value.GetType ())
                throw new ArgumentException (string.Format ("Type should be {0}, but was {1}", node.ObjectValue.GetType (), value.GetType ()), nameof (value));

            node.ObjectValue = value;
        }

        public void WriteToFile (IEnumerable<ResXNode> nodes)
        {
            using (var writer = new ResXResourceWriter (Path)) {
                foreach (var item in nodes)
                    writer.AddResource (item);

                foreach (var item in Metadata)
                    writer.AddMetadata (item.Name, GetValue (item));

                writer.Generate ();
            }
        }

        static IEnumerable<Projects.ProjectFile> GatherResX (Projects.ProjectFile pf)
        {
            yield return pf;

        }

        public static IEnumerable<ResXData> FromProjectFile (Projects.ProjectFile pf)
        {
            foreach (var resxFile in GatherResX (pf)) {
                var ret = FromFile (resxFile.FilePath);
                ret.ProjectFile = resxFile;
                yield return ret;
            }
        }

        public static ResXData FromFile (string path)
        {
            List<ResXNode> nodes, metadata;
            using (var reader = new ResXResourceReader (path) { UseResXDataNodes = true, }) {
                nodes = reader.Cast<DictionaryEntry> ().Select (x => (ResXNode)(ResXDataNode)x.Value).ToList ();
                metadata = new List<ResXNode> ();

                var enumerator = reader.GetMetadataEnumerator ();
                while (enumerator.MoveNext ()) {
                    metadata.Add ((ResXDataNode)enumerator.Value);
                }
            }

            return new ResXData (path) {
                Nodes = nodes,
                Metadata = metadata,
            };
        }
    }
}
