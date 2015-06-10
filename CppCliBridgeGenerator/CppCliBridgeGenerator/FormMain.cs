using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace CppCliBridgeGenerator
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            this.textBoxOutputFolder.Text = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? "", "output\\");
        }

        /// <summary>
        /// Close app action
        /// </summary>
        private void CloseApp(object sender, EventArgs e)
        {
            Close();
        }



        #region "Add assembly" methods

        /// <summary>
        /// Load .NET assembly and add it to tree
        /// </summary>
        /// <param name="file">.NET assembly file</param>
        private void AddAssembly(String file)
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFile(file); // try to load for reflection
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            // add root node
            TreeNode nodeAssembly = new TreeNode() { Text = assembly.FullName, ImageKey = "Assembly.png", SelectedImageKey = "Assembly.png", Tag = assembly };
            this.treeViewAssemblies.Nodes.Add(nodeAssembly);

            nodeAssembly.Expand(); // immediately expand this root node

            var types = assembly.GetTypes().Where(t => t.IsPublic).OrderBy(t => t.FullName); // get public types from assembly and order them

            // add all enums and classes/structs
            foreach (var type in types)
            {
                if (type.IsEnum)
                    AddAssembly_enum(nodeAssembly, type);
                else
                    AddAssembly_class(nodeAssembly, type);
            }
        }

        /// <summary>
        /// Add nodes to tree for classes
        /// </summary>
        /// <param name="nodeAssembly">Root node with assembly</param>
        /// <param name="type">Class type</param>
        private void AddAssembly_class(TreeNode nodeAssembly, Type type)
        {
            // icon - class / struct
            string img = "Class.png";
            if (!type.IsClass) img = "Struct.png";

            // create node and add it to root node
            TreeNode nodeType = new TreeNode() { Text = type.FullName, ImageKey = img, SelectedImageKey = img, Tag = type };
            nodeAssembly.Nodes.Add(nodeType);


            // get and add child nodes for constructors
            var ctors = type.GetConstructors().OrderBy(m => m.Attributes);

            foreach (var ctor in ctors)
            {
                if (ctor.DeclaringType != type) continue; // INFO: skip inherited methods - could be controlled via checkbox

                nodeType.Nodes.Add(new TreeNode()
                {
                    Text = ctor.DeclaringType.Name
                           + "("
                           + string.Join(", ", ctor.GetParameters().Select(par => par.ParameterType.Name + " " + par.Name).ToArray())
                           + ")",
                    ToolTipText = ctor.ToString(),
                    ImageKey = "Constructor.png",
                    SelectedImageKey = "Constructor.png",
                    Tag = ctor
                });
            }


            // get and add child nodes for fields (class variables)
            var fields = type.GetFields().OrderBy(m => m.Name);

            foreach (var field in fields)
            {
                img = "Field.png";
                if (field.IsStatic) img = "FieldStatic.png";

                nodeType.Nodes.Add(new TreeNode()
                {
                    Text = field.Name
                           + " : "
                           + field.FieldType.Name,
                    ToolTipText = field.ToString(),
                    ImageKey = img,
                    SelectedImageKey = img,
                    Tag = field
                });
            }

            // get and add child nodes for methods
            var methods = type.GetMethods()
                              .OrderBy(m => !m.IsSpecialName).ThenBy(m => m.Name);

            foreach (var method in methods)
            {
                if (method.DeclaringType != type) continue; // INFO: skip inherited methods - could be controlled via checkbox
                if (method.IsSpecialName && method.DeclaringType.GetProperties().FirstOrDefault(p => p.GetSetMethod() == method) == null) continue; // INFO: skip event handlers (but leave property methods)
                if (method.Name == "Dispose") continue; // INFO: skip dispose method - not available in C++/CLI (use delete instead, if needed)

                img = "Method.png";
                if (method.IsStatic) img = "MethodStatic.png";
                else if (method.IsSpecialName) img = "Field.png";

                nodeType.Nodes.Add(new TreeNode()
                {
                    Text = method.Name
                           + "("
                           + String.Join(", ", method.GetParameters().Select(par => par.ParameterType.Name + " " + par.Name).ToArray())
                           + ") : "
                           + method.ReturnType.Name,
                    ToolTipText = (method.IsStatic ? "static " : "") + method.ToString(),
                    ImageKey = img,
                    SelectedImageKey = img,
                    Tag = method
                });
            }
        }

        /// <summary>
        /// Add nodes to tree for enums
        /// </summary>
        /// <param name="nodeAssembly">Root node with assembly</param>
        /// <param name="type">Enum type</param>
        private void AddAssembly_enum(TreeNode nodeAssembly, Type type)
        {
            TreeNode nodeType = new TreeNode() { Text = type.FullName, ImageKey = "Enum.png", SelectedImageKey = "Enum.png", Tag = type };
            nodeAssembly.Nodes.Add(nodeType);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                nodeType.Nodes.Add(new TreeNode()
                {
                    Text = field.Name
                           + " = "
                           + Convert.ChangeType(field.GetValue(null), typeof(ulong)),
                    ToolTipText = field.ToString(),
                    ImageKey = "EnumValue.png",
                    SelectedImageKey = "EnumValue.png",
                    Tag = field
                });
            }
        }

        #endregion



        #region TreeView Events

        /// <summary>
        /// DrawNode overhaul - drawing node text - everything after ( : = draw using gray color
        /// </summary>
        private void treeViewAssemblies_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Color color = (e.State & TreeNodeStates.Focused) != 0 ? SystemColors.HighlightText : SystemColors.WindowText;

            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;


            int begin = e.Node.Text.IndexOf("(");
            if (begin == -1) begin = e.Node.Text.IndexOf(" : ");
            if (begin == -1) begin = e.Node.Text.IndexOf(" = ");

            if (e.Node.Level == 2 && begin > 0)
            {
                Color color2 = (e.State & TreeNodeStates.Focused) != 0 ? ControlPaint.Dark(color, 0f) : ControlPaint.Light(color, 1f);

                String method = e.Node.Text.Substring(0, begin);
                String past = e.Node.Text.Substring(begin);

                Size s = TextRenderer.MeasureText(e.Graphics, method, this.Font, e.Bounds.Size, flags);

                Rectangle pos = e.Bounds;
                pos.Offset(s.Width - 7, 0);

                TextRenderer.DrawText(e.Graphics, method, this.Font, e.Bounds, color, flags);
                TextRenderer.DrawText(e.Graphics, past, this.Font, pos, color2, flags);
            }
            else
            {
                TextRenderer.DrawText(e.Graphics, e.Node.Text, this.Font, e.Bounds, color, flags);
            }
        }

        /// <summary>
        /// AfterCheck overhaul - dynamic childnodes un/checking
        /// </summary>
        private void treeViewAssemblies_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown) return;

            // create action for checking all childnodes recursively
            Action<bool, TreeNode> checkChilds = null;
            checkChilds = delegate(bool check, TreeNode node)
            {
                node.Checked = check;
                foreach (TreeNode child in node.Nodes) checkChilds(check, child);
            };

            // call this action
            if (e.Node.ImageKey == "EnumValue.png") checkChilds(e.Node.Checked, e.Node.Parent);
            else checkChilds(e.Node.Checked, e.Node);

            // check parents
            if (e.Node.Checked)
            {
                var parent = e.Node.Parent;

                while (parent != null && !parent.Checked)
                {
                    parent.Checked = true;
                    parent = parent.Parent;
                }
            }
            else if (e.Node.Level == 1) // uncheck root, if no child is checked 
            {
                bool checkRoot = e.Node.Parent.Nodes.OfType<TreeNode>().Any(n => n.Checked);
                e.Node.Parent.Checked = checkRoot;
            }
        }

        /// <summary>
        /// Tree drag and drop - enter action - copy symbol
        /// </summary>
        private void treeViewAssemblies_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// Tree drag and drop - drop action - add assemblies
        /// </summary>
        private void treeViewAssemblies_DragDrop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[]; // get all dropped files (or other junk)
            if (files == null) return;

            foreach (string file in files)
            {
                if (!File.Exists(file)) continue; // accept only existing files
                AddAssembly(file);
            }
        }

        #endregion



        #region Form buttons

        /// <summary>
        /// Add assembly button - opens dialog for selecting file
        /// </summary>
        private void buttonAddAssembly_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            foreach (var file in this.openFileDialog1.FileNames)
            {
                AddAssembly(file);
            }
        }

        /// <summary>
        /// Remove assembly button - removes assembly from tree (assembly node, ie root node, must be selected)
        /// </summary>
        private void buttonRemoveAssembly_Click(object sender, EventArgs e)
        {
            if (this.treeViewAssemblies.SelectedNode != null && this.treeViewAssemblies.SelectedNode.Parent == null)
            {
                this.treeViewAssemblies.SelectedNode.Remove();
            }
            else
            {
                MessageBox.Show("Please select assembly (root node in tree).");
            }
        }

        /// <summary>
        /// Change output folder button - opens dialog for selecting folder
        /// </summary>
        private void buttonOutputFolderBrowse_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;

            this.textBoxOutputFolder.Text = this.folderBrowserDialog1.SelectedPath;
        }


        /// <summary>
        /// Generate button - filter selected items in tree and triggers all generators.
        /// </summary>
        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(this.textBoxOutputFolder.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("The entered output folder path is invalid.\nPlease choose valid path and try again.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            try
            {
                var generatorChain = new List<Generator>();

                // prepare all generators
                generatorChain.Add(new WrapperMockupGenerator(this.textBoxOutputFolder.Text, generatorChain)); // fixes missing files (forces to generate empty mockups for them)
                generatorChain.Add(new WrapperHeaderGenerator(this.textBoxOutputFolder.Text)); // generates main header file, which contains DLL export
                generatorChain.Add(new WrapperSourceGenerator(this.textBoxOutputFolder.Text)); // generates source file for bridge
                generatorChain.Add(new WrapperILBridgeGenerator(this.textBoxOutputFolder.Text)); // generates IL bridge for general classes/structs
                generatorChain.Add(new WrapperILDelegateGenerator(this.textBoxOutputFolder.Text)); // generates IL bridge for delegates
                generatorChain.Add(new WrapperProjectGenerator(this.textBoxOutputFolder.Text, generatorChain)); // generates vcxproj file


                // traverse assemblies in tree
                foreach (TreeNode nodeAssembly in this.treeViewAssemblies.Nodes)
                {
                    if (!nodeAssembly.Checked) continue;

                    // call AssemblyLoad
                    generatorChain.ForEach(g => g.AssemblyLoad((Assembly) nodeAssembly.Tag));

                    // traverse items in assemblies
                    foreach (TreeNode nodeType in nodeAssembly.Nodes)
                    {
                        if (!nodeType.Checked) continue;

                        var type = (Type) nodeType.Tag;

                        // call EnumLoad or ClassLoad function on generators
                        if (type.IsEnum)
                        {
                            var chcked = nodeType.Nodes.OfType<TreeNode>().Where(n => n.Checked);
                            var fields = chcked.Where(n => n.Tag is FieldInfo).Select(n => n.Tag).Cast<FieldInfo>().ToArray(); // get checked enum fields (though UI allows to select only all or none)

                            // call EnumLoad
                            generatorChain.ForEach(g => g.EnumLoad(type, fields));
                        }
                        else
                        {
                            var chcked = nodeType.Nodes.OfType<TreeNode>().Where(n => n.Checked).ToList(); // get checked parts of class/struct

                            var fields = chcked.Where(n => n.Tag is FieldInfo).Select(n => n.Tag).Cast<FieldInfo>().ToArray(); // get fields
                            var ctors = chcked.Where(n => n.Tag is ConstructorInfo).Select(n => n.Tag).Cast<ConstructorInfo>().ToArray(); // get constructors
                            var methods = chcked.Where(n => n.Tag is MethodInfo).Select(n => n.Tag).Cast<MethodInfo>().ToArray(); // get methods

                            // call ClassLoad
                            generatorChain.ForEach(g => g.ClassLoad(type, fields, ctors, methods));
                        }
                    }
                }

                // call GeneratorFinalize
                generatorChain.ForEach(g => g.GeneratorFinalize());

                MessageBox.Show("Generation succesfully done!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Sorry, an error occured during generation.\nFeel free to contact the author to solve this issue.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// About menu item - shows info dialog
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormInfo().ShowDialog();
        }

        #endregion
    }
}