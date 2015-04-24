namespace ManagedToNativeWrapperGenerator
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonAddAssembly = new System.Windows.Forms.Button();
            this.buttonRemoveAssembly = new System.Windows.Forms.Button();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxOutputFolder = new System.Windows.Forms.TextBox();
            this.buttonOutputFolderBrowse = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.treeViewAssemblies = new ManagedToNativeWrapperGenerator.FixedTreeView();
            this.SuspendLayout();
            // 
            // buttonAddAssembly
            // 
            this.buttonAddAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddAssembly.Location = new System.Drawing.Point(473, 12);
            this.buttonAddAssembly.Name = "buttonAddAssembly";
            this.buttonAddAssembly.Size = new System.Drawing.Size(109, 23);
            this.buttonAddAssembly.TabIndex = 1;
            this.buttonAddAssembly.Text = "Add assembly";
            this.buttonAddAssembly.UseVisualStyleBackColor = true;
            this.buttonAddAssembly.Click += new System.EventHandler(this.buttonAddAssembly_Click);
            // 
            // buttonRemoveAssembly
            // 
            this.buttonRemoveAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemoveAssembly.Location = new System.Drawing.Point(588, 12);
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
            this.buttonGenerate.Location = new System.Drawing.Point(473, 383);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(115, 23);
            this.buttonGenerate.TabIndex = 3;
            this.buttonGenerate.Text = "Generate";
            this.buttonGenerate.UseVisualStyleBackColor = true;
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(470, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Output folder:";
            // 
            // textBoxOutputFolder
            // 
            this.textBoxOutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutputFolder.Location = new System.Drawing.Point(473, 68);
            this.textBoxOutputFolder.Name = "textBoxOutputFolder";
            this.textBoxOutputFolder.Size = new System.Drawing.Size(167, 20);
            this.textBoxOutputFolder.TabIndex = 5;
            // 
            // buttonOutputFolderBrowse
            // 
            this.buttonOutputFolderBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOutputFolderBrowse.Location = new System.Drawing.Point(646, 67);
            this.buttonOutputFolderBrowse.Name = "buttonOutputFolderBrowse";
            this.buttonOutputFolderBrowse.Size = new System.Drawing.Size(57, 21);
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
            // treeViewAssemblies
            // 
            this.treeViewAssemblies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewAssemblies.CheckBoxes = true;
            this.treeViewAssemblies.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeViewAssemblies.ImageIndex = 0;
            this.treeViewAssemblies.ImageList = this.imageList1;
            this.treeViewAssemblies.Location = new System.Drawing.Point(13, 13);
            this.treeViewAssemblies.Name = "treeViewAssemblies";
            this.treeViewAssemblies.SelectedImageIndex = 0;
            this.treeViewAssemblies.ShowNodeToolTips = true;
            this.treeViewAssemblies.Size = new System.Drawing.Size(451, 393);
            this.treeViewAssemblies.TabIndex = 7;
            this.treeViewAssemblies.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewAssemblies_AfterCheck);
            this.treeViewAssemblies.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeViewAssemblies_DrawNode);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 418);
            this.Controls.Add(this.treeViewAssemblies);
            this.Controls.Add(this.buttonOutputFolderBrowse);
            this.Controls.Add(this.textBoxOutputFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonGenerate);
            this.Controls.Add(this.buttonRemoveAssembly);
            this.Controls.Add(this.buttonAddAssembly);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ManagedToNative Wrapper Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAddAssembly;
        private System.Windows.Forms.Button buttonRemoveAssembly;
        private System.Windows.Forms.Button buttonGenerate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxOutputFolder;
        private System.Windows.Forms.Button buttonOutputFolderBrowse;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private FixedTreeView treeViewAssemblies;

    }
}

