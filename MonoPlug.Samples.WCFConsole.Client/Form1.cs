using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace MonoPlug.Samples.WCFConsole.Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.remoteConsole.Connect("ddoffice", 28001);
        }

        private void remoteConsole_ConsoleMessage(object sender, MonoPlug.ConMessageEventArgs e)
        {
            int start = this.rtb.TextLength;
            string a = e.Message + Environment.NewLine;
            this.rtb.Text += a;
            this.rtb.Select(start, a.Length);
            if (e.HasColor)
            {
                this.rtb.SelectionColor = e.Color;
            }
            if (e.IsDebug)
            {
                this.rtb.SelectionFont = new Font(this.rtb.SelectionFont, FontStyle.Italic);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.remoteConsole.Disconnect();
        }
    }
}
