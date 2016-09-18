using System;
using Mono.Addins;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.ResXEditor
{
	class DisplayBindingAttachmentHandler : CommandHandler
	{
		protected override void Run()
		{
			Ide.IdeApp.Workbench.DocumentOpened += HandleDocumentOpened;
		}

		const string ResXEditorsExtensionPath = "/MonoDevelop/ResXEditor/ResXEditors";
		static void HandleDocumentOpened(object sender, DocumentEventArgs e)
		{
			var document = e.Document;
			if (document == null ||
			    !document.IsFile ||
			    !".resx".Equals(document.FileName.Extension, StringComparison.OrdinalIgnoreCase) ||
			    DesktopService.GetMimeTypeForUri(document.FileName) != "text/microsoft-resx")
				return;

			// Load resx data.
			var resx = new ResXData();

			int index = 0;
			foreach (var editor in AddinManager.GetExtensionObjects<ResXEditorBinding>(ResXEditorsExtensionPath))
			{
				document.Window.InsertViewContent(index++, editor.CreateViewContent(resx));
			}
		}
	}
}
