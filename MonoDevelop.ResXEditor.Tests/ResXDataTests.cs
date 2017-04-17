using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Resources;

namespace MonoDevelop.ResXEditor.Tests
{
    [TestFixture]
    public class ResXDataTests
    {
        static readonly string resxFile = Path.Combine(Path.GetDirectoryName(typeof(ResXDataTests).Assembly.Location), "Data", "Image.resx");

        static void CheckEquality(ResXData resxdata)
        {
            Assert.AreEqual(2, resxdata.Nodes.Count);

            var icon = resxdata.Nodes[0];
            Assert.AreEqual("icon", icon.Name);
            Assert.AreEqual(typeof(System.Drawing.Icon).AssemblyQualifiedName, icon.TypeName);
            var iconValue = resxdata.GetValue<System.Drawing.Icon>(icon);
            Assert.AreEqual(32, iconValue.Width);
            Assert.AreEqual(32, iconValue.Height);

            var bitmap = resxdata.Nodes[1];
            Assert.AreEqual("bitmap", bitmap.Name);
            Assert.AreEqual(typeof(System.Drawing.Bitmap).AssemblyQualifiedName, bitmap.TypeName);
            var bitmapValue = resxdata.GetValue<System.Drawing.Bitmap>(bitmap);
            Assert.AreEqual(392, bitmapValue.Width);
            Assert.AreEqual(243, bitmapValue.Height);
        }

        [Test]
        public void TestLoad()
        {
            var resxdata = ResXData.FromFile(resxFile);

            Assert.IsNull(resxdata.ProjectFile);
            CheckEquality(resxdata);
        }

        [Test]
        [Ignore("Enable when MD addin engine is initialized")]
        public void TestLoadFromProjectFile()
        {
            var pf = new Projects.ProjectFile(resxFile);
            var resxdata = ResXData.FromProjectFile(pf);

            Assert.AreSame(pf, resxdata.First().ProjectFile);
            CheckEquality(resxdata.First());
        }

		[Test]
		[Ignore("Enable when MD addin engine is initialized")]
        public void TestCreateNode()
        {
            var resxdata = ResXData.FromFile(resxFile);

            var fileRef = (ResXFileRef)resxdata.Nodes[1].ObjectValue;
            Core.FilePath path = fileRef.FileName;
            var absolutePath = Path.GetFullPath(path.ToAbsolute(resxdata.Path));
            var otherNode = (ResXDataNode)resxdata.CreateNode(absolutePath, typeof(System.Drawing.Bitmap));

            Assert.AreEqual(fileRef.FileName, otherNode.FileRef.FileName);
            Assert.AreEqual(fileRef.TypeName, otherNode.FileRef.TypeName);
            Assert.AreEqual(fileRef.TextFileEncoding, otherNode.FileRef.TextFileEncoding);

            var value = (System.Drawing.Bitmap)resxdata.GetValue(resxdata.Nodes[0]);
            var otherValue = (System.Drawing.Bitmap)resxdata.GetValue(otherNode);

			Assert.AreEqual(value.Width, otherValue.Width);
            Assert.AreEqual(value.Height, otherValue.Height);
        }

        [Test]
        public void TestGetDrawingImage()
        {
            var resxdata = ResXData.FromFile(resxFile);

            var icon = resxdata.GetDrawingImage(resxdata.Nodes[0]);
            //Assert.AreEqual(System.Drawing.Imaging.ImageFormat.Icon, icon.RawFormat);
            Assert.AreEqual(32, icon.Width);
            Assert.AreEqual(32, icon.Height);

            var bitmap = resxdata.GetDrawingImage(resxdata.Nodes[1]);
            Assert.AreEqual(System.Drawing.Imaging.ImageFormat.Png, bitmap.RawFormat);
            Assert.AreEqual(392, bitmap.Width);
            Assert.AreEqual(243, bitmap.Height);
        }

		[Test]
        public void TestModify()
		{
            string workFile = resxFile.Replace("Image", "Image2");
            try
            {
                File.Copy(resxFile, workFile);
                var resxdata = ResXData.FromFile(workFile);

                resxdata.Nodes.Add(new ResXNode("string", "test", "", "System.String"));

                resxdata.WriteToFile(resxdata.Nodes);

                resxdata = ResXData.FromFile(workFile);
                Assert.AreEqual(3, resxdata.Nodes.Count);

            }
            finally
            {
                File.Delete(workFile);
            }
		}
    }
}
