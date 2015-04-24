using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ManagedToNativeWrapperGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.textBoxOutputFolder.Text = @"c:\Users\Uzivatel\Documents\GitHub\cpp_net_interop\ManagedToNativeWrapperGenerator\TestDir\output\";
            this.addAssembly(@"c:\Users\Uzivatel\Documents\GitHub\cpp_net_interop\ManagedToNativeWrapperGenerator\TestDir\Arrays.dll");
        }


        private void addAssembly(String file)
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFile(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


            TreeNode nodeAssembly = new TreeNode() { Text = assembly.FullName, ImageKey = "Assembly.png", SelectedImageKey = "Assembly.png", Tag = assembly };
            this.treeViewAssemblies.Nodes.Add(nodeAssembly);

            nodeAssembly.Expand();

            var types = assembly.GetTypes().Where(t => t.IsPublic).OrderBy(t => t.FullName);

            foreach (var type in types)
            {
                if (type.IsClass) this.addAssembly_class(nodeAssembly, type);
                else if (type.IsEnum) this.addAssembly_enum(nodeAssembly, type);                                                   
            }    

        }

        private void addAssembly_class(TreeNode nodeAssembly, Type type)
        {
            TreeNode nodeType = new TreeNode() { Text = type.FullName, ImageKey = "Class.png", SelectedImageKey = "Class.png", Tag = type };
            nodeAssembly.Nodes.Add(nodeType);

            var ctors = type.GetConstructors().OrderBy(m => m.Attributes);

            foreach (var ctor in ctors)
            {
                if (ctor.DeclaringType != type) continue;

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


            var fields = type.GetFields().OrderBy(m => m.Name);

            foreach (var field in fields)
            {
                String img = "Field.png";
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


            var methods = type.GetMethods()
                              .OrderBy(m => !m.IsSpecialName).ThenBy(m => m.Name);

            foreach (var method in methods)
            {
                if (method.DeclaringType != type) continue; // TODO: checkbox ?

                String img = "Method.png";
                if (method.IsStatic) img = "MethodStatic.png";
                else if (method.IsSpecialName) img = "Field.png";


                nodeType.Nodes.Add(new TreeNode()
                {
                    Text = method.Name
                            + "("
                            + string.Join(", ", method.GetParameters().Select(par => par.ParameterType.Name + " " + par.Name).ToArray())
                            + ") : "
                            + method.ReturnType.Name,

                    ToolTipText = (method.IsStatic ? "static " : "") + method.ToString(),

                    ImageKey = img,
                    SelectedImageKey = img,
                    Tag = method
                });
            }

        }

        private void addAssembly_enum(TreeNode nodeAssembly, Type type)
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


        #region TreeView Events

        // Drawing node text - everything after "(" draw using gray color
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

                Size s = TextRenderer.MeasureText(e.Graphics, method, Font, e.Bounds.Size, flags);

                Rectangle pos = e.Bounds;
                pos.Offset(s.Width - 7, 0);

                TextRenderer.DrawText(e.Graphics, method, Font, e.Bounds, color, flags);
                TextRenderer.DrawText(e.Graphics, past, Font, pos, color2, flags);
            }
            else
            {                              
                TextRenderer.DrawText(e.Graphics, e.Node.Text, Font, e.Bounds, color, flags);
            }
        }

        // Dynamic nodes un/checking
        private void treeViewAssemblies_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown) return;

            // check all child nodes recursively
            Action<bool, TreeNode> checkChilds = null;
            checkChilds = delegate(bool check, TreeNode node)
            {
                node.Checked = check;
                foreach (TreeNode child in node.Nodes) checkChilds(check, child);
            };

            if (e.Node.ImageKey == "EnumValue.png") checkChilds(e.Node.Checked, e.Node.Parent);
            else checkChilds(e.Node.Checked, e.Node);


            // check parents
            if (e.Node.Checked)
            {
                TreeNode parent = e.Node.Parent;

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

        #endregion


        #region Form buttons

        private void buttonAddAssembly_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            foreach (var file in this.openFileDialog1.FileNames)
            {
                this.addAssembly(file);
            }
        }

        private void buttonRemoveAssembly_Click(object sender, EventArgs e)
        {
            if (treeViewAssemblies.SelectedNode.Parent == null)
            {
                treeViewAssemblies.SelectedNode.Remove();
            }
        }

        private void buttonOutputFolderBrowse_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;

            this.textBoxOutputFolder.Text = this.folderBrowserDialog1.SelectedPath;
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            List<TypeGenerator> generatorChain = new List<TypeGenerator>();

            generatorChain.Add(new WrapperMockupGenerator(this.textBoxOutputFolder.Text, generatorChain)); // fixes missing files
            generatorChain.Add(new WrapperHeaderGenerator(this.textBoxOutputFolder.Text));
            generatorChain.Add(new WrapperSourceGenerator(this.textBoxOutputFolder.Text));
            generatorChain.Add(new WrapperILBridgesGenerator(this.textBoxOutputFolder.Text));
            generatorChain.Add(new WrapperProjectGenerator(this.textBoxOutputFolder.Text, generatorChain));

            foreach (TreeNode nodeAssembly in treeViewAssemblies.Nodes)
            {
                if (!nodeAssembly.Checked) continue;

                // call AssemblyLoad
                generatorChain.ForEach(g => g.AssemblyLoad((Assembly) nodeAssembly.Tag));

                foreach (TreeNode nodeType in nodeAssembly.Nodes)
                {
                    if (!nodeType.Checked) continue;

                    var type = (Type) nodeType.Tag;

                    if (type.IsClass)
                    {
                        var chcked = nodeType.Nodes.OfType<TreeNode>().Where(n => n.Checked);

                        var fields = chcked.Where(n => n.Tag is FieldInfo).Select(n => n.Tag).Cast<FieldInfo>().ToArray();
                        var ctors = chcked.Where(n => n.Tag is ConstructorInfo).Select(n => n.Tag).Cast<ConstructorInfo>().ToArray();
                        var methods = chcked.Where(n => n.Tag is MethodInfo).Select(n => n.Tag).Cast<MethodInfo>().ToArray();

                        // call ClassLoad
                        generatorChain.ForEach(g => g.ClassLoad(type, fields, ctors, methods));

                    }
                    else if (type.IsEnum)
                    {
                        var chcked = nodeType.Nodes.OfType<TreeNode>().Where(n => n.Checked);
                        var fields = chcked.Where(n => n.Tag is FieldInfo).Select(n => n.Tag).Cast<FieldInfo>().ToArray();

                        // call EnumLoad
                        generatorChain.ForEach(g => g.EnumLoad(type, fields));
                    }
                }   

            }


            // call GeneratorFinalize
            generatorChain.ForEach(g => g.GeneratorFinalize());


            MessageBox.Show("Done");
        }

        #endregion


    }
}
