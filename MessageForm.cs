using System;
using System.Windows.Forms;

namespace ToolSL
{
    public partial class MessageForm : Form
    {
        public MessageForm(string message)
        {
            InitializeComponent();
            textBox.Text = message;
            textBox.SelectionStart = 0;
        }

        private void acceptCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            closeButton.Enabled = acceptCheckBox.Checked;
        }

     
    }
}
