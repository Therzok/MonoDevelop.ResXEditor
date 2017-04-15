using NUnit.Framework;
using System;
using System.IO;
namespace MonoDevelop.ResXEditor.Tests
{
    [TestFixture]
    public class ResXDataTests
    {
        static readonly string resxFile = Path.Combine(Path.GetDirectoryName (typeof(ResXDataTests).Assembly.Location), "Data", "Image.resx");
        [Test]
        public void TestLoad()
        {
            var resxdata = ResXData.FromFile(resxFile);

            Assert.AreEqual(2, resxdata.Nodes.Count);

            var icon = resxdata.Nodes[0];
			Assert.AreEqual("icon", icon.Name);
            Assert.AreEqual("System.Drawing.Icon, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", icon.TypeName);
            var iconValue = resxdata.GetValue<System.Drawing.Icon>(icon);
            Assert.AreEqual(32, iconValue.Width);
            Assert.AreEqual(32, iconValue.Height);

            var bitmap = resxdata.Nodes[1];
            Assert.AreEqual("bitmap", bitmap.Name);
			Assert.AreEqual("System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", bitmap.TypeName);
            var bitmapValue = resxdata.GetValue<System.Drawing.Bitmap>(bitmap);
            Assert.AreEqual(392, bitmapValue.Width);
            Assert.AreEqual(243, bitmapValue.Height);
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

                resxdata.WriteToFile();

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
