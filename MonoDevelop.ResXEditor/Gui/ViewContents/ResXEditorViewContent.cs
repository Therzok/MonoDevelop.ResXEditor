using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.ResXEditor
{
    public abstract class ResXEditorViewContent : AbstractXwtViewContent
    {
        Xwt.ScrollView sw;

        readonly protected HashSet<string> names = new HashSet<string>();
        protected IList<ResXData> Datas { get; private set; }
        protected DocumentToolbar Toolbar { get; private set; }

        ResXData data;
        protected ResXData Data {
            get => data;
            private set {
                if (data != value) {
                    data = value;
                    OnDataChanged (value);
                }
            }
        }

        protected virtual void OnDataChanged(ResXData data)
        {
        }

        internal ResXEditorViewContent Initialize(List<ResXData> resxData, ResXData mainResx)
        {
			OnInitialize();
            Datas = resxData;
            Data = mainResx;
            return this;
        }

        Xwt.MenuItem CreateMenuItem(string label, Type nodeType, Xwt.FileDialogFilter filter)
        {
            var mi = new Xwt.MenuItem(label);
            mi.Clicked += (sender, e) =>
            {
                using (var dialog = new Xwt.OpenFileDialog())
                {
                    if (filter != null)
                    {
                        dialog.Filters.Add(filter);
                        dialog.ActiveFilter = filter;
                    }

                    if (!dialog.Run(sw.ParentWindow))
                        return;
                    Data.Nodes.Add(Data.CreateNode(dialog.FileName, nodeType));
                    Data.WriteToFile(Data.Nodes);
                }
            };

            return mi;
        }

        Xwt.Button CreateAddButton()
        {
            var menu = new Xwt.Menu();
            menu.Items.Add(CreateMenuItem("Image", typeof(System.Drawing.Bitmap), null));
            menu.Items.Add(CreateMenuItem("Icon", typeof(System.Drawing.Icon), new Xwt.FileDialogFilter("Icon files", "*.ico")));

            return new Xwt.MenuButton("Add Resource")
            {
                Menu = menu,
            };
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
            // TODO: Also do this on addition of files.
            // TODO: Handle source editor?
            // TODO: Add functionality.
            var languageCombo = new Xwt.ComboBox();
            var itemName = Data.Path.Remove (Data.Path.LastIndexOf (".resx", Core.FilePath.PathComparison), ".resx".Length);
            foreach (var item in Constants.AllCultures)
            {
                string langName = string.Empty;
                if (!string.IsNullOrEmpty (item.Name))
                    langName = "." + item.Name;
                
                var pf = Project.Files.GetFile (itemName + langName + ".resx");
                if (pf == null)
                    continue;

                languageCombo.Items.Add(pf, item.DisplayName);
            }
            languageCombo.SelectedItem = Data.ProjectFile;
            return languageCombo;
        }

        protected virtual void OnToolbarSet()
        {
        }

        protected abstract void OnInitialize();
        protected abstract Xwt.Widget CreateContent();

        protected sealed override void OnWorkbenchWindowChanged()
        {
            base.OnWorkbenchWindowChanged();

            if (WorkbenchWindow != null)
            {
                Toolbar = WorkbenchWindow.GetToolbar(this);
                OnToolbarSet();

                Toolbar.Insert(new XwtControl(CreateAddButton()), 0);
                Toolbar.Add(new XwtControl(new Xwt.VSeparator()));
                if (Data.ProjectFile != null)
                    Toolbar.Add(new XwtControl(CreateGenerationCombo()));
            }
        }

        protected override void OnSetProject(Projects.Project project)
        {
            base.OnSetProject(project);

            Toolbar.Insert(new XwtControl(CreateLanguageCombo()), 0);
        }

        public sealed override Xwt.Widget Widget => sw ?? (sw = new Xwt.ScrollView(CreateContent()) { Visible = true });

		public override Task Save()
		{
			data.WriteToFile(data.Nodes);
			return base.Save();
		}
	}
}
