using System;
using System.Linq;
using NUnit.Framework;

namespace MonoDevelop.ResXEditor.Tests
{
	[TestFixture]
    public class ResXEditorKnownEditorsTests
    {
        string[] defaultKnownTypes = {
            typeof(string).AssemblyQualifiedName,
            typeof(System.Drawing.Icon).AssemblyQualifiedName,
            typeof(System.Drawing.Bitmap).AssemblyQualifiedName,

        };

		[Test]
        [Ignore("No support for loading MD stuff yet")]
        public void CheckRegisteredTypes()
        {
            var types = typeof(ResXEditorKnownEditors).Assembly.GetTypes()
                                                      .Where(x => x.IsSubclassOf(typeof(ResXEditorBinding)) && !x.IsAbstract);

            foreach (var type in types)
            {
                var editorBinding = (ResXEditorBinding)Activator.CreateInstance(type);
                ResXEditorKnownEditors.RegisterKnownTypes(editorBinding.TypesHandled);
			}

            foreach (var type in defaultKnownTypes) {
				Assert.IsTrue(ResXEditorKnownEditors.IsKnownType(type));
            }

            foreach (var type in types)
            {
                var editorBinding = (ResXEditorBinding)Activator.CreateInstance(type);
                ResXEditorKnownEditors.UnregisterKnownTypes(editorBinding.TypesHandled);
            }

            foreach (var type in defaultKnownTypes) {
                Assert.IsFalse(ResXEditorKnownEditors.IsKnownType(type));
            }
        }

		[Test]
		public void CheckRefCounting()
		{
            ResXEditorKnownEditors.RegisterKnownTypes(new[] { typeof(string) });
        	ResXEditorKnownEditors.RegisterKnownTypes(new[] { typeof(string) });

			Assert.IsTrue(ResXEditorKnownEditors.IsKnownType(typeof(string).AssemblyQualifiedName));
            ResXEditorKnownEditors.UnregisterKnownTypes(new[] { typeof(string) });
            Assert.IsTrue(ResXEditorKnownEditors.IsKnownType(typeof(string).AssemblyQualifiedName));
			ResXEditorKnownEditors.UnregisterKnownTypes(new[] { typeof(string) });
            Assert.IsFalse(ResXEditorKnownEditors.IsKnownType(typeof(string).AssemblyQualifiedName));
		}
    }
}
