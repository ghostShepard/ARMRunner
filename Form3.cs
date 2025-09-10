
namespace PowerShellRunner
{
    public partial class Form3 : Form
    {
        public string EditedScriptContent => richTextBox1.Text;
        public Form3(string scriptContent)
        {
            InitializeComponent();
            richTextBox1.Text = scriptContent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
