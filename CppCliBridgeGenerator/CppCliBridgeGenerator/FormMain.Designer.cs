using System.ComponentModel;
using System.Windows.Forms;

namespace CppCliBridgeGenerator
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.buttonAddAssembly = new System.Windows.Forms.Button();
            this.buttonRemoveAssembly = new System.Windows.Forms.Button();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxOutputFolder = new System.Windows.Forms.TextBox();
            this.buttonOutputFolderBrowse = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAssemblyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAssemblyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeViewAssemblies = new CppCliBridgeGenerator.FixedTreeView();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAddAssembly
            // 
            this.buttonAddAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddAssembly.Location = new System.Drawing.Point(495, 27);
            this.buttonAddAssembly.Name = "buttonAddAssembly";
            this.buttonAddAssembly.Size = new System.Drawing.Size(115, 23);
            this.buttonAddAssembly.TabIndex = 1;
            this.buttonAddAssembly.Text = "Add assembly";
            this.buttonAddAssembly.UseVisualStyleBackColor = true;
            this.buttonAddAssembly.Click += new System.EventHandler(this.buttonAddAssembly_Click);
            // 
            // buttonRemoveAssembly
            // 
            this.buttonRemoveAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemoveAssembly.Location = new System.Drawing.Point(495, 56);
            this.buttonRemoveAssembly.Name = "buttonRemoveAssembly";
            this.buttonRemoveAssembly.Size = new System.Drawing.Size(115, 23);
            this.buttonRemoveAssembly.TabIndex = 2;
            this.buttonRemoveAssembly.Text = "Remove assembly";
            this.buttonRemoveAssembly.UseVisualStyleBackColor = true;
            this.buttonRemoveAssembly.Click += new System.EventHandler(this.buttonRemoveAssembly_Click);
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGenerate.Location = new System.Drawing.Point(495, 503);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(115, 23);
            this.buttonGenerate.TabIndex = 3;
            this.buttonGenerate.Text = "Generate";
            this.buttonGenerate.UseVisualStyleBackColor = true;
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 489);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Output folder:";
            // 
            // textBoxOutputFolder
            // 
            this.textBoxOutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutputFolder.Location = new System.Drawing.Point(12, 505);
            this.textBoxOutputFolder.Name = "textBoxOutputFolder";
            this.textBoxOutputFolder.Size = new System.Drawing.Size(414, 20);
            this.textBoxOutputFolder.TabIndex = 5;
            // 
            // buttonOutputFolderBrowse
            // 
            this.buttonOutputFolderBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOutputFolderBrowse.Location = new System.Drawing.Point(432, 503);
            this.buttonOutputFolderBrowse.Name = "buttonOutputFolderBrowse";
            this.buttonOutputFolderBrowse.Size = new System.Drawing.Size(57, 23);
            this.buttonOutputFolderBrowse.TabIndex = 6;
            this.buttonOutputFolderBrowse.Text = "Browse";
            this.buttonOutputFolderBrowse.UseVisualStyleBackColor = true;
            this.buttonOutputFolderBrowse.Click += new System.EventHandler(this.buttonOutputFolderBrowse_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "_empty.png");
            this.imageList1.Images.SetKeyName(1, "Assembly.png");
            this.imageList1.Images.SetKeyName(2, "NameSpace.png");
            this.imageList1.Images.SetKeyName(3, "Class.png");
            this.imageList1.Images.SetKeyName(4, "Constructor.png");
            this.imageList1.Images.SetKeyName(5, "Delegate.png");
            this.imageList1.Images.SetKeyName(6, "Enum.png");
            this.imageList1.Images.SetKeyName(7, "EnumValue.png");
            this.imageList1.Images.SetKeyName(8, "Field.png");
            this.imageList1.Images.SetKeyName(9, "FieldReadOnly.png");
            this.imageList1.Images.SetKeyName(10, "FieldStatic.png");
            this.imageList1.Images.SetKeyName(11, "Interface.png");
            this.imageList1.Images.SetKeyName(12, "Literal.png");
            this.imageList1.Images.SetKeyName(13, "Method.png");
            this.imageList1.Images.SetKeyName(14, "MethodStatic.png");
            this.imageList1.Images.SetKeyName(15, "PInvokeMethod.png");
            this.imageList1.Images.SetKeyName(16, "StaticClass.png");
            this.imageList1.Images.SetKeyName(17, "Struct.png");
            this.imageList1.Images.SetKeyName(18, "VirtualMethod.png");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 8;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(622, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAssemblyToolStripMenuItem,
            this.removeAssemblyToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // addAssemblyToolStripMenuItem
            // 
            this.addAssemblyToolStripMenuItem.Name = "addAssemblyToolStripMenuItem";
            this.addAssemblyToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.addAssemblyToolStripMenuItem.Text = "Add assembly";
            this.addAssemblyToolStripMenuItem.Click += new System.EventHandler(this.buttonAddAssembly_Click);
            // 
            // removeAssemblyToolStripMenuItem
            // 
            this.removeAssemblyToolStripMenuItem.Name = "removeAssemblyToolStripMenuItem";
            this.removeAssemblyToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.removeAssemblyToolStripMenuItem.Text = "Remove assembly";
            this.removeAssemblyToolStripMenuItem.Click += new System.EventHandler(this.buttonRemoveAssembly_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(166, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.CloseApp);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.aboutToolStripMenuItem.Text = "About and usage";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // treeViewAssemblies
            // 
            this.treeViewAssemblies.AllowDrop = true;
            this.treeViewAssemblies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewAssemblies.CheckBoxes = true;
            this.treeViewAssemblies.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeViewAssemblies.ImageIndex = 0;
            this.treeViewAssemblies.ImageList = this.imageList1;
            this.treeViewAssemblies.Location = new System.Drawing.Point(12, 27);
            this.treeViewAssemblies.Name = "treeViewAssemblies";
            this.treeViewAssemblies.SelectedImageIndex = 0;
            this.treeViewAssemblies.ShowNodeToolTips = true;
            this.treeViewAssemblies.Size = new System.Drawing.Size(477, 455);
            this.treeViewAssemblies.TabIndex = 7;
            this.treeViewAssemblies.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewAssemblies_AfterCheck);
            this.treeViewAssemblies.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeViewAssemblies_DrawNode);
            this.treeViewAssemblies.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewAssemblies_DragDrop);
            this.treeViewAssemblies.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewAssemblies_DragEnter);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 538);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.treeViewAssemblies);
            this.Controls.Add(this.buttonOutputFolderBrowse);
            this.Controls.Add(this.textBoxOutputFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonGenerate);
            this.Controls.Add(this.buttonRemoveAssembly);
            this.Controls.Add(this.buttonAddAssembly);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(580, 400);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "C++/CLI bridge-wrapper generator";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button buttonAddAssembly;
        private Button buttonRemoveAssembly;
        private Button buttonGenerate;
        private Label label1;
        private TextBox textBoxOutputFolder;
        private Button buttonOutputFolderBrowse;
        private ImageList imageList1;
        private OpenFileDialog openFileDialog1;
        private FolderBrowserDialog folderBrowserDialog1;
        private FixedTreeView treeViewAssemblies;
        private Label label2;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem addAssemblyToolStripMenuItem;
        private ToolStripMenuItem removeAssemblyToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;

    }
}

