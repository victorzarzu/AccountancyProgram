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
using System.Runtime.InteropServices;

namespace Nova_Tools
{
    public partial class Stoc : Form
    {
        SqlConnection conn;
        DataTable stoc;
        DataColumn col;
        DataRow row;

        public Stoc()
        {
            InitializeComponent();

            stoc = new DataTable();

            col = new DataColumn("Id");
            col.DataType = System.Type.GetType("System.Int32");
            stoc.Columns.Add(col);

            col = new DataColumn("Nume");
            col.DataType = System.Type.GetType("System.String");
            stoc.Columns.Add(col);

            col = new DataColumn("Brand");
            col.DataType = System.Type.GetType("System.String");
            stoc.Columns.Add(col);

            col = new DataColumn("Categorie");
            col.DataType = System.Type.GetType("System.String");
            stoc.Columns.Add(col);

            col = new DataColumn("Cantitate");
            col.DataType = System.Type.GetType("System.Double");
            stoc.Columns.Add(col);

            col = new DataColumn("Pret");
            col.DataType = System.Type.GetType("System.Double");
            stoc.Columns.Add(col);

            col = new DataColumn("UM");
            col.DataType = System.Type.GetType("System.String");
            stoc.Columns.Add(col);

            col = new DataColumn("Tip_discount");
            col.DataType = System.Type.GetType("System.String");
            stoc.Columns.Add(col);

            col = new DataColumn("Discount");
            col.DataType = System.Type.GetType("System.Double");
            stoc.Columns.Add(col);
        }

