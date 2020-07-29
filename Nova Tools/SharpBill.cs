using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Nova_Tools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void firma_button_Click(object sender, EventArgs e)
        {
            Firma firma = new Firma();
            firma.Show();
        }

        private void clienti_button_Click(object sender, EventArgs e)
        {
            Clienti clienti = new Clienti();
            clienti.Show();
        }

        private void furnizori_button_Click(object sender, EventArgs e)
        {
            Furnizori furnizori = new Furnizori();
            furnizori.Show();
        }

        private void intrari_button_Click(object sender, EventArgs e)
        {
            Intrari intrari = new Intrari();
            intrari.Show();
        }

        private void iesiri_button_Click(object sender, EventArgs e)
        {
            Iesiri iesiri = new Iesiri();
            iesiri.Show();
        }

        private void stoc_button_Click(object sender, EventArgs e)
        {
            Stoc stoc = new Stoc();
            stoc.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
