using System.Windows.Forms;

namespace ToolSL
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public string Login => textBox1.Text;
        public string Password => textBox2.Text;
    }
}
