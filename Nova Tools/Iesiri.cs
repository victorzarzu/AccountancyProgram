using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nova_Tools
{
    public partial class Iesiri : Form
    {
        public Iesiri()
        {
            InitializeComponent();
        }

        private void creeaza_button_Click(object sender, EventArgs e)
        {
            Creeaza_iesire creeaza_iesire = new Creeaza_iesire();
            creeaza_iesire.parentForm = this;
            creeaza_iesire.Show();
            this.Hide();
        }

        private void plati_button_Click(object sender, EventArgs e)
        {
            Facturi facturi = new Facturi();
            facturi.parentForm = this;
            facturi.Show();
            this.Hide();
        }

        private void Iesiri_Load(object sender, EventArgs e)
        {

        }
    }
}
