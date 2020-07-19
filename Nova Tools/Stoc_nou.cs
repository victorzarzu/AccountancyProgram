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
    public partial class Stoc_nou : Form
    {
        public Form parentForm { get; set; }

        SqlConnection conn;
        DataTable stoc;
        DataRow row;
        public Stoc_nou(DataTable s)
        {
            InitializeComponent();

            stoc = s;
        }

        void load_comboboxes()
        {
            SqlCommand query = new SqlCommand("SELECT Brand FROM Stoc ORDER BY Brand ASC", conn);
            SqlDataReader dr = query.ExecuteReader();

            while (dr.Read())
                if (!brand_combobox.Items.Contains(dr[0].ToString()))
                    brand_combobox.Items.Add(dr[0].ToString());

            dr.Close(); query.Dispose();

            query = new SqlCommand("SELECT Categorie FROM Stoc ORDER BY Categorie ASC", conn);
            dr = query.ExecuteReader();

            while (dr.Read())
                if (!categorie_combobox.Items.Contains(dr[0].ToString()))
                    categorie_combobox.Items.Add(dr[0].ToString());

            dr.Close(); query.Dispose();
        }

        private void Stoc_nou_Load(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader("connection_string.txt"))
            {
                string connection_string = sr.ReadLine();
                conn = new SqlConnection(connection_string);
            }

            conn.Open();

            load_comboboxes();

            conn.Close();

            tip_discount_combobox.SelectedIndex = um_combobox.SelectedIndex = 0;
            discount_textbox.Text = "0.00";
        }

        private void Stoc_nou_FormClosed(object sender, FormClosedEventArgs e)
        {
            parentForm.Show();
        }

        void fill_datagridview(DataGridView dgv)
        {
            string[] data = new string[8];

            stoc.DefaultView.Sort = "Nume asc";
            stoc = stoc.DefaultView.ToTable();

            foreach(DataRow row1 in stoc.Rows)
            {
                data[0] = row1["Id"].ToString();
                data[1] = row1["Nume"].ToString();
                data[2] = row1["Cantitate"].ToString();
                data[3] = row1["Pret"].ToString();
                data[4] = row1["Tip_discount"].ToString();
                data[5] = row1["Tip_discount"].ToString() == "Procentual" ? row1["Discount"].ToString() + " %" : row1["Discount"].ToString();
                data[6] = row1["Brand"].ToString();
                data[7] = row1["Categorie"].ToString();

                dgv.Rows.Add(data);
            }
        }

        void load_on_parentForm()
        {
            foreach (Control c in parentForm.Controls)
                if (c.GetType() == typeof(TableLayoutPanel))
                    foreach (Control c1 in c.Controls)
                        if (c1.GetType() == typeof(DataGridView))
                        {
                            DataGridView dgv = (DataGridView)c1;
                            dgv.Rows.Clear();

                            fill_datagridview(dgv);
                        }
        }

        private void adauga_button_Click(object sender, EventArgs e)
        {
            if(nume_produs_textbox.Text == "" || (brand_combobox.SelectedIndex == -1 && add_brand_textbox.Text == "") || (categorie_combobox.SelectedIndex == -1 && add_brand_textbox.Text == "") || cantitate_textbox.Text == "" || pret_textbox.Text == "" || discount_textbox.Text == "")
            {
                MessageBox.Show("Date insuficiente!");
                return;
            }

            string nume = nume_produs_textbox.Text, brand = add_brand_textbox.Text, categorie = add_brand_textbox.Text, um = um_combobox.SelectedItem.ToString(), tip_discount = tip_discount_combobox.SelectedItem.ToString();
            double cantitate = Convert.ToDouble(cantitate_textbox.Text), pret = Convert.ToDouble(pret_textbox.Text), discount = Convert.ToDouble(discount_textbox.Text);

            if (brand_combobox.SelectedIndex >= 0)
                brand = brand_combobox.SelectedItem.ToString();
            if (categorie_combobox.SelectedIndex >= 0)
                categorie = categorie_combobox.SelectedItem.ToString();

            conn.Open();

            SqlCommand insert = new SqlCommand("INSERT INTO Stoc(Nume, Categorie, Brand, Cantitate, Pret, UM, Tip_discount, Discount) values(@nume, @categorie, @brand, @cantitate, @pret, @um, @tip_discount, @discount)", conn);
            insert.Parameters.Clear();
            insert.Parameters.AddWithValue("@nume", nume); insert.Parameters.AddWithValue("@categorie", categorie); insert.Parameters.AddWithValue("@brand", brand); insert.Parameters.AddWithValue("@cantitate", cantitate);
            insert.Parameters.AddWithValue("@pret", pret); insert.Parameters.AddWithValue("@um", um); insert.Parameters.AddWithValue("@tip_discount", tip_discount); insert.Parameters.AddWithValue("@discount", discount);
            
            insert.ExecuteNonQuery();
            insert.Dispose();

            int id = 0;

            SqlCommand query = new SqlCommand("SELECT Id FROM Stoc WHERE Nume = @nume and Pret = @pret", conn);
            query.Parameters.AddWithValue("@nume", nume); query.Parameters.AddWithValue("@pret", pret);
            SqlDataReader dr = query.ExecuteReader();
            dr.Read();
            id = Convert.ToInt32(dr[0].ToString());

            dr.Close(); query.Dispose();

            row = stoc.NewRow();

            row["Id"] = id; row["Nume"] = nume; row["Brand"] = brand; row["Cantitate"] = cantitate; row["Pret"] = pret;
            row["UM"] = um; row["Tip_discount"] = tip_discount; row["Discount"] = discount;

            stoc.Rows.Add(row);

            load_on_parentForm();

            conn.Close();

            this.Close();


        }
    }
}
