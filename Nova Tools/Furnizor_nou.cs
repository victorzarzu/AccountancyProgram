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
using System.IO;
using System.Security.Principal;

namespace Nova_Tools
{
    public partial class Furnizor_nou : Form
    {
        public Form parentForm { set; get; }
        SqlConnection conn;
        string nume, adresa, banca, cont_bancar;
        DataTable furnizori;
        DataRow row;
        public Furnizor_nou(DataTable f)
        {
            InitializeComponent();

            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(path.Length - 9);
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);

            furnizori = f;
        }

        private void Furnizor_nou_FormClosed(object sender, FormClosedEventArgs e)
        {
            parentForm.Show();
        }

        void fill_datagridview(DataGridView dgv)
        {
            string[] data = new string[5];
            foreach (DataRow row1 in furnizori.Rows)
            {
                data[0] = row1["Index"].ToString();
                data[1] = row1["Nume"].ToString();
                data[2] = row1["Adresa"].ToString();
                data[3] = row1["Banca"].ToString();
                data[4] = row1["Cont_bancar"].ToString();

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

                            furnizori.DefaultView.Sort = "Nume ASC";
                            furnizori = furnizori.DefaultView.ToTable();

                            fill_datagridview(dgv);
                            return;
                        }

                }
            }
        }

        private void add_button_Click(object sender, EventArgs e)
        {
            conn.Open();

            nume = nume_textbox.Text; adresa = adresa_textbox.Text; banca = banca_textbox.Text; cont_bancar = cont_bancar_textbox.Text;
            if(nume == "")
            {
                MessageBox.Show("Introdu un nume pentru furnizor!");
                conn.Close();
                return;
            }
            SqlCommand query = new SqlCommand("SELECT * FROM Furnizori WHERE Nume = @nume", conn);
            query.Parameters.AddWithValue("@nume", nume);
            SqlDataReader dr = query.ExecuteReader();
            bool exist = dr.HasRows;
            dr.Close(); query.Dispose();

            if(exist)
            {
                MessageBox.Show("Exista deja un furnizor cu acest nume!");
                conn.Close();
                return;
            }

            SqlCommand insert = new SqlCommand("INSERT INTO Furnizori(Nume, Adresa, Banca, Cont_bancar) values(@nume, @adresa, @banca, @cont_bancar)", conn);
            insert.Parameters.AddWithValue("@nume", nume);
            insert.Parameters.AddWithValue("@adresa", adresa);
            insert.Parameters.AddWithValue("@banca", banca);
            insert.Parameters.AddWithValue("@cont_bancar", cont_bancar);

            insert.ExecuteNonQuery();

            query = new SqlCommand("SELECT * FROM Furnizori WHERE Nume = @nume", conn);
            query.Parameters.AddWithValue("@nume", nume);
            dr = query.ExecuteReader();
            dr.Read();
            int index = Convert.ToInt32(dr[0].ToString());

            dr.Close(); query.Dispose();

            row = furnizori.NewRow();

            row["Index"] = index;
            row["Nume"] = nume;
            row["Adresa"] = adresa;
            row["Banca"] = banca;
            row["Cont_bancar"] = cont_bancar;

            furnizori.Rows.Add(row);

            clear_and_fill();

            this.Close();

            conn.Close();
        }

    }
}
