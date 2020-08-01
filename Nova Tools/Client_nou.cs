using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Diagnostics.PerformanceData;
using System.Net.WebSockets;
using System.Data.SqlClient;
using System.IO;

namespace Nova_Tools
{
    public partial class Client_nou : Form
    {
        public Form parentForm {set; get;}
        string nume, adresa, banca, cont_bancar, cui, numar_inregistrare;
        SqlConnection conn;
        DataTable clienti;


        DataRow row;

        private void Client_nou_Load(object sender, EventArgs e)
        {

        }

        public Client_nou(DataTable c)
        {
            InitializeComponent();
            clienti = c;

            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"\SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);
        }

        void fill_datagridview(DataGridView dgv)
        {
            string[] data = new string[6];
            foreach (DataRow row1 in clienti.Rows)
            {
                data[0] = row1["Index"].ToString();
                data[1] = row1["Nume"].ToString();
                data[2] = row1["Adresa"].ToString();
                data[3] = row1["CUI"].ToString();
                data[4] = row1["Numar_de_inregistrare"].ToString();
                data[5] = row1["Banca"].ToString();
                data[6] = row1["Cont_bancar"].ToString();

                dgv.Rows.Add(data);
            }
        }

        void clear_and_fill()
        {
            foreach (Control c in parentForm.Controls)
            {
                if (c.GetType() == typeof(TableLayoutPanel))
                {
                    foreach (Control c1 in c.Controls)
                        if (c1.GetType() == typeof(DataGridView))
                        {
                            DataGridView dgv = (DataGridView)c1;
                            dgv.Rows.Clear();

                            clienti.DefaultView.Sort = "Nume ASC";
                            clienti = clienti.DefaultView.ToTable();

                            fill_datagridview(dgv);
                            return;
                        }

                }
            }
        }

        private void Client_nou_FormClosed(object sender, FormClosedEventArgs e)
        {
            parentForm.Show();
        }

        private void add_button_Click(object sender, EventArgs e)
        {
            conn.Open();

            nume = nume_textbox.Text; adresa = adresa_textbox.Text; banca = banca_textbox.Text; cont_bancar = cont_bancar_textbox.Text; cui = cui_textbox.Text; numar_inregistrare = numar_inregistrare_textbox.Text;
            if (nume == "")
            {
                MessageBox.Show("Introdu un nume pentru client!");
                conn.Close();
                return;
            }

            SqlCommand query = new SqlCommand("SELECT * FROM Clienti WHERE Nume = @nume", conn);
            query.Parameters.AddWithValue("@nume", nume);
            SqlDataReader dr = query.ExecuteReader();

            bool exist = dr.HasRows;
            dr.Close(); query.Dispose();

            if (exist)
            {
                MessageBox.Show("Exista deja un client cu acest nume!");
                conn.Close();
                return;
            }
            else
            {
                SqlCommand insert = new SqlCommand("INSERT INTO Clienti(Nume, Adresa, CUI, Numar_de_inregistrare, Banca, Cont_bancar) values(@nume, @adresa, @cui, @numar_inregistrare, @banca, @cont_bancar)", conn);
                insert.Parameters.AddWithValue("@nume", nume);
                insert.Parameters.AddWithValue("@adresa", adresa);
                insert.Parameters.AddWithValue("@cui", cui);
                insert.Parameters.AddWithValue("@numar_inregistrare", numar_inregistrare);
                insert.Parameters.AddWithValue("banca", banca);
                insert.Parameters.AddWithValue("cont_bancar", cont_bancar);

                insert.ExecuteNonQuery();
                insert.Dispose();

                query = new SqlCommand("SELECT * FROM Clienti WHERE Nume = @nume", conn);
                query.Parameters.AddWithValue("@nume", nume);
                dr = query.ExecuteReader();

                dr.Read();
                int index = Convert.ToInt32(dr[0].ToString());
                dr.Close(); query.Dispose();

                row = clienti.NewRow();

                row["Index"] = index;
                row["Nume"] = nume;
                row["Adresa"] = adresa;
                row["CUI"] = cui;
                row["Numar_de_inregistrare"] = numar_inregistrare;
                row["Banca"] = banca;
                row["Cont_bancar"] = cont_bancar;

                clienti.Rows.Add(row);

                clear_and_fill();

                this.Close();
            }

            conn.Close();
        }
    }
}
