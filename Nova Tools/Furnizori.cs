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
    public partial class Furnizori : Form
    {
        SqlConnection conn;
        DataColumn col;
        DataTable furnizori;
        DataRow row;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                furnizori.Rows[e.RowIndex]["Nume"] = dataGridView1[1, e.RowIndex].Value.ToString();
                furnizori.Rows[e.RowIndex]["Adresa"] = dataGridView1[2, e.RowIndex].Value.ToString();
                furnizori.Rows[e.RowIndex]["Banca"] = dataGridView1[3, e.RowIndex].Value.ToString();
                furnizori.Rows[e.RowIndex]["Cont_bancar"] = dataGridView1[4, e.RowIndex].Value.ToString();

                conn.Open();

                SqlCommand update = new SqlCommand("UPDATE Furnizori SET Nume = @nume, Adresa = @adresa, Banca = @banca, Cont_bancar = @cont_bancar WHERE Id = @id", conn);
                update.Parameters.AddWithValue("@id", Convert.ToInt32(dataGridView1[0, e.RowIndex].Value.ToString()));
                update.Parameters.AddWithValue("@nume", dataGridView1[1, e.RowIndex].Value.ToString());
                update.Parameters.AddWithValue("@adresa", dataGridView1[2, e.RowIndex].Value.ToString());
                update.Parameters.AddWithValue("@banca", dataGridView1[3, e.RowIndex].Value.ToString());
                update.Parameters.AddWithValue("@cont_bancar", dataGridView1[4, e.RowIndex].Value.ToString());

                update.ExecuteNonQuery();

                dataGridView1.Rows.Clear();

                sort_furnizori();
                fill_datagridview();

                conn.Close();
            }
        }
        public Furnizori()
        {
            InitializeComponent();

            DataGridViewButtonColumn btncol = new DataGridViewButtonColumn();
            btncol.HeaderText = "Editare";
            btncol.Name = "Editare";
            btncol.Text = "Modifica";
            btncol.UseColumnTextForButtonValue = true;

            dataGridView1.Columns.Add(btncol);
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);

            dataGridView1.AllowUserToAddRows = false;

            furnizori = new DataTable();
            col = new DataColumn();

            col = new DataColumn("Index");
            col.DataType = System.Type.GetType("System.Int32");
            furnizori.Columns.Add(col);

            col = new DataColumn("Nume");
            col.DataType = System.Type.GetType("System.String");
            furnizori.Columns.Add(col);

            col = new DataColumn("Adresa");
            col.DataType = System.Type.GetType("System.String");
            furnizori.Columns.Add(col);

            col = new DataColumn("Banca");
            col.DataType = System.Type.GetType("System.String");
            furnizori.Columns.Add(col);

            col = new DataColumn("Cont_bancar");
            col.DataType = System.Type.GetType("System.String");
            furnizori.Columns.Add(col);
        }

        private void client_nou_button_Click(object sender, EventArgs e)
        {
            Furnizor_nou furnizor_nou = new Furnizor_nou(furnizori);
            furnizor_nou.parentForm = this;
            this.Hide();
            furnizor_nou.Show();
        }

        void sort_furnizori()
        {
            furnizori.DefaultView.Sort = "Nume ASC";
            furnizori = furnizori.DefaultView.ToTable();
        }

        void fill_datagridview()
        {
            string[] data = new string[5];

            foreach(DataRow row1 in furnizori.Rows)
            {
                data[0] = row1["Index"].ToString();
                data[1] = row1["Nume"].ToString();
                data[2] = row1["Adresa"].ToString();
                data[3] = row1["Banca"].ToString();
                data[4] = row1["Cont_bancar"].ToString();

                dataGridView1.Rows.Add(data);
            }
        }

        private void Furnizori_Load(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(path.Length - 9);
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);

            conn.Open();

            SqlCommand query = new SqlCommand("SELECT * FROM Furnizori", conn);
            SqlDataReader dr = query.ExecuteReader();

            while(dr.Read())
            {
                row = furnizori.NewRow();

                row["Index"] = Convert.ToInt32(dr[0].ToString());
                row["Nume"] = dr[1].ToString();
                row["Adresa"] = dr[2].ToString();
                row["Banca"] = dr[3].ToString();
                row["Cont_bancar"] = dr[4].ToString();

                furnizori.Rows.Add(row);
            }

            dr.Close(); query.Dispose();

            sort_furnizori();
            fill_datagridview();

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            conn.Close();

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToResizeRows = false;
        }
        bool contains(string text, DataRow row1)
        {
            if (row1["Nume"].ToString().ToLower().Contains(text.ToLower())) return true;
            if (row1["Adresa"].ToString().ToLower().Contains(text.ToLower())) return true;
            if (row1["Banca"].ToString().ToLower().Contains(text.ToLower())) return true;

            return false;
        }


        void search()
        {
            dataGridView1.Rows.Clear();
            string[] data = new string[5];
            string search_text = search_textbox.Text;
            if (search_text == "")
                fill_datagridview();
            else
            {
                foreach (DataRow row1 in furnizori.Rows)
                    if (contains(search_text, row1))
                    {
                        data[0] = row1["Index"].ToString();
                        data[1] = row1["Nume"].ToString();
                        data[2] = row1["Adresa"].ToString();
                        data[3] = row1["Banca"].ToString();
                        data[4] = row1["Cont_bancar"].ToString();

                        dataGridView1.Rows.Add(data);
                    }
            }
        }

        private void cauta_button_Click(object sender, EventArgs e)
        {
            search();
        }

        private void search_textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
                search();
        }
    }
}
