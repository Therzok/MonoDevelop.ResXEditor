using System;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;

namespace MonoDevelop.ResXEditor.Gui
{
	class ResXEditorDisplayBinding : IViewDisplayBinding
	{
		public string Name {
			get {
				return "ResX Editor";
			}
		}

		public bool CanUseAsDefault {
			get { return true; }
		}

		//AssemblyBrowserViewContent viewContent = null;

		//internal AssemblyBrowserViewContent GetViewContent ()
		//{
		//	if (viewContent == null || viewContent.IsDisposed) {
		//		viewContent = new AssemblyBrowserViewContent ();
		//		viewContent.Disposed += HandleDestroyed;
		//	}
		//	return viewContent;
		//}

		//void HandleDestroyed (object sender, EventArgs e)
		//{
		//	((AssemblyBrowserViewContent)sender).Disposed -= HandleDestroyed;
		//	this.viewContent = null;
		//}

		public bool CanHandle (FilePath fileName, string mimeType, Project ownerProject)
		{
            return mimeType == "text/microsoft-resx";
		}

		public ViewContent CreateContent (FilePath fileName, string mimeType, Project ownerProject)
		{
			return null;
		}
	}
}
