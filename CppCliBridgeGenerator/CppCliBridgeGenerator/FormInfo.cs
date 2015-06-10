using System;
using System.Windows.Forms;

namespace CppCliBridgeGenerator
{
    public partial class FormInfo : Form
    {
        public FormInfo()
        {
            InitializeComponent();
        }
 
        private void CloseApp(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.linkLabel1.Text);
        }

    }
}
