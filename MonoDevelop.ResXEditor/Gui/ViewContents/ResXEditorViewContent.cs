using System;
using System.Collections.Generic;
using System.Resources;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.ResXEditor
{
    public abstract class ResXEditorViewContent : AbstractXwtViewContent
    {
        Xwt.ScrollView sw;
        protected ResXEditorViewContent()
        {
        }

        readonly protected HashSet<string> names = new HashSet<string>();
        protected ResXData Data { get; private set; }
        protected DocumentToolbar Toolbar { get; private set; }

        internal ResXEditorViewContent Initialize(ResXData data)
        {
            Data = data;
            OnInitialize(data);
            return this;
        }

        Xwt.MenuItem CreateMenuItem (string label, Type nodeType)
        {
            var mi = new Xwt.MenuItem(label);
            mi.Clicked += (sender, e) => {
                using (var dialog = new Xwt.OpenFileDialog())
                {
                    if (!dialog.Run(sw.ParentWindow))
                        return;
                    Data.Nodes.Add(Data.CreateNode(dialog.FileName, nodeType));
                }
            };

            return mi;
        }

        Xwt.Button CreateAddButton()
        {
            var menu = new Xwt.Menu();
            menu.Items.Add(CreateMenuItem("Image", typeof(System.Drawing.Bitmap)));
            menu.Items.Add(CreateMenuItem("Icon", typeof(System.Drawing.Icon)));

            return new Xwt.MenuButton("Add Resource")
            {
                Menu = menu,
            };;
        }

        Xwt.ComboBox CreateGenerationCombo()
        {
            var generationCombo = new Xwt.ComboBox();
            generationCombo.Items.Add("ResXFileCodeGenerator", "Internal");
            generationCombo.Items.Add("PublicResXFileCodeGenerator", "Public");
            generationCombo.Items.Add("", "No code generation");

            int selectedIndex = generationCombo.Items.IndexOf(Data.ProjectFile.Generator);
            generationCombo.SelectedIndex = selectedIndex != -1 ? selectedIndex : 2;

            generationCombo.SelectionChanged += (sender, e) => Data.ProjectFile.Generator = (string)generationCombo.SelectedItem;
            return generationCombo;
        }

        Xwt.ComboBox CreateLanguageCombo()
        {
            // TODO: Verify why project is null.
            // TODO: Also do this on addition of files.
            // TODO: Handle source editor?
            // TODO: Add functionality.
            var languageCombo = new Xwt.ComboBox();
            languageCombo.Items.Add("", "Neutral");
            if (Project != null)
            {
                foreach (var item in Constants.AllCultures)
                {
                    var pf = Project.Files.GetFile(System.IO.Path.ChangeExtension(Data.Path, item.Name + ".resx"));
                    if (pf == null)
                        continue;

                    languageCombo.Items.Add(item.Name, item.DisplayName);
                }
            }
            languageCombo.SelectedIndex = 0;
            return languageCombo;
        }

        protected virtual void OnToolbarSet()
        {
        }

		protected abstract void OnInitialize(ResXData data);
        protected abstract Xwt.Widget CreateContent();

        protected override void OnWorkbenchWindowChanged()
        {
            base.OnWorkbenchWindowChanged();

            if (WorkbenchWindow != null)
            {
                Toolbar = WorkbenchWindow.GetToolbar(this);
                OnToolbarSet();

                //Toolbar.Insert(new XwtControl(CreateLanguageCombo()), 0);
                Toolbar.Insert(new XwtControl(CreateAddButton()), 0);
                Toolbar.Add(new XwtControl(new Xwt.VSeparator()));
                if (Data.ProjectFile != null)
                    Toolbar.Add(new XwtControl(CreateGenerationCombo()));
            }
        }

        public override Xwt.Widget Widget => sw ?? (sw = new Xwt.ScrollView(CreateContent()) { Visible = true });
    }
}
