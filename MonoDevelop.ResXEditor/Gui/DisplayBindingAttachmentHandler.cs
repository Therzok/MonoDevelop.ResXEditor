using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.ResXEditor
{
    class DisplayBindingAttachmentHandler : CommandHandler
    {
        const string ResXEditorsExtensionPath = "/MonoDevelop/ResXEditor/ResXEditors";
        protected override void Run()
        {
            IdeApp.Workbench.DocumentOpened += HandleDocumentOpened;

            AddinManager.AddExtensionNodeHandler(ResXEditorsExtensionPath, ExtensionNodesChanged);
        }

        static void ExtensionNodesChanged(object sender, ExtensionNodeEventArgs args)
        {
            var binding = (ResXEditorBinding)args.ExtensionObject;
            if (args.Change == ExtensionChange.Add)
                ResXEditorKnownEditors.RegisterKnownTypes(binding.TypesHandled);
            else
                ResXEditorKnownEditors.UnregisterKnownTypes(binding.TypesHandled);
        }

        static void HandleDocumentOpened(object sender, DocumentEventArgs e)
        {
            var document = e.Document;
            if (document == null ||
                !document.IsFile ||
                !".resx".Equals(document.FileName.Extension, StringComparison.OrdinalIgnoreCase) ||
                DesktopService.GetMimeTypeForUri(document.FileName) != "text/microsoft-resx")
                return;

            // Load resx data.
            List<ResXData> resx;
            if (document.HasProject) {
                resx = ResXData.FromProjectFile (document.Project.Files.GetFile (document.FileName)).ToList ();
            } else {
                resx = new List<ResXData> {
                    ResXData.FromFile (document.FileName)
                };;
            }
            var mainResx = resx [0];

            int index = 0;
            foreach (var editor in AddinManager.GetExtensionObjects<ResXEditorBinding>(ResXEditorsExtensionPath))
            {
                var viewContent = editor.CreateViewContent(resx, mainResx);
                document.Window.InsertViewContent(index++, viewContent);
				viewContent.Project = document.Project;
			}
        }
    }
}