        void load_data()
        {
            SqlCommand query = new SqlCommand("SELECT * FROM Stoc ORDER BY Nume ASC", conn);
            SqlDataReader dr = query.ExecuteReader();

            string[] data = new string[8];

            while(dr.Read())
            {
                row = stoc.NewRow();
                row["Id"] = Convert.ToInt32(dr[0]); data[0] = row["Id"].ToString();
                row["Nume"] = dr[1].ToString(); data[1] = row["Nume"].ToString();
                row["Brand"] = dr[2].ToString(); data[7] = row["Brand"].ToString();
                row["Categorie"] = dr[3].ToString(); data[6] = row["Categorie"].ToString();
                row["Cantitate"] = Convert.ToDouble(dr[4].ToString()); data[2] = row["Cantitate"].ToString();
                row["Pret"] = Convert.ToDouble(dr[5].ToString()); data[3] = row["Pret"].ToString();
                row["UM"] = dr[6].ToString();
                row["Tip_discount"] = dr[7].ToString(); data[4] = row["Tip_discount"].ToString();
                row["Discount"] = Convert.ToDouble(dr[8].ToString()); data[5] = row["Tip_discount"].ToString() == "Procentual" ? row["Discount"].ToString() + " %" : row["Discount"].ToString();

                stoc.Rows.Add(row);
                dataGridView1.Rows.Add(data);
            }

            dr.Close(); query.Dispose();
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

        private void Stoc_Load(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(path.Length - 9);
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);

            conn.Open();

            load_data();
            load_comboboxes();

            conn.Close();

            brand_combobox.SelectedIndex = categorie_combobox.SelectedIndex = 0;

            dataGridView1.RowHeadersVisible = dataGridView1.AllowUserToAddRows = dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        bool contains(string search_text, string brand, string categorie, DataRow row1)
        {
            if (!row1["Nume"].ToString().ToLower().Contains(search_text.ToLower())) return false;
            if (!row1["Brand"].ToString().ToLower().Contains(brand.ToLower())) return false;
            if (!row1["Categorie"].ToString().ToLower().Contains(categorie.ToLower())) return false;

            return true;
        }

        private void nou_button_Click(object sender, EventArgs e)
        {
            Stoc_nou stoc_nou = new Stoc_nou(stoc);
            stoc_nou.parentForm = this;
            this.Hide();
            stoc_nou.Show();
        }
        
        bool cont(string search_text, string brand, string categorie, DataRow row1)
        {
            if (!row1["Nume"].ToString().ToLower().Contains(search_text.ToLower())) return false;
            if (!row1["Brand"].ToString().ToLower().Contains(brand.ToLower())) return false;
            if (!row1["Categorie"].ToString().ToLower().Contains(categorie.ToLower())) return false;

            return true;
        }

        void search(string search_text, string brand, string categorie)
        {
            dataGridView1.Rows.Clear();
            string[] data = new string[8];

            foreach (DataRow row1 in stoc.Rows)
                if (cont(search_text, brand, categorie, row1))
                {
                    data[0] = row1["Id"].ToString();
                    data[1] = row1["Nume"].ToString();
                    data[2] = row1["Cantitate"].ToString();
                    data[3] = row1["Pret"].ToString();
                    data[4] = row1["Tip_discount"].ToString();
                    data[5] = row1["Discount"].ToString();
                    data[7] = row1["Brand"].ToString();
                    data[6] = row1["Categorie"].ToString();

                    dataGridView1.Rows.Add(data);
                }
        }

        private void search_button_Click(object sender, EventArgs e)
        {
            string search_text = "", brand = "", categorie = "";

            if (brand_combobox.SelectedIndex > 0)
                brand = brand_combobox.SelectedItem.ToString();
            if (categorie_combobox.SelectedIndex > 0)
                categorie = categorie_combobox.SelectedItem.ToString();
            search_text = search_textbox.Text;

            search(search_text, brand, categorie);
        }

        void delete_from_stoc(int id)
        {
            foreach (DataRow row1 in stoc.Rows)
                if (Convert.ToInt32(row1["Id"].ToString()) == id)
                {
                    stoc.Rows.Remove(row1);
                    break;
                }
            conn.Open();

            SqlCommand delete = new SqlCommand("DELETE FROM Stoc WHERE Id = @id", conn);
            delete.Parameters.AddWithValue("@id", id);
            delete.ExecuteNonQuery();

            delete.Dispose();

            conn.Close();
        }

        void update_stoc(int row_index)
        {
            int id = Convert.ToInt32(dataGridView1[0, row_index].Value.ToString());
            string nume = dataGridView1[1, row_index].Value.ToString(), tip_discount = dataGridView1[4, row_index].Value.ToString(), brand = dataGridView1[6, row_index].Value.ToString(), categorie = dataGridView1[7, row_index].Value.ToString();
            double cantitate = Convert.ToDouble(dataGridView1[2, row_index].Value.ToString()), pret = Convert.ToDouble(dataGridView1[3, row_index].Value.ToString()), discount = 0;
            string disc = dataGridView1[5, row_index].Value.ToString();
            string[] disc1 = disc.Split(' ');
            discount = Convert.ToDouble(disc1[0].Trim());

            conn.Open();

            SqlCommand update = new SqlCommand("UPDATE Stoc SET Nume = @nume, Categorie = @categorie, Brand = @brand, Cantitate = @cantitate, Pret = @pret, Tip_discount = @tip_discount, Discount = @discount WHERE Id = @id", conn);
            update.Parameters.AddWithValue("@nume", nume); update.Parameters.AddWithValue("@cantitate", cantitate); update.Parameters.AddWithValue("@pret", pret); update.Parameters.AddWithValue("@tip_discount", tip_discount);
            update.Parameters.AddWithValue("@discount", discount); update.Parameters.AddWithValue("@brand", brand); update.Parameters.AddWithValue("@categorie", categorie); update.Parameters.AddWithValue("@id", id);

            update.ExecuteNonQuery();
            update.Dispose();

            conn.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 8)
                return;
            if (e.ColumnIndex == 9)
            {
                delete_from_stoc(Convert.ToInt32(dataGridView1[0, e.RowIndex].Value.ToString()));
                dataGridView1.Rows.RemoveAt(e.RowIndex);
            }
            else
                update_stoc(e.RowIndex);
        }
    }
}
