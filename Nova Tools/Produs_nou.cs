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

namespace Nova_Tools
{
    public partial class Produs_nou : Form
    {
        public Form parentForm { get; set; }

        SqlConnection conn;
        DataTable produse;
        DataRow row;

        void sort_datatable(DataTable dt, string sort)
        {
            dt.DefaultView.Sort = sort;
            dt = dt.DefaultView.ToTable();
        }

        void fill_comboboxes()
        {
            sort_datatable(produse, "Brand ASC");
            foreach (DataRow row1 in produse.Rows)
                if (!brand_combobox.Items.Contains(row1["Brand"].ToString()))
                    brand_combobox.Items.Add(row1["Brand"].ToString());
            sort_datatable(produse, "Categorie ASC");
            foreach (DataRow row1 in produse.Rows)
                if (!categ_combobox.Items.Contains(row1["Categorie"].ToString()))
                    categ_combobox.Items.Add(row1["Categorie"].ToString());
        }

        void fill_datagridview(DataGridView dgv)
        {
            string[] data = new string[5];
            foreach (DataRow row1 in produse.Rows)
            {
                data[0] = row1["Id"].ToString();
                data[1] = row1["Nume"].ToString();
                data[2] = row1["Cod"].ToString();
                data[3] = row1["Brand"].ToString();
                data[4] = row1["Categorie"].ToString();

                dgv.Rows.Add(data);
            }
        }

        public Produs_nou(DataTable p)
        {
            InitializeComponent();

            produse = p;

            fill_comboboxes();

            um_combobox.SelectedIndex = 0;

        }

        private void Produs_nou_FormClosed(object sender, FormClosedEventArgs e)
        {
            fill_datagridview();
            parentForm.Show();
        }

        void fill_datagridview()
        {
            foreach(Control c in parentForm.Controls)
            {
                if(c.GetType() == typeof(TableLayoutPanel))
                {
                    foreach (Control c1 in c.Controls)
                        if(c1.GetType() == typeof(TableLayoutPanel))
                        {
                            foreach (Control c2 in c1.Controls)
                                if (c2.GetType() == typeof(TableLayoutPanel))
                                {
                                    foreach (Control c3 in c2.Controls)
                                        if(c3.GetType() == typeof(DataGridView))
                                        {
                                            DataGridView dgv = (DataGridView)c3;
                                            dgv.Rows.Clear();

                                            sort_datatable(produse, "Nume ASC");
                                            fill_datagridview(dgv);
                                        }    
                                }
                        }
                }
            }
        }

        private void add_button_Click(object sender, EventArgs e)
        {
            string nume = nume_textbox.Text, cod = cod_textbox.Text, um = um_combobox.SelectedItem.ToString(), brand, categorie;
            if (brand_textbox.Text == "" && brand_combobox.SelectedIndex == -1)
            {
                MessageBox.Show("Introdu un brand!");
                return;
            }
            if (categ_textbox.Text == "" && categ_combobox.SelectedIndex == -1)
            {
                MessageBox.Show("Introdu o categorie");
                return;
            }
            if (brand_textbox.Text != "")
                brand = brand_textbox.Text;
            else
                brand = brand_combobox.SelectedItem.ToString();
            if (categ_textbox.Text != "")
                categorie = categ_textbox.Text;
            else
                categorie = categ_combobox.SelectedItem.ToString();

            conn.Open();

            SqlCommand query = new SqlCommand("SELECT * FROM Produse WHERE Cod = @cod", conn);
            query.Parameters.AddWithValue("@cod", cod);
            SqlDataReader dr = query.ExecuteReader();
            bool exist = dr.HasRows;
            dr.Close(); query.Dispose();

            if(exist)
            {
                MessageBox.Show("Acest produs exista deja in baza de date!");
                conn.Close();
                return;
            }

            SqlCommand insert = new SqlCommand("INSERT INTO Produse(Nume, Cod, Brand, Categorie, UM) values(@nume, @cod, @brand, @categorie, @um)", conn);
            insert.Parameters.AddWithValue("@nume", nume);
            insert.Parameters.AddWithValue("@cod", cod);
            insert.Parameters.AddWithValue("@brand", brand);
            insert.Parameters.AddWithValue("@categorie", categorie);
            insert.Parameters.AddWithValue("@um", um);

            insert.ExecuteNonQuery();


            query = new SqlCommand("SELECT * FROM Produse WHERE Cod = @cod", conn);
            query.Parameters.AddWithValue("@cod", cod);
            dr = query.ExecuteReader(); dr.Read();
            int id = Convert.ToInt32(dr[0].ToString());
            dr.Close(); query.Dispose();

            row = produse.NewRow();

            row["Id"] = id;
            row["Nume"] = nume;
            row["Cod"] = cod;
            row["Brand"] = brand;
            row["Categorie"] = categorie;
            row["UM"] = um;

            produse.Rows.Add(row);
 
            this.Close();
        }

        private void Produs_nou_Load(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"\SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);

            fill_datagridview();
        }
    }
}
