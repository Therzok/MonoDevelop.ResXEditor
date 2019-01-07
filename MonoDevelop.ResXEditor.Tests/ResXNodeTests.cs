using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using NUnit.Framework;

namespace MonoDevelop.ResXEditor.Tests
{
    [TestFixture]
    public class ResXNodeTests
	{
        static readonly string resxFile = Path.Combine(Path.GetDirectoryName(typeof(ResXNodeTests).Assembly.Location), "Data", "Image.resx");
		[Test]
		public void TestConvertToAndFromResXNode()
		{
			var resxdata = ResXData.FromFile(resxFile);

            foreach (var icon in resxdata.Nodes)
            {
                ResXDataNode node = icon;
                ResXNode icon2 = node;

                AreEqual(resxdata.GetValue(icon), resxdata.GetValue(icon2));
                Assert.AreEqual(icon.Name, icon2.Name);
                Assert.AreEqual(icon.TypeName, icon2.TypeName);
                Assert.AreEqual(icon.Comment, icon2.Comment);
            }
		}

		[Test]
		public void TestConvertToAndFromResXDataNode()
		{
            List<ResXDataNode> nodes;
            using (var reader = new ResXResourceReader(resxFile) { UseResXDataNodes = true, })
            {
                nodes = reader.Cast<DictionaryEntry>().Select(x => (ResXDataNode)x.Value).ToList();
            }
            foreach (var node in nodes)
            {
                ResXNode icon = node;
                ResXDataNode node2 = icon;

                var resolution = default(System.ComponentModel.Design.ITypeResolutionService);
                Assert.AreEqual(node.GetValueTypeName(resolution), node2.GetValueTypeName(resolution));
                Assert.AreEqual(node.Name, node2.Name);
                Assert.AreEqual(node.Comment, node2.Comment);
                Assert.AreEqual(node.FileRef, node2.FileRef);

                if (node.FileRef == null)
                    AreEqual(node.GetValue(resolution), node2.GetValue(resolution));
			}
		}

        static void AreEqual (object a, object b)
        {
            if (a == b)
                return;

            if (a is System.Drawing.Image) {
				var aa = (System.Drawing.Image)a;
				var bb = (System.Drawing.Image)b;

				Assert.AreEqual(aa.Width, bb.Width);
                Assert.AreEqual(aa.Height, bb.Height);
                return;
            }
            if (a is System.Drawing.Icon) {
                var aa = (System.Drawing.Icon)a;
                var bb = (System.Drawing.Icon)b;

				Assert.AreEqual(aa.Width, bb.Width);
                Assert.AreEqual(aa.Height, bb.Height);
                return;
            }
            Assert.Fail($"No comparer for {a.GetType()}");
        }
    }
}
