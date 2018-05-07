using System.Windows.Forms;

namespace ToolSL
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public void SetData(string login = "", string password = "")
        {
            textBox1.Text = login;
            textBox2.Text = password;
        }

        public string Login => textBox1.Text;
        public string Password => textBox2.Text;

        private const int WS_SYSMENU = 0x80000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~WS_SYSMENU;
                return cp;
            }
        }

        private void showHideButton_Click(object sender, System.EventArgs e)
        {
            if (showHideButton.Text == "show")
            {
                textBox2.PasswordChar = '\0';
                showHideButton.Text = "hide";
            }
            else
            {
                textBox2.PasswordChar = '*';
                showHideButton.Text = "show";
            }
        }
    }
}
