using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PowerShellRunner.Form1;

namespace PowerShellRunner
{
    public partial class Form2 : Form
    {
        public Form2(List<ScriptResource> scriptResources)
        {
            InitializeComponent();
            dataGridView1.DataSource = scriptResources.Select(sr => new
            {
                sr.Id,
                sr.ResourceId,
                sr.ScriptContent
            }).ToList();
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
