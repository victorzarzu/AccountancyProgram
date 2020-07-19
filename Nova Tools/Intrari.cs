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
    public partial class Intrari : Form
    {
        public Intrari()
        {
            InitializeComponent();
        }

        private void creeaza_button_Click(object sender, EventArgs e)
        {
            Creeaza_intrare creeaza_intrare = new Creeaza_intrare();
            creeaza_intrare.parentForm = this;
            this.Hide();
            creeaza_intrare.Show();
        }

        private void plati_button_Click(object sender, EventArgs e)
        {
            Plati plati = new Plati();
            plati.parentForm = this;
            plati.Show();
            this.Hide();
        }

        private void produse_button_Click(object sender, EventArgs e)
        {
            Produse produse = new Produse();
            produse.parentForm = this;
            produse.Show();
            this.Hide();
        }
    }
}
