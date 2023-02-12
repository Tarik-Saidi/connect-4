// File Name:     StarForm.cs
// By:            Saidi Tarik
// Date:          16, 09, 2022
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnectFour.Forms
{
    public partial class StarForm : Form
    {
        public StarForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm m2 = new MainForm();
            m2.ShowDialog();
            this.Close();
        }

        private void StarForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
