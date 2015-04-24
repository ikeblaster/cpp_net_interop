using System.Windows.Forms;
using System.Runtime.InteropServices;
using System;

namespace ManagedToNativeWrapperGenerator
{
    class FixedTreeView : TreeView
    {
        protected override void WndProc(ref Message m)
        {
            // Filter WM_LBUTTONDBLCLK
            if (m.Msg != 0x203) base.WndProc(ref m);
        }

    }
}

